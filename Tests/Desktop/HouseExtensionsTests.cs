using System.Collections.Generic;
using Desktop.Extensions;
using Models;
using Xunit;

namespace Tests.Desktop
{
    public class HouseExtensionsTests
    {
        private static IList<object> CreateAccountRow(string flat, string accountId)
        {
            var data = new List<object> { flat, accountId, "Тест" };
            for (int i = 3; i <= 35; i++) data.Add("0");
            data[6] = "";
            return data;
        }

        private static House CreateHouseWithAccounts(params (string flat, string accountId)[] accounts)
        {
            var rows = new List<IList<object>>();
            foreach (var (flat, accountId) in accounts)
                rows.Add(CreateAccountRow(flat, accountId));

            var rates = new List<object> { "0", "0", "0", "0", "0", "0", "0", "0", "0" };
            return new House("20/1", rows, rates);
        }

        [Fact]
        public void GetObjects_SkipsAccount7695()
        {
            var house = CreateHouseWithAccounts(
                ("1", "1001"),
                ("2", "7695"),
                ("3", "1002")
            );

            var objects = house.GetObjects();

            Assert.Equal(2, objects.Count);
            // Verify 7695 is not in the results
            foreach (var row in objects)
            {
                Assert.NotEqual("7695", row[1].ToString());
            }
        }

        [Fact]
        public void GetObjects_AllAccountsIncluded_WhenNo7695()
        {
            var house = CreateHouseWithAccounts(
                ("1", "1001"),
                ("2", "1002"),
                ("3", "1003")
            );

            var objects = house.GetObjects();

            Assert.Equal(3, objects.Count);
        }

        [Fact]
        public void GetObjects_EmptyHouse_ReturnsEmptyList()
        {
            var rates = new List<object> { "0", "0", "0", "0", "0", "0", "0", "0", "0" };
            var house = new House("20/1", new List<IList<object>>(), rates);

            var objects = house.GetObjects();

            Assert.Empty(objects);
        }
    }
}
