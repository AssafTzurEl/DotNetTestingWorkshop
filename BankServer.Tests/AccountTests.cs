using BankServer.Model;

namespace BankServer.Tests
{
    public class AccountTests
    {
        [Fact]
        public void Constructor_NewAccount_HasZeroBalance()
        {
            // Arrange
            const decimal ZeroBalance = 0m;
            var sut = new Account();

            // Act – nothing to do: construction is the behavior

            // Assert
            Assert.Equal(ZeroBalance, sut.Balance);
        }

        [Fact]
        public void Credit_PositiveAmount_IncreasesBalance()
        {
            // Arrange
            const decimal Amount = 100m;
            var sut = new Account();

            // Act
            sut.Credit(Amount);

            // Assert
            Assert.Equal(Amount, sut.Balance);
        }

        [Fact]
        public void Credit_NegativeAmount_Throws()
        {
            // Arrange
            const decimal NegativeAmount = -1m;
            var sut = new Account();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Credit(NegativeAmount));
        }

        [Fact]
        public void Charge_PositiveAmount_DecreasesBalance()
        {
            // Arrange
            const decimal Amount = 100m;
            var sut = new Account();

            // Act
            sut.Charge(Amount);

            // Assert
            Assert.Equal(-Amount, sut.Balance);
        }

        [Fact]
        public void Charge_NegativeAmount_Throws()
        {
            // Arrange
            const decimal NegativeAmount = -1m;
            var sut = new Account();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Charge(NegativeAmount));
        }
    }
}
