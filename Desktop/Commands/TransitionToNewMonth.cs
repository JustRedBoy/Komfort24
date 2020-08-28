using GoogleLib;
using Tools;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Desktop.Commands
{
    internal static class TransitionToNewMonth
    {
        internal delegate void TransitionHandler(double value, string message);
        internal static event TransitionHandler UpdateProgress;

        internal static bool Processing { get; set; } = false;

        private static int _processIndicator = 0;
        private static readonly double _interval = 100.0 / (Houses.Count * 2 + 3);

        /// <summary>
        /// Starting of the transition to a new month
        /// </summary>
        /// <returns></returns>
        internal async static Task<bool> StartTransitionAsync()
        {
            Processing = true;
            GoogleDrive drive = new GoogleDrive();
            GoogleSheets sheets = new GoogleSheets();
            _processIndicator = 0;

            try
            {
                if (await TransitionCheck(drive))
                {
                    await CreateFolderAndCopyFilesAsync(drive);
                    await AddNewPayments(sheets);
                    await CorrectFiles(sheets);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                Processing = false;
            }
        }
        private static async Task<bool> TransitionCheck(GoogleDrive drive)
        {
            UpdateInfo("Проверка ...");
            IEnumerable<string> files = await drive.GetFilesAsync();
            if (files != null && files.Count() > 0)
            {
                foreach (string name in files)
                {
                    //if a folder with a special name exists, then the transition has already been
                    if (name == Date.GetPrevDate())
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private static async Task CreateFolderAndCopyFilesAsync(GoogleDrive drive)
        {
            UpdateInfo("Создание отдельной папки для файлов ...");
            string folderId = await drive.CreateFolderAsync(Date.GetPrevDate());
            UpdateInfo("Копирование файлов в отдельную папку ...");
            await drive.CopyFileAsync(Sheets.HeatingSpreadSheetId, $"Ведомость О ({Date.GetPrevDate()})", folderId);
            await drive.CopyFileAsync(Sheets.WerSpreadSheetId, $"Ведомость СД ({Date.GetPrevDate()})", folderId);
        }
        private static async Task AddNewPayments(GoogleSheets sheets)
        {
            IList<IList<object>> payments = new List<IList<object>>();
            for (int i = 0; i < Houses.Count; i++)
            {
                UpdateInfo($"Формирование платежей для дома {Houses.GetHouseInfo(i).fullHouseNumber} ...");
                var info = await sheets.GetHouseInfoAsync(Houses.GetHouseInfo(i).fullHouseNumber);
                for (int j = 0; j < Houses.GetNumFlats(i); j++)
                {
                    double heatingPayment = Number.GetDouble(info[j][13]) + Number.GetDouble(info[j][14]) + Number.GetDouble(info[j][11]);
                    double werPayment = Number.GetDouble(info[j][29]) + Number.GetDouble(info[j][30]) + Number.GetDouble(info[j][27]);
                    if (heatingPayment != 0 || werPayment != 0)
                    {
                        double forWater = Number.GetDouble(info[j][25]);
                        double forWer = Math.Round(werPayment - forWater, 2);
                        payments.Add(new List<object>()
                        {
                            info[j][1],
                            info[j][2],
                            forWer,
                            forWater,
                            heatingPayment,
                            forWer + forWater + heatingPayment,
                            Date.GetNamePrevMonth(),
                            DateTime.Now.Year
                        });
                    }
                }
            }
            var oldPayments = await sheets.GetPaymentsAsync();
            foreach (var item in oldPayments)
            {
                payments.Add(new List<object>()
                {
                    item[0],
                    item[1],
                    Number.GetDouble(item[2]),
                    Number.GetDouble(item[3]),
                    Number.GetDouble(item[4]),
                    Number.GetDouble(item[5]),
                    item[6],
                    item[7]
                });
            }
            await sheets.WriteInfoAsync(payments, Sheets.PaymentsSpreadSheetId, $"A2:H{payments.Count + 1}");
        }
        private static async Task CorrectFiles(GoogleSheets sheets)
        {
            var month = new List<IList<object>> { new List<object>() };
            month[0].Add(Date.GetNameCurMonth());

            for (int i = 0; i < Houses.Count; i++)
            {
                UpdateInfo($"Переход на новый месяц дома {Houses.GetHouseInfo(i).fullHouseNumber} ...");
                string houseNum = Houses.GetHouseInfo(i).fullHouseNumber;
                int numFlats = Houses.GetNumFlats(i);

                // Change debit and credit
                var debitAndCredit = await sheets.ReadInfoAsync(Sheets.WerSpreadSheetId, $"{houseNum}!Q9:R{8 + numFlats}");
                await sheets.WriteInfoAsync(GetEmptyList(numFlats, 2), Sheets.WerSpreadSheetId, $"{houseNum}!D9:E{8 + numFlats}");
                await sheets.WriteInfoAsync(GetListDoubles(debitAndCredit), Sheets.WerSpreadSheetId, $"{houseNum}!D9:E{8 + numFlats}");

                // Dublicate last values
                var lastValues = await sheets.ReadInfoAsync(Sheets.WerSpreadSheetId, $"{houseNum}!H9:H{8 + numFlats}");
                await sheets.WriteInfoAsync(GetListDoubles(lastValues, 3), Sheets.WerSpreadSheetId, $"{houseNum}!I9:I{8 + numFlats}");

                // Clear payments
                await sheets.WriteInfoAsync(GetEmptyList(numFlats, 2), Sheets.WerSpreadSheetId, $"{houseNum}!O9:P{8 + numFlats}");

                // Clear privileges
                await sheets.WriteInfoAsync(GetEmptyList(numFlats, 1), Sheets.WerSpreadSheetId, $"{houseNum}!M9:M{8 + numFlats}");

                //Change month
                await sheets.WriteInfoAsync(month, Sheets.WerSpreadSheetId, $"{houseNum}!I2:J2");


                // Change debit and credit
                var debitAndCredit2 = await sheets.ReadInfoAsync(Sheets.HeatingSpreadSheetId, $"{houseNum}!P9:Q{8 + numFlats}");
                await sheets.WriteInfoAsync(GetEmptyList(numFlats, 2), Sheets.HeatingSpreadSheetId, $"{houseNum}!D9:E{8 + numFlats}");
                await sheets.WriteInfoAsync(GetListDoubles(debitAndCredit2), Sheets.HeatingSpreadSheetId, $"{houseNum}!D9:E{8 + numFlats}");

                // Dublicate last values
                var lastValues2 = await sheets.ReadInfoAsync(Sheets.HeatingSpreadSheetId, $"{houseNum}!H9:H{8 + numFlats}");
                await sheets.WriteInfoAsync(GetListDoubles(lastValues2, 3), Sheets.HeatingSpreadSheetId, $"{houseNum}!I9:I{8 + numFlats}");

                // Clear payments
                await sheets.WriteInfoAsync(GetEmptyList(numFlats, 2), Sheets.HeatingSpreadSheetId, $"{houseNum}!N9:O{8 + numFlats}");

                // Clear privileges
                await sheets.WriteInfoAsync(GetEmptyList(numFlats, 1), Sheets.HeatingSpreadSheetId, $"{houseNum}!L9:L{8 + numFlats}");

                //Change month
                await sheets.WriteInfoAsync(month, Sheets.HeatingSpreadSheetId, $"{houseNum}!I2:J2");
            }
            await sheets.WriteInfoAsync(month, Sheets.WerSpreadSheetId, $"Сводная ведомость!H2");
            await sheets.WriteInfoAsync(month, Sheets.HeatingSpreadSheetId, $"Сводная ведомость!H2");
        }
        private static IList<IList<object>> GetListDoubles(IList<IList<object>> info, int digits = 2)
        {
            for (int i = 0; i < info.Count; i++)
            {
                for (int j = 0; j < info[i].Count; j++)
                {
                    info[i][j] = Number.GetDouble(info[i][j], digits);
                }
            }
            return info;
        }
        private static IList<IList<object>> GetEmptyList(int rows, int colomns)
        {
            var list = new List<IList<object>>();
            for (int i = 0; i < rows; i++)
            {
                list.Add(new List<object>());
                for (int j = 0; j < colomns; j++)
                {
                    list[i].Add(0);
                }
            }
            return list;
        }
        private static void UpdateInfo(string message)
        {
            UpdateProgress(++_processIndicator * _interval, message);
        }
    }
}
