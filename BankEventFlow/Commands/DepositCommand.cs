using EventFlow.Commands;

namespace BankEventFlow;

public class DepositCommand : Command<AccountAggregate, AccountId>
{
    public decimal Amount { get; }
    
    public DepositCommand(AccountId accountId ,decimal amount) : base(accountId)
    {
        Amount = amount;
    }
}