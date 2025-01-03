using EventFlow.Commands;

namespace BankEventFlow;

public class TransferCommandHandler : CommandHandler<AccountAggregate, AccountId, TransferCommand>
{
    public override Task ExecuteAsync(AccountAggregate aggregate, TransferCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.Transfer(command.SourceAccountId, command.Amount, command.TargetAccountId);
        return Task.CompletedTask;
    }
}