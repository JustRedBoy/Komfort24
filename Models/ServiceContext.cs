﻿using GoogleLib;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools;

namespace Models
{
    public class ServiceContext
    {
        private const int MaxAccountNumber = 120;

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

            Houses = new List<House>();
            List<string> houses = new List<string>()
            {
                "20/1", "24/2", "22/2", "26/2", "26/1", "20/2", "24A"
            };

            for (int i = 0; i < houses.Count; i++)
            {
                Houses.Add(new House(houses[i], accounts.Skip(MaxAccountNumber * i).Take(MaxAccountNumber), rates[i]));
            }
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
