using AwesomeAssertions;
using BankServer.Exceptions;
using BankServer.Model;
using BankServer.Repositories;
using BankServer.Services;

namespace BankServer.Tests
{
    public class AccountServiceTests
    {
        private static AccountService CreateSut() =>
            new AccountService(new InMemoryAccountRepository());

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
    }
}
