using AwesomeAssertions;
using BankServer.Exceptions;
using BankServer.Model;
using BankServer.Notifications;
using BankServer.Repositories;
using BankServer.Services;
using FakeItEasy;

namespace BankServer.Tests
{
    public class AccountServiceTests
    {
        private static AccountService CreateSut(
            IAccountRepository? repository = null,
            IAccountNotifier? notifier = null) =>
            new AccountService(
                repository ?? new InMemoryAccountRepository(),
                notifier ?? A.Fake<IAccountNotifier>());

        [Fact]
        public void Add_NewAccount_AssignsId()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var account = sut.Add(new Account());

            // Assert
            account.Id.Should().BePositive();
        }

        [Fact]
        public void Get_MissingAccount_Throws()
        {
            // Arrange
            const int MissingAccountId = 1;
            var sut = CreateSut();

            // Act
            Action act = () => sut.Get(MissingAccountId);

            // Assert
            act.Should().Throw<EntityNotFoundException>();
        }

        [Fact]
        public void Credit_ExistingAccount_IncreasesBalance()
        {
            // Arrange
            const decimal Amount = 100m;
            var sut = CreateSut();
            var account = sut.Add(new Account());

            // Act
            sut.Credit(account.Id, Amount);

            // Assert
            sut.Get(account.Id).Balance.Should().Be(Amount);
        }

        [Fact]
        public void Charge_ExistingAccount_DecreasesBalance()
        {
            // Arrange
            const decimal Amount = 100m;
            var sut = CreateSut();
            var account = sut.Add(new Account());

            // Act
            sut.Charge(account.Id, Amount);

            // Assert
            sut.Get(account.Id).Balance.Should().Be(-Amount);
        }

        [Fact]
        public void Delete_ExistingAccount_RemovesIt()
        {
            // Arrange
            var sut = CreateSut();
            var account = sut.Add(new Account());

            // Act
            sut.Delete(account.Id);

            // Assert
            Action act = () => sut.Get(account.Id);
            act.Should().Throw<EntityNotFoundException>();
        }

        [Fact]
        public void Charge_CrossesBlockingThreshold_NotifiesOnce()
        {
            // Arrange
            const decimal AmountBeyondThreshold = 5_000.01m;
            var notifier = A.Fake<IAccountNotifier>();
            var sut = CreateSut(notifier: notifier);
            var account = sut.Add(new Account());

            // Act
            sut.Charge(account.Id, AmountBeyondThreshold);

            // Assert
            A.CallTo(() => notifier.NotifyBlocked(account.Id))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Charge_DownToThresholdExactly_DoesNotNotify()
        {
            // Arrange
            const decimal AmountToThreshold = 5_000m;
            var notifier = A.Fake<IAccountNotifier>();
            var sut = CreateSut(notifier: notifier);
            var account = sut.Add(new Account());

            // Act
            sut.Charge(account.Id, AmountToThreshold);

            // Assert
            A.CallTo(() => notifier.NotifyBlocked(A<int>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public void Charge_RepositoryFails_DoesNotNotify()
        {
            // Arrange
            const int AccountId = 1;
            const decimal Amount = 100m;
            var repository = A.Fake<IAccountRepository>();
            A.CallTo(() => repository.Charge(A<int>._, A<decimal>._))
                .Throws(new TimeoutException());
            var notifier = A.Fake<IAccountNotifier>();
            var sut = CreateSut(repository, notifier);

            // Act
            Action act = () => sut.Charge(AccountId, Amount);

            // Assert
            act.Should().Throw<TimeoutException>();
            A.CallTo(() => notifier.NotifyBlocked(A<int>._))
                .MustNotHaveHappened();
        }
    }
}
