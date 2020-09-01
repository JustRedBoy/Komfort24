using System.Collections.Generic;
using Tools;

namespace Models
{
    public class Report
    {
        public string FlatNumber { get; set; }
        public string AccountId { get; set; }
        public string Owner { get; set; }
        public double HeatingStartDebit { get; set; }
        public double HeatingStartCredit { get; set; }
        public double HeatingSquare { get; set; }
        public string HeatingType { get; set; }
        public double HeatingCurrentValue { get; set; }
        public double HeatingPreviousValue { get; set; }
        public double HeatingValue { get; set; }
        public double HeatingForService { get; set; }
        public double HeatingPreviliges { get; set; }
        public double HeatingTotal { get; set; }
        public double HeatingCash { get; set; }
        public double HeatingBank { get; set; }
        public double HeatingEndDebit { get; set; }
        public double HeatingEndCredit { get; set; }

        public double WerStartDebit { get; set; }
        public double WerStartCredit { get; set; }
        public double WerSquare { get; set; }
        public double WerForMonth { get; set; }
        public int WaterCurrentValue { get; set; }
        public int WaterPreviousValue { get; set; }
        public int WaterValue { get; set; }
        public double WaterForMonth { get; set; }
        public double WerForService { get; set; }
        public double WerPreviliges { get; set; }
        public double WerTotal { get; set; }
        public double WerCash { get; set; }
        public double WerBank { get; set; }
        public double WerEndDebit { get; set; }
        public double WerEndCredit { get; set; }

        public string Month { get; set; }
        public int Year { get; set; }

        public Report(string flatNumber, string accountId, string owner,
            double heatingStartDebit, double heatingStartCredit, double heatingSquare,
            string heatingType, double heatingCurrentValue, double heatingPreviousValue,
            double heatingValue, double heatingForService, double heatingPreviliges,
            double heatingTotal, double heatingCash, double heatingBank,
            double heatingEndDebit, double heatingEndCredit, double werStartDebit,
            double werStartCredit, double werSquare, double werForMonth,
            int waterCurrentValue, int waterPreviousValue, int waterValue,
            double waterForMonth, double werForService, double werPreviliges, double werTotal,
            double werCash, double werBank, double werEndDebit, double werEndCredit,
            string month, int year)
        {
            FlatNumber = flatNumber;
            AccountId = accountId;
            Owner = owner;
            HeatingStartDebit = heatingStartDebit;
            HeatingStartCredit = heatingStartCredit;
            HeatingSquare = heatingSquare;
            HeatingType = heatingType;
            HeatingCurrentValue = heatingCurrentValue;
            HeatingPreviousValue = heatingPreviousValue;
            HeatingValue = heatingValue;
            HeatingForService = heatingForService;
            HeatingPreviliges = heatingPreviliges;
            HeatingTotal = heatingTotal;
            HeatingCash = heatingCash;
            HeatingBank = heatingBank;
            HeatingEndDebit = heatingEndDebit;
            HeatingEndCredit = heatingEndCredit;
            WerStartDebit = werStartDebit;
            WerStartCredit = werStartCredit;
            WerSquare = werSquare;
            WerForMonth = werForMonth;
            WaterCurrentValue = waterCurrentValue;
            WaterPreviousValue = waterPreviousValue;
            WaterValue = waterValue;
            WaterForMonth = waterForMonth;
            WerForService = werForService;
            WerPreviliges = werPreviliges;
            WerTotal = werTotal;
            WerCash = werCash;
            WerBank = werBank;
            WerEndDebit = werEndDebit;
            WerEndCredit = werEndCredit;
            Month = month;
            Year = year;
        }

        public Report(IList<object> info) : this(
            info[0].ToString(), info[1].ToString(), info[2].ToString(), info[3].ToDouble(), 
            info[4].ToDouble(), info[5].ToDouble(), info[6].ToString(), info[7].ToDouble(3),
            info[8].ToDouble(3), info[9].ToDouble(3), info[10].ToDouble(), info[11].ToDouble(), 
            info[12].ToDouble(), info[13].ToDouble(), info[14].ToDouble(), info[15].ToDouble(), 
            info[16].ToDouble(), info[17].ToDouble(), info[18].ToDouble(), info[19].ToDouble(), 
            info[20].ToDouble(), info[21].ToInt(), info[22].ToInt(), info[23].ToInt(), 
            info[24].ToDouble(), info[25].ToDouble(), info[26].ToDouble(), info[27].ToDouble(), 
            info[28].ToDouble(), info[29].ToDouble(), info[30].ToDouble(), info[31].ToDouble(), 
            info[32].ToString(), info[33].ToInt())
        { }
    }
}
