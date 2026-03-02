using System.Collections.Generic;
using Models;
using Xunit;

namespace Tests.Models
{
    public class AccountTests
    {
        private static IList<object> CreateAccountData(string flat = "15", string accountId = "1001", string owner = "Иванов И.И.")
        {
            var data = new List<object> { flat, accountId, owner };
            // Add 33 more columns for Report2 (indices 3-35)
            for (int i = 3; i <= 35; i++)
            {
                data.Add("0");
            }
            // Set HeatingType (index 6) to valid string
            data[6] = "";
            return data;
        }

        [Fact]
        public void Constructor_ParsesFlatNumberAccountIdOwner()
        {
            var rates = new Rates(new List<object> { "0", "0", "0", "0", "0", "0", "0", "0", "0" });
            var houseAccounts = new List<IList<object>> { CreateAccountData() };
            var house = new House("20/1", houseAccounts, new List<object> { "0", "0", "0", "0", "0", "0", "0", "0", "0" });
            var account = house.Accounts[0];

            Assert.Equal("15", account.FlatNumber);
            Assert.Equal("1001", account.AccountId);
            Assert.Equal("Иванов И.И.", account.Owner);
            Assert.Same(house, account.House);
            Assert.NotNull(account.CurrentReport);
        }
    }
}
