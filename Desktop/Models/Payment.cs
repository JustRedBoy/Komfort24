using System.Collections.Generic;

namespace Desktop.Models
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
            ForWer = GetNumberValue(info[2]);
            ForWater = GetNumberValue(info[3]);
            ForHeating = GetNumberValue(info[4]);
            Total = GetNumberValue(info[5]);
            Month = info[6].ToString();
            Year = int.Parse(info[7].ToString());
        }

        private static double GetNumberValue(object obj)
        {
            return string.IsNullOrEmpty(obj.ToString()) ? 0.0 : double.Parse(obj.ToString());
        }
    }
}
