using EventFlow.Aggregates;
using EventFlow.Exceptions;

namespace BankEventFlow;

public class AccountAggregate : AggregateRoot<AccountAggregate, AccountId>, IEmit<DepositedMoneyEvent>, IEmit<WithdrawedMoneyEvent>,
    IEmit<TransferedMoneyEvent>
{
    public decimal Balance { get; private set; }

    public AccountAggregate(AccountId id) : base(id)
    {
    }

    public void Apply(DepositedMoneyEvent depositedMoneyEvent)
    {
        Balance += depositedMoneyEvent.Amount;
    }

    public void Apply(WithdrawedMoneyEvent withdrawedMoneyEvent)
    {
        Balance -= withdrawedMoneyEvent.Amount;
    }

    public void Apply(TransferedMoneyEvent aggregateEvent)
    {
    }

    public Task Deposit(decimal amount)
    {
        if (decimal.IsNegative(amount))
        {
            throw DomainError.With("Deposit amount cannot be negative");
        }

        Emit(new DepositedMoneyEvent(amount));
        return Task.CompletedTask;
    }

    public Task Withdraw(decimal amount)
    {
        if (decimal.IsNegative(amount))
        {
            throw DomainError.With("Withdraw amount cannot be negative");
        }

        if (Balance < amount)
        {
            throw DomainError.With("Balance needs to be more than withdraw amount");
        }

        Emit(new WithdrawedMoneyEvent(amount));
        return Task.CompletedTask;
    }

    public Task Transfer(AccountId sourceAccountId, decimal amount, AccountId targetAccountId)
    {
        if (sourceAccountId.Value == targetAccountId.Value)
        {
            throw DomainError.With("The two accounts cannot be the same");
        }

        Emit(new TransferedMoneyEvent(sourceAccountId, amount, targetAccountId));
        return Task.CompletedTask;
    }
}