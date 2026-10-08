namespace BankServer.Notifications
{
    /// <summary>
    /// Stands for a real notification channel (email, SMS). Here it only writes to the log.
    /// </summary>
    public class LoggingAccountNotifier : IAccountNotifier
    {
        public LoggingAccountNotifier(ILogger<LoggingAccountNotifier> logger)
        {
            _logger = logger;
        }

        public void NotifyBlocked(int accountId)
        {
            _logger.LogWarning("Account {AccountId} is blocked.", accountId);
        }

        private readonly ILogger<LoggingAccountNotifier> _logger;
    }
}
