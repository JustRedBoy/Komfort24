using System.Collections.Generic;
using Tools.Extensions;

namespace Models
{
    public class Rates
    {
        public double SpecialWerRate { get; }
        public double GeneralWerRate { get; }
        public double WaterRate { get; }
        public double CentralHeatingRate { get; }
        public double CustomHeatingRate { get; }
        public double GarbageRate { get; }

        public Rates(IList<object> rates)
        {
            WaterRate = rates[0].ToDouble(3);
            CentralHeatingRate = rates[1].ToDouble(3);
            CustomHeatingRate = rates[2].ToDouble(3);
            GeneralWerRate = rates[3].ToDouble(3);
            SpecialWerRate = rates[4].ToDouble(3);
            GarbageRate = rates[5].ToDouble(3);
        }
    }
}
