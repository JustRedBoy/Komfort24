using GoogleLib;
using Management.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Management.Commands
{
    public static class SearchPayments
    {
        public static bool Processing { get; set; } = false;

        public static async Task<List<Payment>> SearchAsync(string accountId)
        {
            Processing = true;
            List<Payment> payments = null;
            Regex regex = new Regex(@"(\d{4}$)|(\d{4}/[1|2]$)");
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
            Processing = false;
            return payments;
        }
    }
}
