using EventFlow.Exceptions;
using FluentAssertions;
using Xunit;

namespace BankEventFlow;

public class AccountAggregateTests
{
    [Fact]
    public void Deposit_WithValidAmount_ShouldEmitDepositEvent()
    {
        //Arrange
        var accountId = AccountId.New;
        var aggregate = new AccountAggregate(accountId);

        //Act
        decimal depositAmount = 100;
        aggregate.Deposit(depositAmount);

        //Assert
        var uncommittedEvents = aggregate.UncommittedEvents;
        uncommittedEvents.Should().ContainSingle(e => e.AggregateEvent is DepositedMoneyEvent)
            .Which.AggregateEvent.As<DepositedMoneyEvent>().Amount.Should().Be(depositAmount);
        aggregate.Balance.Should().Be(depositAmount);
    }

    [Fact]
    public void Deposit_WithNegativeAmount_ShouldThrowException()
    {
        // Arrange
        var accountId = AccountId.New;
        var aggregate = new AccountAggregate(accountId);

        // Act
        Action depositAction = () => aggregate.Deposit(-50);

        // Assert
        depositAction.Should().Throw<DomainError>()
            .WithMessage("Deposit amount cannot be negative");
    }

    [Fact]
    public void ApplyDepositEvent_ShouldUpdateBalance()
    {
        // Arrange
        var accountId = AccountId.New;
        var aggregate = new AccountAggregate(accountId);

        // Act
        decimal depositAmount = 150;
        aggregate.Apply(new DepositedMoneyEvent(depositAmount));

        // Assert
        aggregate.Balance.Should().Be(depositAmount);
    }
    
    [Fact]
    public void Deposit_Withdraw_Deposit_ShouldUpdateBalanceCorrectly()
    {
        // Arrange
        var accountId = AccountId.New;
        var aggregate = new AccountAggregate(accountId);

        // Act
        var initialDepositAmount = 1000m;
        var withdrawalAmount = 300m;
        var secondDepositAmount = 200m;
        
        aggregate.Deposit(initialDepositAmount);
        aggregate.Withdraw(withdrawalAmount);
        aggregate.Deposit(secondDepositAmount);

        // Assert
        var expectedBalance = initialDepositAmount - withdrawalAmount + secondDepositAmount;
        aggregate.Balance.Should().Be(expectedBalance);
        aggregate.Balance.Should().Be(900);
    }
}