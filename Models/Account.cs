using System.Collections.Generic;

namespace Models
{
    public class Account
    {
        public string AccountId { get; }
        public string FlatNumber { get; }
        public string Owner { get; }
        public House House { get; }
        public Report2 CurrentReport { get; }

        public Account(IList<object> accountInfo, House house)
        {
            FlatNumber = accountInfo[0].ToString();
            AccountId = accountInfo[1].ToString();
            Owner = accountInfo[2].ToString();
            House = house;

            CurrentReport = new Report2(accountInfo);
        }
    }
}
