using BankServer.Model;
using BankServer.Notifications;
using BankServer.Repositories;

namespace BankServer.Services
{
    public class AccountService : IAccountService
    {
        public AccountService(IAccountRepository repository, IAccountNotifier notifier)
        {
            _repository = repository;
            _notifier = notifier;
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
            var account = _repository.Charge(accountId, amount); // throws if already blocked

            if (account.IsBlocked)
            {
                // Blocked after a successful charge: this charge is what blocked it
                _notifier.NotifyBlocked(account.Id);
            }

            return account;
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
        private readonly IAccountNotifier _notifier;
    }
}
