using System.Text.RegularExpressions;

namespace Tools
{
    /// <summary>
    /// Class for matching account ID
    /// </summary>
    public static class Matching
    {
        /// <summary>
        /// Checking if a string is really an identifier
        /// </summary>
        /// <param name="accountId">String for check</param>
        /// <returns>Is the string really an accountId</returns>
        public static bool IsAccountId(string accountId)
        {
            Regex regex = new Regex(@"(\d{4}$)|(\d{4}/[1|2]$)"); // 4 digits or 4 digits with /1 or /2
            MatchCollection matches = regex.Matches(accountId);
            return matches.Count > 0;
        }
    }
}
