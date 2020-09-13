using System.Collections.Generic;
using Tools.Extensions;

namespace Models
{
    public class ArchiveReport : Report
    {
        public string AccountId { get; }
        public string FlatNumber { get; }
        public string Owner { get; }
        public string Month { get; set; }
        public int Year { get; set; }

        public ArchiveReport(IList<object> info) : base(info)
        {
            FlatNumber = info[0].ToString();
            AccountId = info[1].ToString();
            Owner = info[2].ToString();
            Month = info[32].ToString();
            Year = info[33].ToInt();
        }
    }
}
