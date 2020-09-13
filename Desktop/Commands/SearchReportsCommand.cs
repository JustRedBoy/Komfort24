using GoogleLib;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;

namespace Desktop.Commands
{
    internal static class SearchReportsCommand
    {
        internal static bool Processing { get; set; } = false;
        private static IList<IList<object>> ArchiveReportsInfo { get; set; } = null;

        /// <summary>
        /// Starting the archive reports search process
        /// </summary>
        /// <param name="accountId">Account to search reports</param>
        /// <returns>List of achive reports</returns>
        internal static async Task<List<ArchiveReport>> SearchAsync(string accountId)
        {
            Processing = true;
            try
            {
                if (Matching.IsAccountId(accountId))
                {
                    if (ArchiveReportsInfo == null)
                    {
                        GoogleSheets googleSheets = new GoogleSheets();
                        ArchiveReportsInfo = await googleSheets.GetArchiveReportsInfoAsync();
                    }
                    List<ArchiveReport> reports = new List<ArchiveReport>();
                    foreach (var item in ArchiveReportsInfo)
                    {
                        if (item[1].ToString() == accountId)
                        {
                            reports.Add(new ArchiveReport(item));
                        }
                    }
                    return reports;
                }
                return null;
            }
            finally
            {
                Processing = false;
            }
        }

        /// <summary>
        /// Сlear stored reports information
        /// </summary>
        internal static void ClearReportsInfo()
        {
            ArchiveReportsInfo = null;
        }

        /// <summary>
        /// Сheck the existence of reports information
        /// </summary>
        internal static bool HaveReportsInfo()
        {
            return ArchiveReportsInfo != null;
        }
    }
}
