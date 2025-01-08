using EventFlow.Queries;
using Moq;

namespace BankEventFlow;

public class AccountServiceTests
{
    private readonly Mock<IQueryProcessor> _queryProcessorMock;
    private readonly IAccountService _accountService;

    public AccountServiceTests()
    {
        _queryProcessorMock = new Mock<IQueryProcessor>();
        _accountService = new AccountService(_queryProcessorMock.Object);
    }

    [Fact]
    public async Task GetAccountBalanceAsync_Should_Return_Correct_Balance()
    {
        // Arrange
        var accountId = AccountId.New;
        var expectedBalance = 150m;

        var accountReadModel = new AccountReadModel
        {
            AccountId = accountId,
            Balance = expectedBalance
        };

        _queryProcessorMock
            .Setup(qp => qp.ProcessAsync(It.IsAny<GetAccountBalanceByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountReadModel);

        // Act
        var result = await _accountService.GetAccountBalanceAsync(accountId, CancellationToken.None);

        // Assert
        Assert.Equal(expectedBalance, result);
    }

    [Fact]
    public async Task GetAccountBalanceAsync_Should_Throw_Exception_When_Account_Not_Found()
    {
        // Arrange
        var accountId = AccountId.New;

        _queryProcessorMock
            .Setup(qp => qp.ProcessAsync(It.IsAny<GetAccountBalanceByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AccountReadModel)null); // Simulate account not found

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _accountService.GetAccountBalanceAsync(accountId, CancellationToken.None));
    }
}