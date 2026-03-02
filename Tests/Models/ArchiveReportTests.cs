using System.Collections.Generic;
using Models;
using Xunit;

namespace Tests.Models
{
    public class ArchiveReportTests
    {
        private static IList<object> CreateArchiveReportData()
        {
            var data = new List<object>
            {
                "15", "1001", "Иванов",       // 0-2: flat, account, owner
                "100", "0", "50", "гкал",     // 3-6
                "10,500", "8,200", "2,300",   // 7-9
                "150", "0", "250",            // 10-12
                "50", "100",                  // 13-14
                "100", "0",                   // 15-16
                "200", "0", "50",             // 17-19
                "80",                         // 20
                "150", "140", "10",           // 21-23
                "50", "165",                  // 24-25
                "0", "365",                   // 26-27
                "100", "200",                 // 28-29
                "65", "0",                    // 30-31
                "январь", "2023"              // 32-33: month, year
            };
            return data;
        }

        [Fact]
        public void Constructor_ParsesIdentityAndPeriod()
        {
            var data = CreateArchiveReportData();
            var report = new ArchiveReport(data);

            Assert.Equal("15", report.FlatNumber);
            Assert.Equal("1001", report.AccountId);
            Assert.Equal("Иванов", report.Owner);
            Assert.Equal("январь", report.Month);
            Assert.Equal(2023, report.Year);
        }

        [Fact]
        public void Constructor_InheritsReportFields()
        {
            var data = CreateArchiveReportData();
            var report = new ArchiveReport(data);

            Assert.Equal(100.0, report.HeatingStartDebit);
            Assert.Equal("гкал", report.HeatingType);
            Assert.Equal(10.5, report.HeatingCurrentValue);
        }
    }
}
