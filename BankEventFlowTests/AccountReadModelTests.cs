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
    private IReadModelStore<AccountReadModelProjection> _readModelStore;

    public AccountReadModelTests()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddLogging(configure => configure.AddConsole())
            .AddEventFlow(ef =>
            {
                ef.UseInMemoryReadStoreFor<AccountReadModelProjection>();
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
        _readModelStore = _serviceProvider.GetRequiredService<IReadModelStore<AccountReadModelProjection>>();
    }

    [Fact]
    public async Task Command_Should_Deposit_Both_Source_And_Target_Balances()
    {
        // Arrange
        var sourceAccountId = AccountId.New;
        var targetAccountId = AccountId.New;
        var initialDepositAmount = 100;

        // Act

        // Step 1: Deposit 100 into the source account
        var depositToSourceAccount = new DepositMoneyCommand(sourceAccountId, initialDepositAmount);
        await _commandBus.PublishAsync(depositToSourceAccount, CancellationToken.None);

        // Step 2: Deposit 100 into the target account
        var depositToTargetAccount = new DepositMoneyCommand(targetAccountId, initialDepositAmount);
        await _commandBus.PublishAsync(depositToTargetAccount, CancellationToken.None);

        // Assert

        // Retrieve the updated read models
        var sourceAccountReadModel = await _readModelStore.GetAsync(sourceAccountId.Value, CancellationToken.None);
        var targetAccountReadModel = await _readModelStore.GetAsync(targetAccountId.Value, CancellationToken.None);

        Assert.Equal(100, sourceAccountReadModel.ReadModel.GetBalance(sourceAccountId));
        Assert.Equal(100, targetAccountReadModel.ReadModel.GetBalance(targetAccountId));
    }

    [Fact]
    public async Task Command_Should_Update_Both_Source_And_Target_Balances_On_Transfer()
    {
        // Arrange
        var sourceAccountId = AccountId.New;
        var targetAccountId = AccountId.New;
        var initialDepositAmount = 100;
        var transferAmount = 50;

        // Act

        // Step 1: Deposit 100 into the source account
        var depositToSourceAccount = new DepositMoneyCommand(sourceAccountId, initialDepositAmount);
        await _commandBus.PublishAsync(depositToSourceAccount, CancellationToken.None);

        // Step 2: Deposit 100 into the target account
        var depositToTargetAccount = new DepositMoneyCommand(targetAccountId, initialDepositAmount);
        await _commandBus.PublishAsync(depositToTargetAccount, CancellationToken.None);

        // Step 3: Transfer 50 from the source account to the target account
        var transferCommand = new TransferMoneyCommand(sourceAccountId, transferAmount, targetAccountId);
        await _commandBus.PublishAsync(transferCommand, CancellationToken.None);

        // Assert

        // Retrieve the updated read models
        var sourceAccountReadModel = await _readModelStore.GetAsync(sourceAccountId.Value, CancellationToken.None);
        var targetAccountReadModel = await _readModelStore.GetAsync(targetAccountId.Value, CancellationToken.None);

        // After transfer, the source account should have 50 (100 - 50)
        Assert.Equal(50, sourceAccountReadModel.ReadModel.GetBalance(sourceAccountId));
        // The target account should have 150 (100 + 50)
        Assert.Equal(150, targetAccountReadModel.ReadModel.GetBalance(targetAccountId));
    }
}