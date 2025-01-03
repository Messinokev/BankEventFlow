using EventFlow.Aggregates;
using EventFlow.ReadStores;

namespace BankEventFlow;

public class AccountReadModel : IReadModel
{
    public AccountId AccountId { get; set; } = null!;
    public decimal Balance { get; set; }
}

public class AccountReadModelProjection : IReadModel,
    IAmReadModelFor<AccountAggregate, AccountId, DepositedMoneyEvent>,
    IAmReadModelFor<AccountAggregate, AccountId, WithdrawedMoneyEvent>

{
    private readonly Dictionary<AccountId, AccountReadModel> _accountReadModels = new();

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<AccountAggregate, AccountId, DepositedMoneyEvent> domainEvent, CancellationToken cancellationToken)
    {
        var accountId = domainEvent.AggregateIdentity;

        if (!_accountReadModels.TryGetValue(accountId, out var accountReadModel))
        {
            accountReadModel = new AccountReadModel
            {
                AccountId = accountId,
                Balance = 0
            };
            _accountReadModels[accountId] = accountReadModel;
        }

        accountReadModel.Balance += domainEvent.AggregateEvent.Amount;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<AccountAggregate, AccountId, WithdrawedMoneyEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var accountId = domainEvent.AggregateIdentity;

        if (!_accountReadModels.TryGetValue(accountId, out var accountReadModel))
        {
            accountReadModel = new AccountReadModel
            {
                AccountId = accountId,
                Balance = 0
            };
            _accountReadModels[accountId] = accountReadModel;
        }

        accountReadModel.Balance -= domainEvent.AggregateEvent.Amount;

        return Task.CompletedTask;
    }

    public decimal GetBalance(AccountId accountId)
    {
        if (_accountReadModels.TryGetValue(accountId, out var accountReadModel))
        {
            return accountReadModel.Balance;
        }

        return 0;
    }
}