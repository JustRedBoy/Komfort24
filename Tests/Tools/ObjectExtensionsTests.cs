using Xunit;
using Tools.Extensions;

namespace Tests.Tools
{
    public class ObjectExtensionsTests
    {
        [Theory]
        [InlineData("1,5", 2, 1.5)]
        [InlineData("100,123", 3, 100.123)]
        [InlineData("0", 2, 0.0)]
        [InlineData("1 000,50", 2, 1000.5)]
        [InlineData("  ", 2, 0.0)]
        [InlineData("", 2, 0.0)]
        public void ToDouble_VariousInputs_ReturnsExpected(string input, int digits, double expected)
        {
            Assert.Equal(expected, ((object)input).ToDouble(digits));
        }

        [Fact]
        public void ToDouble_DefaultPrecision_RoundsToTwoDigits()
        {
            Assert.Equal(1.23, ((object)"1,234").ToDouble());
        }

        [Fact]
        public void ToDouble_ThreeDigitPrecision_RoundsToThreeDigits()
        {
            // Math.Round uses banker's rounding by default: 1.2345 -> 1.234
            Assert.Equal(1.234, ((object)"1,2345").ToDouble(3));
        }

        [Theory]
        [InlineData("5", 5)]
        [InlineData("100", 100)]
        [InlineData("0", 0)]
        [InlineData(" 42 ", 42)]
        [InlineData("  ", 0)]
        [InlineData("", 0)]
        public void ToInt_VariousInputs_ReturnsExpected(string input, int expected)
        {
            Assert.Equal(expected, ((object)input).ToInt());
        }

        [Fact]
        public void ToDouble_WithSpacesInNumber_ParsesCorrectly()
        {
            Assert.Equal(12345.68, ((object)"12 345,678").ToDouble());
        }
    }
}
