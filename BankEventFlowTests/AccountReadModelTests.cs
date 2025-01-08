using BankEventFlow.Sagas;
using EventFlow;
using EventFlow.Extensions;
using EventFlow.ReadStores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BankEventFlow;

public class AccountReadModelTests
{
    private IServiceProvider _serviceProvider;
    private ICommandBus _commandBus;
    private IReadModelStore<AccountReadModel> _readModelStore;

    private const decimal InitialDepositAmount = 100m;
    private const decimal TransferAmount = 50m;

    public AccountReadModelTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection
            .AddLogging(configure => configure.AddConsole())
            .AddEventFlow(ef =>
            {
                ef.UseInMemoryReadStoreFor<AccountReadModel>();
                ef.AddCommands(typeof(DepositMoneyCommand), typeof(WithdrawMoneyCommand), typeof(TransferMoneyCommand));
                ef.AddEvents(typeof(DepositedMoneyEvent), typeof(WithdrawedMoneyEvent), typeof(TransferedMoneyEvent));
                ef.AddCommandHandlers(typeof(DepositMoneyCommandHandler), typeof(WithdrawMoneyCommandHandler),
                    typeof(TransferMoneyCommandHandler));
                ef.AddSagas(typeof(TransferSaga));
                ef.RegisterServices(sc => sc.AddSingleton<TransferSagaLocator>());
            })
            .AddTransient<ICommandBus, CommandBus>();

        _serviceProvider = serviceCollection.BuildServiceProvider();
        _commandBus = _serviceProvider.GetRequiredService<ICommandBus>();
        _readModelStore = _serviceProvider.GetRequiredService<IReadModelStore<AccountReadModel>>();
    }

    private async Task<AccountReadModel> GetReadModelAsync(AccountId accountId, CancellationToken cancellationToken)
    {
        var accountReadModel = await _readModelStore.GetAsync(accountId.Value, cancellationToken);
        if (accountReadModel == null)
        {
            throw new InvalidOperationException($"Account read model for {accountId} not found.");
        }

        return accountReadModel.ReadModel;
    }

    private async Task DepositMoney(AccountId accountId, decimal amount)
    {
        var depositCommand = new DepositMoneyCommand(accountId, amount);
        await _commandBus.PublishAsync(depositCommand, CancellationToken.None);
    }

    private async Task TransferMoney(AccountId sourceAccountId, AccountId targetAccountId, decimal amount)
    {
        var transferCommand = new TransferMoneyCommand(sourceAccountId, amount, targetAccountId);
        await _commandBus.PublishAsync(transferCommand, CancellationToken.None);
    }

    [Fact]
    public async Task Command_Should_Deposit_Both_Source_And_Target_Balances()
    {
        // Arrange
        var sourceAccountId = AccountId.New;
        var targetAccountId = AccountId.New;

        // Act
        await DepositMoney(sourceAccountId, InitialDepositAmount);
        await DepositMoney(targetAccountId, InitialDepositAmount);

        // Assert
        var sourceAccountReadModel = await GetReadModelAsync(sourceAccountId, CancellationToken.None);
        var targetAccountReadModel = await GetReadModelAsync(targetAccountId, CancellationToken.None);

        Assert.Equal(InitialDepositAmount, sourceAccountReadModel.Balance);
        Assert.Equal(InitialDepositAmount, targetAccountReadModel.Balance);
    }

    [Fact]
    public async Task Command_Should_Update_Both_Source_And_Target_Balances_On_Transfer()
    {
        // Arrange
        var sourceAccountId = AccountId.New;
        var targetAccountId = AccountId.New;

        // Step 1: Deposit money into source and target accounts
        await DepositMoney(sourceAccountId, InitialDepositAmount);
        await DepositMoney(targetAccountId, InitialDepositAmount);

        // Step 2: Transfer money from source to target
        await TransferMoney(sourceAccountId, targetAccountId, TransferAmount);

        // Assert the balances
        var sourceAccountReadModel = await GetReadModelAsync(sourceAccountId, CancellationToken.None);
        var targetAccountReadModel = await GetReadModelAsync(targetAccountId, CancellationToken.None);

        Assert.Equal(InitialDepositAmount - TransferAmount, sourceAccountReadModel.Balance);
        Assert.Equal(InitialDepositAmount + TransferAmount, targetAccountReadModel.Balance);
    }
}