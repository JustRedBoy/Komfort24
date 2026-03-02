using System.Collections.Generic;
using Models;
using Xunit;

namespace Tests.Models
{
    public class Report2Tests
    {
        private static IList<object> CreateReport2Data(
            string flatNumber = "1", string accountId = "1001", string owner = "Иванов",
            double heatingStartDebit = 100.0, double heatingStartCredit = 0.0,
            double heatingSquare = 50.0, string heatingType = "гкал",
            double heatingCurrentValue = 10.5, double heatingPreviousValue = 8.2,
            double heatingValue = 2.3, double heatingForService = 150.0,
            double heatingPreviliges = 0.0, double heatingTotal = 250.0,
            double heatingBank = 100.0, double heatingCash = 50.0,
            double heatingEndDebit = 100.0, double heatingEndCredit = 0.0,
            double werStartDebit = 200.0, double werStartCredit = 0.0,
            double werSquare = 50.0, double werForMonth = 80.0,
            double repairForMonth = 20.0, int livingPersons = 3,
            double garbageForMonth = 15.0, int waterCurrentValue = 150,
            int waterPreviousValue = 140, int waterValue = 10,
            double waterForMonth = 50.0, double werWaterForService = 165.0,
            double werPreviliges = 0.0, double werTotal = 365.0,
            double werRepair = 20.0, double werBank = 200.0,
            double werCash = 100.0, double werEndDebit = 65.0,
            double werEndCredit = 0.0)
        {
            return new List<object>
            {
                flatNumber, accountId, owner,
                heatingStartDebit.ToString().Replace('.', ','),
                heatingStartCredit.ToString().Replace('.', ','),
                heatingSquare.ToString().Replace('.', ','),
                heatingType,
                heatingCurrentValue.ToString().Replace('.', ','),
                heatingPreviousValue.ToString().Replace('.', ','),
                heatingValue.ToString().Replace('.', ','),
                heatingForService.ToString().Replace('.', ','),
                heatingPreviliges.ToString().Replace('.', ','),
                heatingTotal.ToString().Replace('.', ','),
                heatingBank.ToString().Replace('.', ','),
                heatingCash.ToString().Replace('.', ','),
                heatingEndDebit.ToString().Replace('.', ','),
                heatingEndCredit.ToString().Replace('.', ','),
                werStartDebit.ToString().Replace('.', ','),
                werStartCredit.ToString().Replace('.', ','),
                werSquare.ToString().Replace('.', ','),
                werForMonth.ToString().Replace('.', ','),
                repairForMonth.ToString().Replace('.', ','),
                livingPersons.ToString(),
                garbageForMonth.ToString().Replace('.', ','),
                waterCurrentValue.ToString(),
                waterPreviousValue.ToString(),
                waterValue.ToString(),
                waterForMonth.ToString().Replace('.', ','),
                werWaterForService.ToString().Replace('.', ','),
                werPreviliges.ToString().Replace('.', ','),
                werTotal.ToString().Replace('.', ','),
                werRepair.ToString().Replace('.', ','),
                werBank.ToString().Replace('.', ','),
                werCash.ToString().Replace('.', ','),
                werEndDebit.ToString().Replace('.', ','),
                werEndCredit.ToString().Replace('.', ',')
            };
        }

        [Fact]
        public void Constructor_ParsesHeatingFieldsCorrectly()
        {
            var data = CreateReport2Data();
            var report = new Report2(data);

            Assert.Equal(100.0, report.HeatingStartDebit);
            Assert.Equal(0.0, report.HeatingStartCredit);
            Assert.Equal(50.0, report.HeatingSquare);
            Assert.Equal("гкал", report.HeatingType);
            Assert.Equal(10.5, report.HeatingCurrentValue);
            Assert.Equal(8.2, report.HeatingPreviousValue);
            Assert.Equal(2.3, report.HeatingValue);
            Assert.Equal(150.0, report.HeatingForService);
            Assert.Equal(0.0, report.HeatingPreviliges);
            Assert.Equal(250.0, report.HeatingTotal);
            Assert.Equal(100.0, report.HeatingBank);
            Assert.Equal(50.0, report.HeatingCash);
            Assert.Equal(100.0, report.HeatingEndDebit);
            Assert.Equal(0.0, report.HeatingEndCredit);
        }

        [Fact]
        public void Constructor_ParsesWerFieldsCorrectly()
        {
            var data = CreateReport2Data();
            var report = new Report2(data);

            Assert.Equal(200.0, report.WerStartDebit);
            Assert.Equal(0.0, report.WerStartCredit);
            Assert.Equal(50.0, report.WerSquare);
            Assert.Equal(80.0, report.WerForMonth);
            Assert.Equal(20.0, report.RepairForMonth);
            Assert.Equal(3, report.LivingPersons);
            Assert.Equal(15.0, report.GarbageForMonth);
            Assert.Equal(150, report.WaterCurrentValue);
            Assert.Equal(140, report.WaterPreviousValue);
            Assert.Equal(10, report.WaterValue);
            Assert.Equal(50.0, report.WaterForMonth);
            Assert.Equal(165.0, report.WerWaterForService);
            Assert.Equal(0.0, report.WerPreviliges);
            Assert.Equal(365.0, report.WerTotal);
            Assert.Equal(20.0, report.WerRepair);
            Assert.Equal(200.0, report.WerBank);
            Assert.Equal(100.0, report.WerCash);
            Assert.Equal(65.0, report.WerEndDebit);
            Assert.Equal(0.0, report.WerEndCredit);
        }

        [Fact]
        public void DefaultConstructor_CreatesInstanceWithDefaults()
        {
            var report = new Report2();

            Assert.Equal(0.0, report.HeatingStartDebit);
            Assert.Null(report.HeatingType);
        }
    }
}
