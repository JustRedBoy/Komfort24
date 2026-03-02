using System.Collections.Generic;
using Models;
using Xunit;

namespace Tests.Models
{
    public class RatesTests
    {
        [Fact]
        public void Constructor_ParsesAllRatesCorrectly()
        {
            IList<object> data = new List<object>
            {
                "10,500", "20,300", "15,750", "5,100", "3,200",
                "25,000", "30,000", "8,500", "12,750"
            };

            var rates = new Rates(data);

            Assert.Equal(10.5, rates.WaterRate);
            Assert.Equal(20.3, rates.CentralHeatingRate);
            Assert.Equal(15.75, rates.CustomHeatingRate);
            Assert.Equal(5.1, rates.GeneralWerRate);
            Assert.Equal(3.2, rates.SpecialWerRate);
            Assert.Equal(25.0, rates.CentralHeatingForAllRate);
            Assert.Equal(30.0, rates.CentralHeatingForSomeRate);
            Assert.Equal(8.5, rates.GarbageRate);
            Assert.Equal(12.75, rates.RepairRate);
        }

        [Fact]
        public void Constructor_ZeroValues_ParsesAsZero()
        {
            IList<object> data = new List<object>
            {
                "0", "0", "0", "0", "0", "0", "0", "0", "0"
            };

            var rates = new Rates(data);

            Assert.Equal(0.0, rates.WaterRate);
            Assert.Equal(0.0, rates.CentralHeatingRate);
            Assert.Equal(0.0, rates.GarbageRate);
        }
    }
}
