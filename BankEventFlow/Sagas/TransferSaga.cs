using EventFlow;
using EventFlow.Aggregates;
using EventFlow.Sagas;
using EventFlow.Sagas.AggregateSagas;
using EventFlow.ValueObjects;

namespace BankEventFlow.Sagas;

public class TransferSaga : AggregateSaga<TransferSaga, TransferSagaId, TransferSagaLocator>,
    ISagaIsStartedBy<AccountAggregate, AccountId, TransferEvent>,
    IApply<WithdrawEvent>
{
    private readonly ICommandBus _commandBus;
    public AccountId _targetAccountId;

    public TransferSaga(TransferSagaId id, ICommandBus commandBus) : base(id)
    {
        _commandBus = commandBus ?? throw new ArgumentNullException(nameof(commandBus));
    }

    public async Task HandleAsync(IDomainEvent<AccountAggregate, AccountId, TransferEvent> domainEvent,
        ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        _targetAccountId = domainEvent.AggregateEvent.TargetAccountId;

        var withdrawalCommand =
            new WithdrawCommand(domainEvent.AggregateEvent.SourceAccountId, domainEvent.AggregateEvent.Amount);

        // Send the withdrawal command
        await _commandBus.PublishAsync(withdrawalCommand, cancellationToken);
    }

    public void Apply(WithdrawEvent withdrawEvent)
    {
        var depositCommand = new DepositCommand(_targetAccountId, withdrawEvent.Amount);

        // Send the deposit command
        Task.Run(async () =>
        {
            await _commandBus.PublishAsync(depositCommand, CancellationToken.None);
        });
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