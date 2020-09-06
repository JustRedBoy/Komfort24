using GoogleLib;
using Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Desktop.Commands
{
    internal static class SearchReportsCommand
    {
        internal static bool Processing { get; set; } = false;
        private static IList<IList<object>> Info { get; set; } = null;

        /// <summary>
        /// Starting the reports search process
        /// </summary>
        /// <param name="accountId">Account to search reports</param>
        /// <returns></returns>
        internal static async Task<List<Report>> SearchAsync(string accountId)
        {
            Processing = true;
            try
            {
                List<Report> reports = null;
                Regex regex = new Regex(@"(\d{4}$)|(\d{4}/[1|2]$)"); // 4 digits or 4 digits with /1 or /2
                MatchCollection matches = regex.Matches(accountId);
                if (matches.Count > 0)
                {
                    if (Info == null)
                    {
                        GoogleSheets googleSheets = new GoogleSheets();
                        Info = await googleSheets.GetReportsAsync();
                    }
                    reports = new List<Report>();
                    foreach (var item in Info)
                    {
                        if (item[1].ToString() == accountId)
                        {
                            reports.Add(new Report(item));
                        }
                    }
                }
                return reports;
            }
            finally
            {
                Processing = false;
            }
        }
    }
}
