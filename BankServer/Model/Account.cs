using BankServer.Exceptions;

namespace BankServer.Model
{
    public class Account
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public decimal Balance => _balance;

        /// <summary>
        /// A blocked account can't be charged. Crediting is still allowed –
        /// that's how the owner gets unblocked.
        /// </summary>
        public bool IsBlocked => _balance <= BlockingThreshold;

        public decimal Credit(decimal amount)
        {
            ValidateAmount(amount);

            lock (_lock)
            {
                _balance += amount;

                return _balance;
            }
        }

        public decimal Charge(decimal amount)
        {
            ValidateAmount(amount);

            lock (_lock)
            {
                if (IsBlocked)
                {
                    throw new AccountBlockedException(Id);
                }

                _balance += amount;

                return _balance;
            }
        }

        private static void ValidateAmount(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Amount must be positive.");
            }
        }

        private const decimal BlockingThreshold = -5000m;

        private readonly object _lock = new();
        private decimal _balance;
    }
}
