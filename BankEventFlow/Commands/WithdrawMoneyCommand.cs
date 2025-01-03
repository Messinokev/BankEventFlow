using EventFlow.Commands;

namespace BankEventFlow;

public class WithdrawMoneyCommand : Command<AccountAggregate, AccountId>
{
    public decimal Amount { get; }
    
    public WithdrawMoneyCommand(AccountId accountId ,decimal amount) : base(accountId)
    {
        Amount = amount;
    }
}