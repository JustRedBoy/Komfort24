using System.Collections.Generic;
using Tools.Extensions;

namespace Models
{
    public class Report
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
        public double WerWaterForService { get; set; }
        public double WerPreviliges { get; set; }
        public double WerTotal { get; set; }
        public double WerCash { get; set; }
        public double WerBank { get; set; }
        public double WerEndDebit { get; set; }
        public double WerEndCredit { get; set; }

        public Report(IList<object> info)
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
            HeatingCash = info[13].ToDouble();
            HeatingBank = info[14].ToDouble();
            HeatingEndDebit = info[15].ToDouble();
            HeatingEndCredit = info[16].ToDouble();
            WerStartDebit = info[17].ToDouble();
            WerStartCredit = info[18].ToDouble();
            WerSquare = info[19].ToDouble();
            WerForMonth = info[20].ToDouble();
            WaterCurrentValue = info[21].ToInt();
            WaterPreviousValue = info[22].ToInt();
            WaterValue = info[23].ToInt();
            WaterForMonth = info[24].ToDouble();
            WerWaterForService = info[25].ToDouble();
            WerPreviliges = info[26].ToDouble();
            WerTotal = info[27].ToDouble();
            WerCash = info[28].ToDouble();
            WerBank = info[29].ToDouble();
            WerEndDebit = info[30].ToDouble();
            WerEndCredit = info[31].ToDouble();
        }
    }
}
