using FluentAssertions;

namespace BankEventFlow;

public class DepositedMoneyEventTests
{
    [Fact]
    public void DepositEvent_ShouldInitializeCorrectly()
    {
        // Arrange
        decimal depositAmount = 200;

        // Act
        var depositEvent = new DepositedMoneyEvent(depositAmount);

        // Assert
        depositEvent.Amount.Should().Be(depositAmount);
    }
}