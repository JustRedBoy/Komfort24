using Desktop.Commands;
using Desktop.Tools;
using GoogleLib;
using Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tools;

namespace Desktop.Extensions
{
    internal static class HouseExtensions
    {
        internal static int StartFlyersGeneration(this House house, int startNum, CancellationToken token = default)
        {
            Word word = new Word();
            string folderPath = Environment.CurrentDirectory + $"\\Листовки за {Date.GetNameCurMonth()}";
            string filePath = folderPath + $"\\{house.ShortAdress.Replace('/', '_')}.docx";
            if (startNum == 0)
            {
                word.CopyDocument(Environment.CurrentDirectory + $"\\Resources\\FlyerTemplate.docx", filePath);
            }
            var doc = word.OpenDocument(filePath);

            int generatedInHouse = startNum;
            if (startNum == 0)
            {
                doc.Tables[1].Range.Cut();
            }
            try
            {
                for (; generatedInHouse < house.FlatCount; generatedInHouse++)
                {
                    if (token.IsCancellationRequested)
                    {
                        return generatedInHouse;
                    }
                    word.FormationFlayer(doc, house.Accounts[generatedInHouse], house.Rates);
                    GenerationFlyersCommand.UpdateInfo();
                }
                return generatedInHouse;
            }
            catch (Exception)
            {
                return generatedInHouse;
            }
            finally
            {
                word.SaveDocument(doc);
                word.CloseDocument(doc);
                word.Quit();
            }
        }

        internal static async Task TransitionToNewMonth(this House house, string monthName)
        {
            var currentHeatingReportsInfo = new List<IList<object>>();
            var currentWerReportsInfo = new List<IList<object>>();
            var month = new List<IList<object>> { new List<object> { monthName } };
            int row = 9;
            foreach (Account account in house.Accounts)
            {
                account.CurrentReport.TransitionToNewMonth();
                var (heating, wer) = account.CurrentReport.GetObjects(row);
                currentHeatingReportsInfo.Add(heating);
                currentWerReportsInfo.Add(wer);
                row++;
            }
            GoogleSheets sheets = new GoogleSheets();
            await sheets.UpdateHouseInfoAsync(house.ShortAdress, currentHeatingReportsInfo,
                currentWerReportsInfo, month);
        }

        internal static IList<IList<object>> GetObjects(this House house)
        {
            IList<IList<object>> objects = new List<IList<object>>();
            foreach (Account account in house.Accounts)
            {
                if (account.AccountId == "7695")
                {
                    continue;
                }
                objects.Add(account.GetObjects());
            }
            return objects;
        }
    }
}
