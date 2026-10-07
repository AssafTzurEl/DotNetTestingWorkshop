namespace BankServer.Exceptions
{
    public class AccountBlockedException : Exception
    {
        public AccountBlockedException(int accountId)
            : base($"Account {accountId} is blocked and can't be charged.")
        {
            AccountId = accountId;
        }

        public int AccountId { get; }
    }
}
