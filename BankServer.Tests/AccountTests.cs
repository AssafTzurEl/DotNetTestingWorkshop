using AwesomeAssertions;
using BankServer.Exceptions;
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
            sut.Balance.Should().Be(ZeroBalance);
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
            sut.Name.Should().Be(expected);
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
            sut.Balance.Should().Be(amount);
        }

        [Fact]
        public void Credit_NegativeAmount_Throws()
        {
            // Arrange
            const decimal NegativeAmount = -1m;
            var sut = new Account();

            // Act
            Action act = () => sut.Credit(NegativeAmount);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
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

            // Act
            Action act = () => sut.Credit(amount);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
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
            sut.Balance.Should().Be(-Amount);
        }

        [Fact]
        public void Charge_NegativeAmount_Throws()
        {
            // Arrange
            const decimal NegativeAmount = -1m;
            var sut = new Account();

            // Act
            Action act = () => sut.Charge(NegativeAmount);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Charge_BlockedAccount_Throws()
        {
            // Arrange
            const decimal AmountBeyondThreshold = 5_000.01m;
            const decimal Amount = 1m;
            var sut = new Account();
            sut.Charge(AmountBeyondThreshold);

            // Act
            Action act = () => sut.Charge(Amount);

            // Assert
            act.Should().Throw<AccountBlockedException>();
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

            // Act
            Action act = () => sut.Credit(decimal.MaxValue);

            // Assert
            act.Should().Throw<OverflowException>();
        }

        [Fact]
        public void Credit_TwoAmounts_BalanceIsTheirSum()
        {
            // Arrange
            const decimal FirstAmount = 100m;
            const decimal SecondAmount = 50m;
            const decimal ExpectedBalance = 150m;
            var sut = new Account();

            // Act
            sut.Credit(FirstAmount);
            sut.Credit(SecondAmount);

            // Assert
            sut.Balance.Should().Be(ExpectedBalance);
        }
    }
}
