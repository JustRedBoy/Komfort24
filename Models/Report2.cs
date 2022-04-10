using System.Collections.Generic;
using Tools.Extensions;

namespace Models
{
    /// <summary>
    /// New struct for report later than 2022
    /// </summary>
    public class Report2
    {
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
        public double HeatingBank { get; set; }
        public double HeatingCash { get; set; }
        public double HeatingEndDebit { get; set; }
        public double HeatingEndCredit { get; set; }

        public double WerStartDebit { get; set; }
        public double WerStartCredit { get; set; }
        public double WerSquare { get; set; }
        public double WerForMonth { get; set; }
        public double RepairForMonth { get; set; }
        public int LivingPersons { get; set; }
        public double GarbageForMonth { get; set; }
        public int WaterCurrentValue { get; set; }
        public int WaterPreviousValue { get; set; }
        public int WaterValue { get; set; }
        public double WaterForMonth { get; set; }
        public double WerWaterForService { get; set; }
        public double WerPreviliges { get; set; }
        public double WerTotal { get; set; }
        public double WerRepair { get; set; }
        public double WerBank { get; set; }
        public double WerCash { get; set; }
        public double WerEndDebit { get; set; }
        public double WerEndCredit { get; set; }

        public Report2(IList<object> info)
        {
            HeatingStartDebit = info[3].ToDouble();
            HeatingStartCredit = info[4].ToDouble();
            HeatingSquare = info[5].ToDouble();
            HeatingType = info[6].ToString();
            HeatingCurrentValue = info[7].ToDouble(3);
            HeatingPreviousValue = info[8].ToDouble(3);
            HeatingValue = info[9].ToDouble(3);
            HeatingForService = info[10].ToDouble();
            HeatingPreviliges = info[11].ToDouble();
            HeatingTotal = info[12].ToDouble();
            HeatingBank = info[13].ToDouble();
            HeatingCash = info[14].ToDouble();
            HeatingEndDebit = info[15].ToDouble();
            HeatingEndCredit = info[16].ToDouble();

            WerStartDebit = info[17].ToDouble();
            WerStartCredit = info[18].ToDouble();
            WerSquare = info[19].ToDouble();
            WerForMonth = info[20].ToDouble();
            RepairForMonth = info[21].ToDouble();
            LivingPersons = info[22].ToInt();
            GarbageForMonth = info[23].ToDouble();
            WaterCurrentValue = info[24].ToInt();
            WaterPreviousValue = info[25].ToInt();
            WaterValue = info[26].ToInt();
            WaterForMonth = info[27].ToDouble();
            WerWaterForService = info[28].ToDouble();
            WerPreviliges = info[29].ToDouble();
            WerTotal = info[30].ToDouble();
            WerRepair = info[31].ToDouble();
            WerBank = info[32].ToDouble();
            WerCash = info[33].ToDouble();
            WerEndDebit = info[34].ToDouble();
            WerEndCredit = info[35].ToDouble();
        }

        /// <summary>
        /// To support the old archived report structure
        /// </summary>
        public Report2() { }
    }
}