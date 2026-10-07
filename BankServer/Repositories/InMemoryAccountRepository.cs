using System.Collections.Concurrent;
using BankServer.Exceptions;
using BankServer.Model;

namespace BankServer.Repositories
{
    public class InMemoryAccountRepository : IAccountRepository
    {
        public Account Add(Account account)
        {
            if (account.Id != 0)
            {
                throw new ArgumentException("A new account must not have an ID.", nameof(account));
            }

            account.Id = Interlocked.Increment(ref _lastAccountId);
            _accounts[account.Id] = account;

            return account;
        }

        public Account Get(int accountId)
        {
            if (!_accounts.TryGetValue(accountId, out var account))
            {
                throw new EntityNotFoundException(accountId);
            }

            return account;
        }

        public IEnumerable<Account> GetAll()
        {
            return _accounts.Values.OrderBy(account => account.Id).ToList();
        }

        public Account Credit(int accountId, decimal amount)
        {
            var account = Get(accountId);
            account.Credit(amount);

            return account;
        }

        public Account Charge(int accountId, decimal amount)
        {
            var account = Get(accountId);
            account.Charge(amount);

            return account;
        }

        public void Delete(int accountId)
        {
            if (!_accounts.TryRemove(accountId, out _))
            {
                throw new EntityNotFoundException(accountId);
            }
        }

        public void DeleteAll()
        {
            _accounts.Clear();
        }

        public void Dispose()
        {
            // Nothing to release in memory. A database implementation would close its connection here.
        }

        private readonly ConcurrentDictionary<int, Account> _accounts = new();
        private int _lastAccountId;
    }
}
