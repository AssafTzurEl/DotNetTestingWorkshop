using BankServer.Model;

namespace BankServer.Repositories
{
    /// <summary>
    /// Stands for a database: disposable, because a real one holds a connection.
    /// </summary>
    public interface IAccountRepository : IDisposable
    {
        Account Add(Account account);
        Account Get(int accountId);
        IEnumerable<Account> GetAll();
        Account Credit(int accountId, decimal amount);
        Account Charge(int accountId, decimal amount);
        void Delete(int accountId);
        void DeleteAll();
    }
}
