using System.Collections.Generic;
using Desktop.Extensions;
using Models;
using Xunit;

namespace Tests.Desktop
{
    public class AccountExtensionsTests
    {
        [Fact]
        public void GetObjects_ReturnsCorrectStructure()
        {
            var accountRow = new List<object> { "15", "1001", "Иванов" };
            for (int i = 3; i <= 35; i++) accountRow.Add("0");
            accountRow[6] = "";

            var rates = new List<object> { "0", "0", "0", "0", "0", "0", "0", "0", "0" };
            var house = new House("20/1", new List<IList<object>> { accountRow }, rates);
            var account = house.Accounts[0];

            var objects = account.GetObjects();

            // Structure: flat(0), accountId(1), owner(2), heating(14 items), wer(19 items), month, year
            Assert.Equal("15", objects[0]);
            Assert.Equal("1001", objects[1]);
            Assert.Equal("Иванов", objects[2]);
            // Total: 3 + 14 + 19 + 2 = 38 items
            Assert.Equal(38, objects.Count);
        }

        [Fact]
        public void GetObjects_LastTwoItems_AreMonthAndYear()
        {
            var accountRow = new List<object> { "15", "1001", "Иванов" };
            for (int i = 3; i <= 35; i++) accountRow.Add("0");
            accountRow[6] = "";

            var rates = new List<object> { "0", "0", "0", "0", "0", "0", "0", "0", "0" };
            var house = new House("20/1", new List<IList<object>> { accountRow }, rates);
            var account = house.Accounts[0];

            var objects = account.GetObjects();

            // Last two items: month name and year
            var monthName = objects[objects.Count - 2].ToString();
            var year = (int)objects[objects.Count - 1];

            Assert.False(string.IsNullOrEmpty(monthName));
            Assert.True(year >= 2020);
        }
    }
}
