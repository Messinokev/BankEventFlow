using EventFlow.Commands;

namespace BankEventFlow;

public class DepositCommandHandler : CommandHandler<AccountAggregate, AccountId, DepositCommand>
{
    public override Task ExecuteAsync(AccountAggregate aggregate, DepositCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.Deposit(command.Amount);
        return Task.CompletedTask;
    }
}