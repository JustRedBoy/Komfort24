using System.Collections.Generic;
using Models;
using Xunit;

namespace Tests.Models
{
    public class HouseTests
    {
        private static IList<object> CreateAccountRow(string flat, string accountId, string owner = "Тест")
        {
            var data = new List<object> { flat, accountId, owner };
            for (int i = 3; i <= 35; i++)
                data.Add("0");
            data[6] = "";
            return data;
        }

        private static IList<object> ZeroRates()
        {
            return new List<object> { "0", "0", "0", "0", "0", "0", "0", "0", "0" };
        }

        [Fact]
        public void Constructor_SetsShortAndFullAddress()
        {
            var house = new House("20/1", new List<IList<object>>(), ZeroRates());

            Assert.Equal("20/1", house.ShortAdress);
            Assert.Equal("Пишоновская, 20/1", house.FullAdress);
        }

        [Fact]
        public void Constructor_FiltersInvalidAccountIds()
        {
            var accounts = new List<IList<object>>
            {
                CreateAccountRow("1", "1001"),
                CreateAccountRow("2", "invalid"),
                CreateAccountRow("3", "1002"),
                CreateAccountRow("4", "abc"),
            };

            var house = new House("20/1", accounts, ZeroRates());

            Assert.Equal(2, house.FlatCount);
            Assert.Equal(2, house.Accounts.Count);
            Assert.Equal("1001", house.Accounts[0].AccountId);
            Assert.Equal("1002", house.Accounts[1].AccountId);
        }

        [Fact]
        public void Constructor_EmptyAccounts_CreatesEmptyList()
        {
            var house = new House("24A", new List<IList<object>>(), ZeroRates());

            Assert.Empty(house.Accounts);
            Assert.Equal(0, house.FlatCount);
        }

        [Fact]
        public void Constructor_ParsesRates()
        {
            var rates = new List<object> { "1,5", "2,3", "3,1", "4,0", "5,0", "6,0", "7,0", "8,0", "9,0" };
            var house = new House("20/1", new List<IList<object>>(), rates);

            Assert.Equal(1.5, house.Rates.WaterRate);
            Assert.Equal(2.3, house.Rates.CentralHeatingRate);
        }

        [Fact]
        public void Constructor_AccountWithSlash_IsValid()
        {
            var accounts = new List<IList<object>>
            {
                CreateAccountRow("1", "1001/1"),
                CreateAccountRow("2", "1001/2"),
            };

            var house = new House("20/1", accounts, ZeroRates());

            Assert.Equal(2, house.FlatCount);
        }
    }
}
