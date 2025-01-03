using EventFlow.Commands;

namespace BankEventFlow;

public class DepositMoneyCommand : Command<AccountAggregate, AccountId>
{
    public decimal Amount { get; }
    
    public DepositMoneyCommand(AccountId accountId ,decimal amount) : base(accountId)
    {
        Amount = amount;
    }
}