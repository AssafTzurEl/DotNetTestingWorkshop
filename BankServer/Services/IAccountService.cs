using BankServer.Model;

namespace BankServer.Services
{
    public interface IAccountService
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
