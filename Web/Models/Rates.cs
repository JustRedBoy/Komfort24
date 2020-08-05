namespace Web.Models
{
    public class Rates
    {
        public Houses.House HouseId { get; set; }
        public double SpecialWerRate { get; set; }
        public double GeneralWerRate { get; set; }
        public double WaterRate { get; set; }
        public double CentralHeatingRate { get; set; }
        public double CustomHeatingRate { get; set; }
        //public double GarbageRate { get; set; }
        //public int Month { get; set; }
        //public int Year { get; set; }

        public Rates(Houses.House houseId, double swr, double gwr, double wr, double cshr, double cnhr)
        {
            HouseId = houseId;
            SpecialWerRate = swr;
            GeneralWerRate = gwr;
            WaterRate = wr;
            CustomHeatingRate = cshr;
            CentralHeatingRate = cnhr;
        }
    }
}
