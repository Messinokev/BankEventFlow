using EventFlow.Aggregates;

namespace BankEventFlow;

public class DepositedMoneyEvent : AggregateEvent<AccountAggregate, AccountId>
{
    public decimal Amount { get; }

    public DepositedMoneyEvent(decimal amount)
    {
        Amount = amount;
    }
}