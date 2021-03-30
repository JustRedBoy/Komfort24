using Models;
using System;
using System.Collections.Generic;
using Tools;

namespace Desktop.Extensions
{
    internal static class AccountExtensions
    {
        internal static IList<object> GetObjects(this Account account)
        {
            var objects = new List<object>
            {
                account.FlatNumber,
                account.AccountId,
                account.Owner
            };
            var (heating, wer) = account.CurrentReport.GetObjects();
            objects.AddRange(heating);
            objects.AddRange(wer);
            objects.Add(Date.GetNamePrevMonth());
            objects.Add(DateTime.Now.AddMonths(-1).Year);
            return objects;
        }
    }
}
