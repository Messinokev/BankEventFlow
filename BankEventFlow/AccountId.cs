using EventFlow.Core;

namespace BankEventFlow;

public class AccountId : Identity<AccountId>
{
    public AccountId(string value) : base(value)
    {
    }
}