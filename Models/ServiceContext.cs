using GoogleLib;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools;

namespace Models
{
    public class ServiceContext
    {
        public int TotalHouses { get => Houses.Count; }
        public int TotalAccounts
        {
            get
            {
                int totalCount = 0;
                foreach (House house in Houses)
                {
                    totalCount += house.FlatCount;
                }
                return totalCount;
            }
        }
        public List<House> Houses { get; private set; }

        /// <summary>
        /// Context initialization
        /// </summary>
        /// <param name="googleSheets">Google sheets service</param>
        public async Task InitContextAsync(GoogleSheets googleSheets)
        {
            var accounts = await googleSheets.GetAccountsInfoAsync();
            var rates = await googleSheets.GetRatesInfoAsync();

            Houses = new List<House>()
            {
                new House("20/1", accounts.Take(100), rates[0]),
                new House("24/2", accounts.Skip(100).Take(100), rates[1]),
                new House("22/2", accounts.Skip(200).Take(100), rates[2]),
                new House("26/2", accounts.Skip(300).Take(100), rates[3]),
                new House("26/1", accounts.Skip(400).Take(100), rates[4]),
                new House("20/2", accounts.Skip(500).Take(100), rates[5])
            };
        }

        /// <summary>
        /// Search account in all houses
        /// </summary>
        /// <param name="accountId">Account ID for search</param>
        /// <returns>Account or null</returns>
        public Account GetAccountById(string accountId)
        {
            if (Matching.IsAccountId(accountId))
            {
                foreach (var house in Houses)
                {
                    var account = house.Accounts.FirstOrDefault(a => a.AccountId == accountId);
                    if (account != null)
                    {
                        return account;
                    }
                }
            }
            return null;
        }
    }
}
