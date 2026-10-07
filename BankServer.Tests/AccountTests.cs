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

        [Theory]
        [InlineData("Assaf", "Assaf")]
        [InlineData("  Assaf", "Assaf")]
        [InlineData("Assaf  ", "Assaf")]
        [InlineData(" Tzur-El ", "Tzur-El")]
        [InlineData("Tzur El", "Tzur El")]
        public void Name_Set_StoresTrimmedValue(string input, string expected)
        {
            // Arrange
            var sut = new Account();

            // Act
            sut.Name = input;

            // Assert
            Assert.Equal(expected, sut.Name);
        }

        public static TheoryData<decimal> PositiveAmounts => new()
        {
            0.01m, 100m, 1_000_000_000m
        };

        [Theory]
        [MemberData(nameof(PositiveAmounts))]
        public void Credit_PositiveAmount_IncreasesBalance(decimal amount)
        {
            // Arrange
            var sut = new Account();

            // Act
            sut.Credit(amount);

            // Assert
            Assert.Equal(amount, sut.Balance);
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

        public static TheoryData<decimal> NonPositiveAmounts => new()
        {
            0m, -0.01m, -100m
        };

        [Theory]
        [MemberData(nameof(NonPositiveAmounts))]
        public void Credit_NonPositiveAmount_Throws(decimal amount)
        {
            // Arrange
            var sut = new Account();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Credit(amount));
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

        // Nobody defined what Credit should do when the balance would overflow.
        // decimal arithmetic throws OverflowException, so that's what Credit does today.
        // This test pins down the current behavior until someone decides otherwise.
        [Fact]
        public void Credit_BeyondMaxValue_ThrowsOverflowException()
        {
            // Arrange
            var sut = new Account();
            sut.Credit(decimal.MaxValue);

            // Act & Assert
            Assert.Throws<OverflowException>(() => sut.Credit(decimal.MaxValue));
        }

        // For the demo: the expectation here is deliberately wrong, and Assert.True
        // can't tell you that. Run it and read the failure message.
        [Fact]
        public void Credit_TwoAmounts_BalanceIsTheirSum()
        {
            // Arrange
            const decimal FirstAmount = 100m;
            const decimal SecondAmount = 50m;
            const decimal ExpectedBalance = 200m;
            var sut = new Account();

            // Act
            sut.Credit(FirstAmount);
            sut.Credit(SecondAmount);

            // Assert
            Assert.True(sut.Balance == ExpectedBalance);
        }
    }
}
