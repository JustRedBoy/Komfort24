using System.Collections.Generic;
using Models;
using Xunit;

namespace Tests.Models
{
    public class ArchiveReport2Tests
    {
        private static IList<object> CreateArchiveReport2Data()
        {
            var data = new List<object>
            {
                "15", "1001", "Иванов",       // 0-2
                "100", "0", "50", "гкал",     // 3-6
                "10,500", "8,200", "2,300",   // 7-9
                "150", "0", "250",            // 10-12
                "100", "50",                  // 13-14
                "100", "0",                   // 15-16
                "200", "0", "50",             // 17-19
                "80", "20",                   // 20-21
                "3",                          // 22: livingPersons
                "15",                         // 23: garbageForMonth
                "150", "140", "10",           // 24-26
                "50", "165",                  // 27-28
                "0", "365",                   // 29-30
                "20", "200", "100",           // 31-33: werRepair, werBank, werCash
                "65", "0",                    // 34-35: werEndDebit, werEndCredit
                "февраль", "2023"             // 36-37: month, year
            };
            return data;
        }

        [Fact]
        public void Constructor_ParsesIdentityAndPeriod()
        {
            var data = CreateArchiveReport2Data();
            var report = new ArchiveReport2(data);

            Assert.Equal("15", report.FlatNumber);
            Assert.Equal("1001", report.AccountId);
            Assert.Equal("Иванов", report.Owner);
            Assert.Equal("февраль", report.Month);
            Assert.Equal(2023, report.Year);
        }

        [Fact]
        public void Constructor_InheritsReport2Fields()
        {
            var data = CreateArchiveReport2Data();
            var report = new ArchiveReport2(data);

            Assert.Equal(100.0, report.HeatingStartDebit);
            Assert.Equal("гкал", report.HeatingType);
            Assert.Equal(3, report.LivingPersons);
            Assert.Equal(20.0, report.RepairForMonth);
        }

        [Fact]
        public void ConversionConstructor_CopiesAllFieldsFromArchiveReport()
        {
            var oldData = new List<object>
            {
                "15", "1001", "Иванов",
                "100", "5", "50", "мвт",
                "10,500", "8,200", "2,300",
                "150", "10", "250",
                "50", "100",
                "100", "5",
                "200", "10", "50",
                "80",
                "150", "140", "10",
                "50", "165",
                "20", "365",
                "100", "200",
                "65", "3",
                "март", "2022"
            };
            var oldReport = new ArchiveReport(oldData);

            var converted = new ArchiveReport2(oldReport);

            Assert.Equal("15", converted.FlatNumber);
            Assert.Equal("1001", converted.AccountId);
            Assert.Equal("Иванов", converted.Owner);
            Assert.Equal("март", converted.Month);
            Assert.Equal(2022, converted.Year);

            // Heating fields copied
            Assert.Equal(oldReport.HeatingStartDebit, converted.HeatingStartDebit);
            Assert.Equal(oldReport.HeatingStartCredit, converted.HeatingStartCredit);
            Assert.Equal(oldReport.HeatingType, converted.HeatingType);
            Assert.Equal(oldReport.HeatingCurrentValue, converted.HeatingCurrentValue);
            Assert.Equal(oldReport.HeatingEndDebit, converted.HeatingEndDebit);

            // Wer fields copied
            Assert.Equal(oldReport.WerStartDebit, converted.WerStartDebit);
            Assert.Equal(oldReport.WerEndDebit, converted.WerEndDebit);
            Assert.Equal(oldReport.WerBank, converted.WerBank);

            // New fields default to 0
            Assert.Equal(0.0, converted.RepairForMonth);
            Assert.Equal(0, converted.LivingPersons);
            Assert.Equal(0.0, converted.GarbageForMonth);
            Assert.Equal(0.0, converted.WerRepair);
        }
    }
}
