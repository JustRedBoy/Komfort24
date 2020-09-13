using System.Collections.Generic;
using Tools;

namespace Models
{
    public class House
    {
        public string FullAdress { get => "Пишоновская, " + ShortAdress; }
        public string ShortAdress { get; }
        public List<Account> Accounts { get; private set; }
        public Rates Rates { get; set; }
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

        /// <summary>
        /// Сlear stored accounts
        /// </summary>
        public void ClearAccounts()
        {
            Accounts = null;
        }

        /// <summary>
        /// Check the existence of account in currect house
        /// </summary>
        /// <param name="accountId">Account ID for check</param>
        /// <param name="account">Account or null</param>
        public bool HaveAccount(string accountId, out Account account)
        {
            int start = int.Parse(Accounts[0].AccountId);
            int end = int.Parse(Accounts[^1].AccountId);
            int numAccount = int.Parse(accountId.Substring(0, 4));
            account = null;
            if (numAccount >= start && numAccount <= end)
            {
                foreach (var acc in Accounts)
                {
                    if (acc.AccountId == accountId)
                    {
                        account = acc;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
