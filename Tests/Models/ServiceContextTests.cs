using System.Collections.Generic;
using Models;
using Xunit;

namespace Tests.Models
{
    public class ServiceContextTests
    {
        private static ServiceContext CreateContextWithAccounts()
        {
            var context = new ServiceContext();
            // Use reflection to set Houses since InitContextAsync requires GoogleSheets
            var housesField = typeof(ServiceContext).GetProperty("Houses");

            var accountRow = new List<object> { "1", "1001", "Тест" };
            for (int i = 3; i <= 35; i++) accountRow.Add("0");
            accountRow[6] = "";

            var rates = new List<object> { "0", "0", "0", "0", "0", "0", "0", "0", "0" };
            var house = new House("20/1", new List<IList<object>> { accountRow }, rates);

            housesField.SetValue(context, new List<House> { house });
            return context;
        }

        [Fact]
        public void GetAccountById_ValidId_ReturnsAccount()
        {
            var context = CreateContextWithAccounts();

            var account = context.GetAccountById("1001");

            Assert.NotNull(account);
            Assert.Equal("1001", account.AccountId);
        }

        [Fact]
        public void GetAccountById_InvalidIdFormat_ReturnsNull()
        {
            var context = CreateContextWithAccounts();

            var account = context.GetAccountById("invalid");

            Assert.Null(account);
        }

        [Fact]
        public void GetAccountById_ValidIdNotFound_ReturnsNull()
        {
            var context = CreateContextWithAccounts();

            var account = context.GetAccountById("9999");

            Assert.Null(account);
        }

        [Fact]
        public void TotalHouses_ReturnsHousesCount()
        {
            var context = CreateContextWithAccounts();

            Assert.Equal(1, context.TotalHouses);
        }

        [Fact]
        public void TotalAccounts_ReturnsSumOfAllAccounts()
        {
            var context = CreateContextWithAccounts();

            Assert.Equal(1, context.TotalAccounts);
        }
    }
}
