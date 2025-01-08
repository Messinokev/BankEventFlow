using EventFlow.Aggregates;
using EventFlow.ReadStores;

namespace BankEventFlow;

public class AccountReadModel : IReadModel,
    IAmReadModelFor<AccountAggregate, AccountId, DepositedMoneyEvent>,
    IAmReadModelFor<AccountAggregate, AccountId, WithdrawedMoneyEvent>

{
    public AccountId AccountId { get; set; } = null!;
    public decimal Balance { get; set; }


    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<AccountAggregate, AccountId, DepositedMoneyEvent> domainEvent, CancellationToken cancellationToken)
    {
        AccountId = domainEvent.AggregateIdentity;
        Balance += domainEvent.AggregateEvent.Amount;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<AccountAggregate, AccountId, WithdrawedMoneyEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        AccountId = domainEvent.AggregateIdentity;
        Balance -= domainEvent.AggregateEvent.Amount;
        return Task.CompletedTask;
    }
}