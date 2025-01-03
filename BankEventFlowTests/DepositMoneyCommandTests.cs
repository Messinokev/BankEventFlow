using FluentAssertions;

namespace BankEventFlow;

public class DepositMoneyCommandTests
{
    [Fact]
    public void DepositCommand_ShouldInitializeCorrectly()
    {
        // Arrange
        var accountId = AccountId.New;
        decimal depositAmount = 100;

        // Act
        var command = new DepositMoneyCommand(accountId, depositAmount);

        // Assert
        command.AggregateId.Should().Be(accountId);
        command.Amount.Should().Be(depositAmount);
    }
}