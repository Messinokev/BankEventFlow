using EventFlow.Aggregates;

namespace BankEventFlow;

public class WithdrawedMoneyEvent : AggregateEvent<AccountAggregate, AccountId>
{
    public decimal Amount { get; }

    public WithdrawedMoneyEvent(decimal amount)
    {
        Amount = amount;
    }
}