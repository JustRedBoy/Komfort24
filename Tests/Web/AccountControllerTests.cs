using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using Models;
using Web.Controllers;
using Web.Models;
using Xunit;

namespace Tests.Web
{
    public class AccountControllerTests
    {
        private static ApplicationContext CreateContextWithAccount(string accountId)
        {
            var context = new ApplicationContext();

            var accountRow = new List<object> { "1", accountId, "Тест" };
            for (int i = 3; i <= 35; i++) accountRow.Add("0");
            accountRow[6] = "";

            var rates = new List<object> { "0", "0", "0", "0", "0", "0", "0", "0", "0" };
            var house = new House("20/1", new List<IList<object>> { accountRow }, rates);

            var serviceContext = new ServiceContext();
            typeof(ServiceContext).GetProperty("Houses")
                .SetValue(serviceContext, new List<House> { house });

            var cacheField = typeof(global::SheetsEF.Models.ApplicationContextBase)
                .GetField("_cache", BindingFlags.NonPublic | BindingFlags.Instance);
            var cache = (MemoryCache)cacheField.GetValue(context);
            cache.Set<ServiceContext>("Service", serviceContext);

            return context;
        }

        [Fact]
        public void GetAccount_ValidId_ReturnsAccount()
        {
            var context = CreateContextWithAccount("1001");
            var controller = new AccountController(context);

            var result = controller.GetAccount("1001");

            Assert.NotNull(result);
            Assert.Equal("1001", result.AccountId);
        }

        [Fact]
        public void GetAccount_InvalidId_ReturnsNull()
        {
            var context = CreateContextWithAccount("1001");
            var controller = new AccountController(context);

            var result = controller.GetAccount("invalid");

            Assert.Null(result);
        }

        [Fact]
        public void GetAccount_NotFound_ReturnsNull()
        {
            var context = CreateContextWithAccount("1001");
            var controller = new AccountController(context);

            var result = controller.GetAccount("9999");

            Assert.Null(result);
        }

        [Fact]
        public void GetAccount_NullContext_ReturnsNull()
        {
            var controller = new AccountController(null);

            var result = controller.GetAccount("1001");

            Assert.Null(result);
        }
    }
}
