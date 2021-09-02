using System.Collections.Generic;
using Tools.Extensions;

namespace Models
{
    public class ArchiveReport2 : Report2
    {
        public string AccountId { get; }
        public string FlatNumber { get; }
        public string Owner { get; }
        public string Month { get; set; }
        public int Year { get; set; }

        public ArchiveReport2(IList<object> info) : base(info)
        {
            FlatNumber = info[0].ToString();
            AccountId = info[1].ToString();
            Owner = info[2].ToString();
            Month = info[36].ToString();
            Year = info[37].ToInt();
        }

        /// <summary>
        /// Convert from old structure
        /// </summary>
        /// <param name="archiveReport">Old archive report</param>
        public ArchiveReport2(ArchiveReport archiveReport)
        {
            FlatNumber = archiveReport.FlatNumber;
            AccountId = archiveReport.AccountId;
            Owner = archiveReport.Owner;
            HeatingStartDebit = archiveReport.HeatingStartDebit;
            HeatingStartCredit = archiveReport.HeatingStartCredit;
            HeatingSquare = archiveReport.HeatingSquare;
            HeatingType = archiveReport.HeatingType;
            HeatingCurrentValue = archiveReport.HeatingCurrentValue;
            HeatingPreviousValue = archiveReport.HeatingPreviousValue;
            HeatingValue = archiveReport.HeatingValue;
            HeatingForService = archiveReport.HeatingForService;
            HeatingPreviliges = archiveReport.HeatingPreviliges;
            HeatingTotal = archiveReport.HeatingTotal;
            HeatingBank = archiveReport.HeatingBank;
            HeatingCash = archiveReport.HeatingCash;
            HeatingEndDebit = archiveReport.HeatingEndDebit;
            HeatingEndCredit = archiveReport.HeatingEndCredit;

            WerStartDebit = archiveReport.WerStartDebit;
            WerStartCredit = archiveReport.WerStartCredit;
            WerSquare = archiveReport.WerSquare;
            WerForMonth = archiveReport.WerForMonth;
            RepairForMonth = 0.0;
            LivingPersons = 0;
            GarbageForMonth = 0.0;
            WaterCurrentValue = archiveReport.WaterCurrentValue;
            WaterPreviousValue = archiveReport.WaterPreviousValue;
            WaterValue = archiveReport.WaterValue;
            WaterForMonth = archiveReport.WaterForMonth;
            WerWaterForService = archiveReport.WerWaterForService;
            WerPreviliges = archiveReport.WerPreviliges;
            WerTotal = archiveReport.WerTotal;
            WerRepair = 0.0;
            WerBank = archiveReport.WerBank;
            WerCash = archiveReport.WerCash;
            WerEndDebit = archiveReport.WerEndDebit;
            WerEndCredit = archiveReport.WerEndCredit;
            Month = archiveReport.Month;
            Year = archiveReport.Year;
        }
    }
}