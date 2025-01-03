using EventFlow.Commands;

namespace BankEventFlow;

public class WithdrawMoneyCommandHandler : CommandHandler<AccountAggregate, AccountId, WithdrawMoneyCommand>
{
    public override Task ExecuteAsync(AccountAggregate aggregate, WithdrawMoneyCommand withdrawMoneyCommand,
        CancellationToken cancellationToken)
    {
        aggregate.Withdraw(withdrawMoneyCommand.Amount);
        return Task.CompletedTask;
    }
}