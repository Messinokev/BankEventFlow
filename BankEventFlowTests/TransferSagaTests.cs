using BankEventFlow.Sagas;
using EventFlow;
using EventFlow.Aggregates;
using EventFlow.Sagas;
using Moq;

namespace BankEventFlow;

public class TransferSagaTests
{
    private readonly Mock<ICommandBus> _commandBusMock;

    public TransferSagaTests()
    {
        _commandBusMock = new Mock<ICommandBus>();
    }

    [Fact]
    public async Task HandleAsync_ShouldPublishWithdrawCommand_WhenTransferEventIsReceived()
    {
        // Arrange
        var sagaId = new TransferSagaId(Guid.NewGuid().ToString());
        var sourceAccountId = AccountId.New;
        var targetAccountId = AccountId.New;
        var amount = 100m;
        var transferEvent = new TransferEvent(sourceAccountId, amount, targetAccountId);
        
        var domainEvent = new DomainEvent<AccountAggregate, AccountId, TransferEvent>(
            aggregateEvent: transferEvent,
            metadata: Mock.Of<IMetadata>(),
            timestamp: DateTimeOffset.Now,
            aggregateIdentity: sourceAccountId,
            aggregateSequenceNumber: 1
        );

        var saga = new TransferSaga(sagaId, _commandBusMock.Object);

        // Act
        await saga.HandleAsync(domainEvent, Mock.Of<ISagaContext>(), CancellationToken.None);

        // Assert
        _commandBusMock.Verify(
            bus => bus.PublishAsync(It.Is<WithdrawCommand>(wc => wc.Amount == amount),
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task ApplyAsync_ShouldPublishDepositCommand_WhenWithdrawEventIsApplied()
    {
        // Arrange
        var sagaId = new TransferSagaId(Guid.NewGuid().ToString());
        var targetAccountId = AccountId.New;
        var amount = 100m;
    
        var withdrawEvent = new WithdrawEvent(amount);
        var saga = new TransferSaga(sagaId, _commandBusMock.Object)
        {
            _targetAccountId = targetAccountId
        };

        // Act
        saga.Apply(withdrawEvent);
    
        // Assert
        _commandBusMock.Verify(
            bus => bus.PublishAsync(It.Is<DepositCommand>(dc => dc.Amount == amount),
                CancellationToken.None),
            Times.Once);
    }
}