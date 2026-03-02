using Desktop.Extensions;
using Models;
using System;
using Xunit;

namespace Tests.Desktop
{
    public class Report2ExtensionsTests
    {
        private static Report2 CreateReport()
        {
            return new Report2
            {
                HeatingEndDebit = 150.456,
                HeatingEndCredit = 10.789,
                HeatingCurrentValue = 25.1234,
                HeatingPreviliges = 5.0,
                HeatingCash = 30.0,
                HeatingBank = 20.0,
                WerEndDebit = 200.567,
                WerEndCredit = 15.891,
                WaterCurrentValue = 300,
                WerPreviliges = 10.0,
                WerRepair = 5.0,
                WerCash = 40.0,
                WerBank = 60.0
            };
        }

        [Fact]
        public void TransitionToNewMonth_SetsHeatingStartFromEnd()
        {
            var report = CreateReport();
            report.TransitionToNewMonth();

            Assert.Equal(Math.Round(150.456, 2), report.HeatingStartDebit);
            Assert.Equal(Math.Round(10.789, 2), report.HeatingStartCredit);
        }

        [Fact]
        public void TransitionToNewMonth_SetsPreviousValueFromCurrent()
        {
            var report = CreateReport();
            report.TransitionToNewMonth();

            Assert.Equal(Math.Round(25.1234, 3), report.HeatingPreviousValue);
        }

        [Fact]
        public void TransitionToNewMonth_ClearsPaymentsAndPreviliges()
        {
            var report = CreateReport();
            report.TransitionToNewMonth();

            Assert.Equal(0.0, report.HeatingPreviliges);
            Assert.Equal(0.0, report.HeatingCash);
            Assert.Equal(0.0, report.HeatingBank);
            Assert.Equal(0.0, report.WerPreviliges);
            Assert.Equal(0.0, report.WerRepair);
            Assert.Equal(0.0, report.WerCash);
            Assert.Equal(0.0, report.WerBank);
        }

        [Fact]
        public void TransitionToNewMonth_SetsWerStartFromEnd()
        {
            var report = CreateReport();
            report.TransitionToNewMonth();

            Assert.Equal(Math.Round(200.567, 2), report.WerStartDebit);
            Assert.Equal(Math.Round(15.891, 2), report.WerStartCredit);
        }

        [Fact]
        public void TransitionToNewMonth_SetsWaterPreviousFromCurrent()
        {
            var report = CreateReport();
            report.TransitionToNewMonth();

            Assert.Equal(300, report.WaterPreviousValue);
        }

        [Fact]
        public void GetObjects_WithoutRow_ReturnsValueStrings()
        {
            var report = new Report2
            {
                HeatingStartDebit = 100,
                HeatingStartCredit = 0,
                HeatingSquare = 50,
                HeatingType = "гкал",
                HeatingCurrentValue = 10,
                HeatingPreviousValue = 8,
                HeatingValue = 2,
                HeatingForService = 150,
                HeatingPreviliges = 0,
                HeatingTotal = 250,
                HeatingBank = 100,
                HeatingCash = 50,
                HeatingEndDebit = 100,
                HeatingEndCredit = 0,
                WerStartDebit = 200,
                WerStartCredit = 0,
                WerSquare = 50,
                WerForMonth = 80,
                RepairForMonth = 20,
                LivingPersons = 3,
                GarbageForMonth = 15,
                WaterCurrentValue = 150,
                WaterPreviousValue = 140,
                WaterValue = 10,
                WaterForMonth = 50,
                WerWaterForService = 165,
                WerPreviliges = 0,
                WerTotal = 365,
                WerRepair = 20,
                WerBank = 200,
                WerCash = 100,
                WerEndDebit = 65,
                WerEndCredit = 0
            };

            var (heating, wer) = report.GetObjects();

            Assert.Equal(14, heating.Count);
            Assert.Equal(19, wer.Count);
            Assert.Equal(100.0, heating[0]); // HeatingStartDebit
            Assert.Equal("гкал", heating[3]); // HeatingType
        }

        [Fact]
        public void GetObjects_WithRow_ReturnsFormulas()
        {
            var report = new Report2 { HeatingType = "гкал" };
            var (heating, wer) = report.GetObjects(10);

            Assert.Equal("=H10-I10", heating[6]); // HeatingValue formula
            Assert.Equal("=J10*K$3*1,1", heating[7]); // HeatingForService formula for гкал
        }

        [Theory]
        [InlineData("гкал", 10, "=J10*K$3*1,1")]
        [InlineData("мвт", 10, "=J10*K$3*0,86*1,1")]
        [InlineData("квт", 10, "=J10*K$3*1,1/1162,2")]
        [InlineData("гдж", 10, "=J10*K$3*1,1/4,187")]
        [InlineData("", 10, "=F10*K$6")]
        public void GetObjects_DifferentHeatingTypes_ReturnsCorrectFormula(string heatingType, int row, string expectedFormula)
        {
            var report = new Report2 { HeatingType = heatingType };
            var (heating, _) = report.GetObjects(row);

            Assert.Equal(expectedFormula, heating[7]);
        }

        [Fact]
        public void GetObjects_InvalidHeatingType_ThrowsArgumentException()
        {
            var report = new Report2 { HeatingType = "invalid" };
            Assert.Throws<ArgumentException>(() => report.GetObjects(10));
        }
    }
}
