using Tools;
using Xunit;

namespace Tests.Tools
{
    public class MatchingTests
    {
        [Theory]
        [InlineData("1234", true)]
        [InlineData("0000", true)]
        [InlineData("9999", true)]
        [InlineData("1234/1", true)]
        [InlineData("1234/2", true)]
        [InlineData("7695", true)]
        public void IsAccountId_ValidIds_ReturnsTrue(string accountId, bool expected)
        {
            Assert.Equal(expected, Matching.IsAccountId(accountId));
        }

        [Theory]
        [InlineData("123")]
        [InlineData("12345a")]
        [InlineData("abcd")]
        [InlineData("1234/3")]
        [InlineData("1234/0")]
        [InlineData("")]
        [InlineData("12/34")]
        public void IsAccountId_InvalidIds_ReturnsFalse(string accountId)
        {
            Assert.False(Matching.IsAccountId(accountId));
        }
    }
}
