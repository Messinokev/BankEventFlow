using FluentAssertions;

namespace BankEventFlow;

public class DepositCommandHandlerTests
{
    [Fact]
    public async Task DepositCommandHandler_ShouldInvokeDepositOnAggregate()
    {
        //Arrange
        var accountId = AccountId.New;
        var aggregate = new AccountAggregate(accountId);
        var commandHandler = new DepositCommandHandler();
        decimal depositAmount = 500;
        var depositCommand = new DepositCommand(accountId, depositAmount);
        
        //Act
        await commandHandler.ExecuteAsync(aggregate, depositCommand, CancellationToken.None);
        
        //Assert
        var uncommittedEvents = aggregate.UncommittedEvents;
        uncommittedEvents.Should().ContainSingle(e => e.AggregateEvent is DepositEvent)
            .Which.AggregateEvent.As<DepositEvent>().Amount.Should().Be(depositAmount);
        aggregate.Balance.Should().Be(depositAmount);
    }
}