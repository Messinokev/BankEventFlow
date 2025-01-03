using FluentAssertions;

namespace BankEventFlow;

public class DepositEventTests
{
    [Fact]
    public void DepositEvent_ShouldInitializeCorrectly()
    {
        // Arrange
        decimal depositAmount = 200;

        // Act
        var depositEvent = new DepositEvent(depositAmount);

        // Assert
        depositEvent.Amount.Should().Be(depositAmount);
    }
}