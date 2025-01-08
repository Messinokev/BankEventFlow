using EventFlow.Queries;

namespace BankEventFlow;

public class GetAccountBalanceByIdQuery(AccountId accountId) : ReadModelByIdQuery<AccountReadModel>(accountId)
{
    public AccountId AccountId { get; } = accountId;
}