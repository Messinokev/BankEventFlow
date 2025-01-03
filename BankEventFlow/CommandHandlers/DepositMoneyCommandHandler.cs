using EventFlow.Commands;

namespace BankEventFlow;

public class DepositMoneyCommandHandler : CommandHandler<AccountAggregate, AccountId, DepositMoneyCommand>
{
    public override Task ExecuteAsync(AccountAggregate aggregate, DepositMoneyCommand depositMoneyCommand,
        CancellationToken cancellationToken)
    {
        aggregate.Deposit(depositMoneyCommand.Amount);
        return Task.CompletedTask;
    }
}