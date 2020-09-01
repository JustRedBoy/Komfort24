using System;

namespace Models
{
    public class Payment
    {
        public string AccountId { get; set; }
        public string Owner { get; set; }
        public double ForWer { get; set; }
        public double ForWater { get; set; }
        public double ForHeating { get; set; }
        public double Total { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }

        public Payment(Report report)
        {
            AccountId = report.AccountId;
            Owner = report.Owner;
            ForWer = report.WerForMonth;
            ForWater = report.WaterForMonth;
            ForHeating = report.HeatingForService;
            Total = Math.Round(report.WerForMonth + report.WaterForMonth + 
                report.HeatingForService, 2);
            Month = report.Month;
            Year = report.Year;
        }
    }
}
