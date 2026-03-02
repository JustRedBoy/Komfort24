using System.Collections.Generic;
using Models;
using Xunit;

namespace Tests.Models
{
    public class ReportTests
    {
        private static IList<object> CreateReportData()
        {
            return new List<object>
            {
                "1", "1001", "Иванов",       // 0-2: flat, account, owner
                "100", "0", "50", "гкал",     // 3-6: heating start debit/credit, square, type
                "10,500", "8,200", "2,300",   // 7-9: heating current/previous/value (3 dp)
                "150", "0", "250",            // 10-12: heating forService, previliges, total
                "50", "100",                  // 13-14: heating cash, bank
                "100", "0",                   // 15-16: heating endDebit, endCredit
                "200", "0", "50",             // 17-19: wer start debit/credit, square
                "80",                         // 20: werForMonth
                "150", "140", "10",           // 21-23: water current/previous/value
                "50", "165",                  // 24-25: waterForMonth, werWaterForService
                "0", "365",                   // 26-27: werPreviliges, werTotal
                "100", "200",                 // 28-29: werCash, werBank
                "65", "0"                     // 30-31: werEndDebit, werEndCredit
            };
        }

        [Fact]
        public void Constructor_ParsesHeatingFieldsCorrectly()
        {
            var data = CreateReportData();
            var report = new Report(data);

            Assert.Equal(100.0, report.HeatingStartDebit);
            Assert.Equal(0.0, report.HeatingStartCredit);
            Assert.Equal(50.0, report.HeatingSquare);
            Assert.Equal("гкал", report.HeatingType);
            Assert.Equal(10.5, report.HeatingCurrentValue);
            Assert.Equal(8.2, report.HeatingPreviousValue);
            Assert.Equal(2.3, report.HeatingValue);
        }

        [Fact]
        public void Constructor_ParsesWerFieldsCorrectly()
        {
            var data = CreateReportData();
            var report = new Report(data);

            Assert.Equal(200.0, report.WerStartDebit);
            Assert.Equal(50.0, report.WerSquare);
            Assert.Equal(80.0, report.WerForMonth);
            Assert.Equal(150, report.WaterCurrentValue);
            Assert.Equal(140, report.WaterPreviousValue);
            Assert.Equal(10, report.WaterValue);
            Assert.Equal(65.0, report.WerEndDebit);
            Assert.Equal(0.0, report.WerEndCredit);
        }
    }
}
