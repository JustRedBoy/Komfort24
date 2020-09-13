using GoogleLib;
using Tools;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Desktop.Extensions;

namespace Desktop.Commands
{
    internal static class TransitionToNewMonthCommand
    {
        internal delegate void TransitionHandler(double value, string message);
        internal static event TransitionHandler UpdateProgress;

        internal static bool Processing { get; set; } = false;

        private static int _processIndicator = 0;
        private static double _interval = 0;

        /// <summary>
        /// Starting of the transition to a new month
        /// </summary>
        /// <returns>Result of operation</returns>
        internal async static Task<bool> StartTransitionAsync()
        {
            Processing = true;
            try
            {
                GoogleDrive drive = new GoogleDrive();
                GoogleSheets sheets = new GoogleSheets();
                ServiceContext serviceContext = new ServiceContext();
                await serviceContext.InitContextAsync(sheets);
                _processIndicator = 0;
                _interval = 100.0 / (serviceContext.TotalHouses + 4);

                if (await TransitionCheck(drive))
                {
                    await CreateFolderAndCopyFilesAsync(drive);
                    await AddReportsToArchive(sheets, serviceContext);
                    await CorrectFiles(sheets, serviceContext);
                    SearchReportsCommand.ClearReportsInfo();

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
                    if (name == Date.GetFullPrevMonth())
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
            string folderId = await drive.CreateFolderAsync(Date.GetFullPrevMonth());
            UpdateInfo("Копирование файлов в отдельную папку ...");
            await drive.CopyAllFilesAsync(folderId);
        }

        private static async Task AddReportsToArchive(GoogleSheets sheets, ServiceContext serviceContext)
        {
            List<IList<object>> archiveReports = new List<IList<object>>();
            for (int i = 0; i < serviceContext.TotalHouses; i++)
            {
                archiveReports.AddRange(serviceContext.Houses[i].GetObjects());
            }
            UpdateInfo($"Сохранение отчета ...");
            var oldReports = await sheets.GetArchiveReportsInfoAsync();
            archiveReports.AddRange(oldReports);
            await sheets.UpdateArchiveReportsInfoAsync(archiveReports);
        }

        private static async Task CorrectFiles(GoogleSheets sheets, ServiceContext serviceContext)
        {
            string month = Date.GetNameCurMonth();
            for (int i = 0; i < serviceContext.TotalHouses; i++)
            {
                UpdateInfo($"Переход на новый месяц дома {serviceContext.Houses[i].ShortAdress} ...");
                await serviceContext.Houses[i].TransitionToNewMonth(month);
            }
            await sheets.UpdateGeneralMonthAsync(new List<IList<object>> { new List<object> { month, DateTime.Now.Year } });
        }

        private static void UpdateInfo(string message)
        {
            UpdateProgress(++_processIndicator * _interval, message);
        }
    }
}
