using EventFlow.Commands;

namespace BankEventFlow;

public class WithdrawCommand : Command<AccountAggregate, AccountId>
{
    public decimal Amount { get; }
    
    public WithdrawCommand(AccountId accountId ,decimal amount) : base(accountId)
    {
        Amount = amount;
    }
}