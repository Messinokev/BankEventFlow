using EventFlow.Aggregates;

namespace BankEventFlow;

public class DepositEvent : AggregateEvent<AccountAggregate, AccountId>
{
    public AccountId SourceAccountId { get; }
    public AccountId TargetAccountId { get; }
    public decimal Amount { get; }

    public DepositEvent(decimal amount)
    {
        Amount = amount;
    }
}