namespace BankServer.Notifications
{
    public interface IAccountNotifier
    {
        void NotifyBlocked(int accountId);
    }
}
