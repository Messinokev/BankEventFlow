using EventFlow.Aggregates;

namespace BankEventFlow;

public class TransferEvent : AggregateEvent<AccountAggregate, AccountId>
{
    public AccountId SourceAccountId { get; }
    public AccountId TargetAccountId { get; }
    public decimal Amount { get; }

    public TransferEvent(AccountId sourceAccountId, decimal amount, AccountId targetAccountId)
    {
        Amount = amount;
        SourceAccountId = sourceAccountId;
        TargetAccountId = targetAccountId;
    }
}