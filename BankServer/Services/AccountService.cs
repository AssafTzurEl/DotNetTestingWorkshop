using BankServer.Model;
using BankServer.Repositories;

namespace BankServer.Services
{
    public class AccountService : IAccountService
    {
        public AccountService(IAccountRepository repository)
        {
            _repository = repository;
        }

        public Account Add(Account account)
        {
            return _repository.Add(account);
        }

        public Account Get(int accountId)
        {
            return _repository.Get(accountId);
        }

        public IEnumerable<Account> GetAll()
        {
            return _repository.GetAll();
        }

        public Account Credit(int accountId, decimal amount)
        {
            return _repository.Credit(accountId, amount);
        }

        public Account Charge(int accountId, decimal amount)
        {
            return _repository.Charge(accountId, amount);
        }

        public void Delete(int accountId)
        {
            _repository.Delete(accountId);
        }

        public void DeleteAll()
        {
            _repository.DeleteAll();
        }

        private readonly IAccountRepository _repository;
    }
}
