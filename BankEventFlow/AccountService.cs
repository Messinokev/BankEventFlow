using EventFlow.Queries;

namespace BankEventFlow;

public interface IAccountService
{
    Task<decimal> GetAccountBalanceAsync(AccountId accountId, CancellationToken cancellationToken);
}

public class AccountService : IAccountService
{
    private IQueryProcessor _queryProcessor;


    public AccountService(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    public async Task<decimal> GetAccountBalanceAsync(AccountId accountId, CancellationToken cancellationToken)
    {
        var query = new GetAccountBalanceByIdQuery(accountId);
        var accountReadModel = await _queryProcessor.ProcessAsync(query, cancellationToken);

        if (accountReadModel == null)
        {
            throw new Exception($"Account {accountId} not found");
        }

        return accountReadModel.Balance;
    }
}