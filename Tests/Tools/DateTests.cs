using System;
using Tools;
using Xunit;

namespace Tests.Tools
{
    public class DateTests
    {
        [Theory]
        [InlineData(1, "январь")]
        [InlineData(2, "февраль")]
        [InlineData(3, "март")]
        [InlineData(4, "апрель")]
        [InlineData(5, "май")]
        [InlineData(6, "июнь")]
        [InlineData(7, "июль")]
        [InlineData(8, "август")]
        [InlineData(9, "сентябрь")]
        [InlineData(10, "октябрь")]
        [InlineData(11, "ноябрь")]
        [InlineData(12, "декабрь")]
        public void GetNameMonth_ValidMonth_ReturnsCorrectName(int monthNum, string expected)
        {
            Assert.Equal(expected, Date.GetNameMonth(monthNum));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(13)]
        [InlineData(-1)]
        public void GetNameMonth_InvalidMonth_ThrowsArgumentException(int monthNum)
        {
            Assert.Throws<ArgumentException>(() => Date.GetNameMonth(monthNum));
        }

        [Theory]
        [InlineData("январь", 1)]
        [InlineData("февраль", 2)]
        [InlineData("март", 3)]
        [InlineData("апрель", 4)]
        [InlineData("май", 5)]
        [InlineData("июнь", 6)]
        [InlineData("июль", 7)]
        [InlineData("август", 8)]
        [InlineData("сентябрь", 9)]
        [InlineData("октябрь", 10)]
        [InlineData("ноябрь", 11)]
        [InlineData("декабрь", 12)]
        public void GetNumMonth_ValidName_ReturnsCorrectNumber(string monthName, int expected)
        {
            Assert.Equal(expected, Date.GetNumMonth(monthName));
        }

        [Theory]
        [InlineData("Январь")]
        [InlineData("ЯНВАРЬ")]
        [InlineData("invalid")]
        [InlineData("")]
        public void GetNumMonth_InvalidName_ThrowsArgumentException(string monthName)
        {
            Assert.Throws<ArgumentException>(() => Date.GetNumMonth(monthName));
        }

        [Theory]
        [InlineData(1, "01")]
        [InlineData(9, "09")]
        [InlineData(10, "10")]
        [InlineData(12, "12")]
        public void GetShortMonth_ValidMonth_ReturnsZeroPadded(int numMonth, string expected)
        {
            Assert.Equal(expected, Date.GetShortMonth(numMonth));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(13)]
        public void GetShortMonth_InvalidMonth_ThrowsArgumentException(int numMonth)
        {
            Assert.Throws<ArgumentException>(() => Date.GetShortMonth(numMonth));
        }

        [Theory]
        [InlineData(2022, "22")]
        [InlineData(2023, "23")]
        [InlineData(2000, "00")]
        [InlineData(1999, "99")]
        public void GetShortYear_ReturnsLastTwoDigits(int year, string expected)
        {
            Assert.Equal(expected, Date.GetShortYear(year));
        }

        [Fact]
        public void GetNameCurMonth_ReturnsCurrentMonthName()
        {
            var expected = Date.GetNameMonth(DateTime.Now.Month);
            Assert.Equal(expected, Date.GetNameCurMonth());
        }

        [Fact]
        public void GetNamePrevMonth_ReturnsPreviousMonthName()
        {
            var expected = Date.GetNameMonth(DateTime.Now.AddMonths(-1).Month);
            Assert.Equal(expected, Date.GetNamePrevMonth());
        }

        [Fact]
        public void GetFullPrevMonth_ReturnsNameAndYear()
        {
            var prevDate = DateTime.Now.AddMonths(-1);
            var expected = $"{Date.GetNameMonth(prevDate.Month)} {prevDate.Year}";
            Assert.Equal(expected, Date.GetFullPrevMonth());
        }
    }
}
