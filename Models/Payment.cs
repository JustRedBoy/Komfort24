using System.Collections.Generic;
using Tools;

namespace Models
{
    public class Payment
    {
        public string AccountId { get; set; }
        public string FlatOwner { get; set; }
        public double ForWer { get; set; }
        public double ForWater { get; set; }
        public double ForHeating { get; set; }
        public double Total { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }

        public Payment(IList<object> info)
        {
            AccountId = info[0].ToString();
            FlatOwner = info[1].ToString();
            ForWer = Number.GetDouble(info[2]);
            ForWater = Number.GetDouble(info[3]);
            ForHeating = Number.GetDouble(info[4]);
            Total = Number.GetDouble(info[5]);
            Month = info[6].ToString();
            Year = int.Parse(info[7].ToString());
        }
    }
}
