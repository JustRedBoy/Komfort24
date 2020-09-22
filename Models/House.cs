using System.Collections.Generic;
using System.Text.Json.Serialization;
using Tools;

namespace Models
{
    public class House
    {
        public string FullAdress { get => "Пишоновская, " + ShortAdress; }
        [JsonIgnore]
        public string ShortAdress { get; }
        [JsonIgnore]
        public List<Account> Accounts { get; }
        public Rates Rates { get; set; }
        [JsonIgnore]
        public int FlatCount { get; }

        public House(string shortAdress, IEnumerable<IList<object>> accounts, IList<object> rates)
        {
            ShortAdress = shortAdress;
            Accounts = new List<Account>();
            foreach (var accountInfo in accounts)
            {
                if (Matching.IsAccountId(accountInfo[1].ToString()))
                {
                    Accounts.Add(new Account(accountInfo, this));
                }
            }
            Rates = new Rates(rates);
            FlatCount = Accounts.Count;
        }
    }
}
