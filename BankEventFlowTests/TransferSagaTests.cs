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
        var transferEvent = new TransferedMoneyEvent(sourceAccountId, amount, targetAccountId);
        
        var domainEvent = new DomainEvent<AccountAggregate, AccountId, TransferedMoneyEvent>(
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
            bus => bus.PublishAsync(It.Is<WithdrawMoneyCommand>(wc => wc.Amount == amount),
                CancellationToken.None),
            Times.Once);
        _commandBusMock.Verify(
            bus => bus.PublishAsync(It.Is<DepositMoneyCommand>(dc => dc.Amount == amount),
                CancellationToken.None),
            Times.Once);
    }
}