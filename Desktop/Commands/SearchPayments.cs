using GoogleLib;
using Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Desktop.Commands
{
    internal static class SearchPayments
    {
        internal static bool Processing { get; set; } = false;

        /// <summary>
        /// Starting the payments search process
        /// </summary>
        /// <param name="accountId">Account to search payments</param>
        /// <returns></returns>
        internal static async Task<List<Payment>> SearchAsync(string accountId)
        {
            Processing = true;
            try
            {
                List<Payment> payments = null;
                Regex regex = new Regex(@"(\d{4}$)|(\d{4}/[1|2]$)"); // 4 digits or 4 digits with /1 or /2
                MatchCollection matches = regex.Matches(accountId);
                if (matches.Count > 0)
                {
                    GoogleSheets googleSheets = new GoogleSheets();
                    var info = await googleSheets.GetPaymentsAsync();

                    payments = new List<Payment>();
                    foreach (var item in info)
                    {
                        if (item[0].ToString() == accountId)
                        {
                            payments.Add(new Payment(item));
                        }
                    }
                }
                return payments;
            }
            finally
            {
                Processing = false;
            }
        }
    }
}
