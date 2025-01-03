using EventFlow.Aggregates;

namespace BankEventFlow;

public class AccountReadModelProjectionTests
{
    private AccountReadModelProjection _projection = new();

    [Fact]
    public async Task ApplyAsync_Should_Update_Balance_On_Deposit()
    {
        // Arrange
        var accountId = AccountId.New;
        var depositEvent = new DepositedMoneyEvent(100);
        var domainEvent = new DomainEvent<AccountAggregate, AccountId, DepositedMoneyEvent>(
            depositEvent,
            new Metadata(),
            DateTimeOffset.UtcNow,
            accountId,
            1);

        // Act
        await _projection.ApplyAsync(null, domainEvent, CancellationToken.None);

        // Assert
        Assert.Equal(100, _projection.GetBalance(accountId));
    }

    [Fact]
    public async Task ApplyAsync_Should_Update_Balance_On_Withdrawal()
    {
        // Arrange
        var accountId = AccountId.New;
        var depositEvent = new DepositedMoneyEvent(100);
        var withdrawEvent = new WithdrawedMoneyEvent(40);
        var domainDepositEvent = new DomainEvent<AccountAggregate, AccountId, DepositedMoneyEvent>(
            depositEvent,
            new Metadata(),
            DateTimeOffset.UtcNow,
            accountId,
            1);
        var domainWithdrawEvent = new DomainEvent<AccountAggregate, AccountId, WithdrawedMoneyEvent>(
            withdrawEvent,
            new Metadata(),
            DateTimeOffset.UtcNow,
            accountId,
            2);

        // Act
        await _projection.ApplyAsync(null, domainDepositEvent, CancellationToken.None);
        await _projection.ApplyAsync(null, domainWithdrawEvent, CancellationToken.None);

        // Assert
        Assert.Equal(60, _projection.GetBalance(accountId));
    }
}