using EventFlow;
using EventFlow.Aggregates;
using EventFlow.Sagas;
using EventFlow.Sagas.AggregateSagas;
using EventFlow.ValueObjects;

namespace BankEventFlow.Sagas;

public class TransferSaga : AggregateSaga<TransferSaga, TransferSagaId, TransferSagaLocator>,
    ISagaIsStartedBy<AccountAggregate, AccountId, TransferedMoneyEvent>
{
    private readonly ICommandBus _commandBus;

    public TransferSaga(TransferSagaId id, ICommandBus commandBus) : base(id)
    {
        _commandBus = commandBus ?? throw new ArgumentNullException(nameof(commandBus));
    }

    public async Task HandleAsync(IDomainEvent<AccountAggregate, AccountId, TransferedMoneyEvent> domainEvent,
        ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        var withdrawalCommand =
            new WithdrawMoneyCommand(domainEvent.AggregateEvent.SourceAccountId, domainEvent.AggregateEvent.Amount);
        await _commandBus.PublishAsync(withdrawalCommand, cancellationToken);

        var depositCommand =
            new DepositMoneyCommand(domainEvent.AggregateEvent.TargetAccountId, domainEvent.AggregateEvent.Amount);
        await _commandBus.PublishAsync(depositCommand, cancellationToken);
        
        Complete();
    }
}

public class TransferSagaLocator : ISagaLocator
{
    public Task<ISagaId> LocateSagaAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var eventVersion = domainEvent.Metadata["event_version"];
        var transferSagaId = new TransferSagaId($"transfer-{eventVersion}");

        return Task.FromResult<ISagaId>(transferSagaId);
    }
}

public class TransferSagaId : SingleValueObject<string>, ISagaId
{
    public TransferSagaId(string value)
        : base(value)
    {
    }
}