using EventFlow.Aggregates;

namespace BankEventFlow;

public class WithdrawEvent : AggregateEvent<AccountAggregate, AccountId>
{
    public decimal Amount { get; }

    public WithdrawEvent(decimal amount)
    {
        Amount = amount;
    }
}