using Models;
using System;
using Tools;

namespace Desktop.Models
{
    public class PrintReport
    {
        public string AccountId { get; set; }
        public string Owner { get; set; }
        public double HeatingStateStart { get; set; }
        public double WerStateStart { get; set; }
        public double ForHeating { get; set; }
        public double ForWer { get; set; }
        public double ForWater { get; set; }
        public double WerPayment { get; set; }
        public double WaterPayment { get; set; }
        public double HeatingPayment { get; set; }
        public double HeatingStateEnd { get; set; }
        public double WerStateEnd { get; set; }
        public string MonthAndYear { get; set; }

        public PrintReport(Report report)
        {
            AccountId = report.AccountId;
            Owner = report.Owner;
            HeatingStateStart = Math.Round(report.HeatingStartDebit - report.HeatingStartCredit, 2);
            WerStateStart = Math.Round(report.WerStartDebit - report.WerStartCredit, 2);
            ForHeating = report.HeatingForService - report.HeatingPreviliges;
            ForWer = report.WerForMonth - report.WerPreviliges;
            ForWater = report.WaterForMonth;
            HeatingPayment = Math.Round(report.HeatingBank + report.HeatingCash, 2);
            double werTotalPayment = Math.Round(report.WerBank + report.WerCash, 2);
            double werPayment = Math.Round(werTotalPayment - report.WaterForMonth, 2);
            WerPayment = werTotalPayment == 0.0 ? 0.0 : werPayment < 0.0 ? 0.0 : werPayment;
            WaterPayment = werTotalPayment == 0.0 ? 0.0 : report.WaterForMonth;
            HeatingStateEnd = Math.Round(report.HeatingEndDebit - report.HeatingEndCredit, 2);
            WerStateEnd = Math.Round(report.WerEndDebit - report.WerEndCredit, 2);
            MonthAndYear = $"{Date.GetShortMonth(Date.GetNumMonth(report.Month))}.{report.Year}";
        }
    }
}
