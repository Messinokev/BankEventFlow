using EventFlow.Commands;

namespace BankEventFlow;

public class TransferMoneyCommandHandler : CommandHandler<AccountAggregate, AccountId, TransferMoneyCommand>
{
    public override Task ExecuteAsync(AccountAggregate aggregate, TransferMoneyCommand transferMoneyCommand,
        CancellationToken cancellationToken)
    {
        aggregate.Transfer(transferMoneyCommand.SourceAccountId, transferMoneyCommand.Amount, transferMoneyCommand.TargetAccountId);
        return Task.CompletedTask;
    }
}