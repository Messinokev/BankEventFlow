using EventFlow.Commands;

namespace BankEventFlow;

public class TransferMoneyCommand: Command<AccountAggregate, AccountId>
{
    public AccountId SourceAccountId { get; }
    public AccountId TargetAccountId { get; }
    public decimal Amount { get; }
    
    public TransferMoneyCommand(AccountId sourceAccountId ,decimal amount, AccountId targetAccountId) : base(sourceAccountId)
    {
        Amount = amount;
        SourceAccountId = sourceAccountId;
        TargetAccountId = targetAccountId;
    }
}