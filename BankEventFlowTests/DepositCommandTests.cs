using FluentAssertions;

namespace BankEventFlow;

public class DepositCommandTests
{
    [Fact]
    public void DepositCommand_ShouldInitializeCorrectly()
    {
        // Arrange
        var accountId = AccountId.New;
        decimal depositAmount = 100;

        // Act
        var command = new DepositCommand(accountId, depositAmount);

        // Assert
        command.AggregateId.Should().Be(accountId);
        command.Amount.Should().Be(depositAmount);
    }
}