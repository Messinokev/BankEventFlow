using FluentAssertions;

namespace BankEventFlow;

public class DepositMoneyCommandHandlerTests
{
    [Fact]
    public async Task DepositCommandHandler_ShouldInvokeDepositOnAggregate()
    {
        //Arrange
        var accountId = AccountId.New;
        var aggregate = new AccountAggregate(accountId);
        var commandHandler = new DepositMoneyCommandHandler();
        decimal depositAmount = 500;
        var depositCommand = new DepositMoneyCommand(accountId, depositAmount);
        
        //Act
        await commandHandler.ExecuteAsync(aggregate, depositCommand, CancellationToken.None);
        
        //Assert
        var uncommittedEvents = aggregate.UncommittedEvents;
        uncommittedEvents.Should().ContainSingle(e => e.AggregateEvent is DepositedMoneyEvent)
            .Which.AggregateEvent.As<DepositedMoneyEvent>().Amount.Should().Be(depositAmount);
        aggregate.Balance.Should().Be(depositAmount);
    }
}