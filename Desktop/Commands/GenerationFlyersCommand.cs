using GoogleLib;
using Tools;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Models;
using Desktop.Extensions;

namespace Desktop.Commands
{
    public static class GenerationFlyersCommand
    {
        internal delegate void GenerationHandler(double value);
        internal static event GenerationHandler UpdateProgress;

        internal static bool Processing { get; set; } = false;
        internal static bool Cancelled { get; set; } = false;

        private static int _processIndicator = 0;
        private static double _interval = 0;
        private static CancellationTokenSource _cts;

        /// <summary>
        /// Starting process of generating flyers
        /// </summary>
        /// <returns>Result of operation</returns>
        internal static async Task<bool> StartGenerationAsync()
        {
            Processing = true;
            Cancelled = false;
            _cts = new CancellationTokenSource();
            try
            {
                GoogleSheets sheets = new GoogleSheets();
                ServiceContext serviceContext = new ServiceContext();
                await serviceContext.InitContextAsync(sheets);
                _processIndicator = 0;
                _interval = 100.0 / serviceContext.TotalAccounts;

                string folderPath = Environment.CurrentDirectory + $"\\Листовки за {Date.GetNamePrevMonth()}";
                Directory.CreateDirectory(folderPath);

                //start generation (every house in task)
                Task<int>[] tasks = new Task<int>[serviceContext.TotalHouses];
                for (int i = 0; i < serviceContext.TotalHouses; i++)
                {
                    int num = i;
                    tasks[i] = Task.Run(() => serviceContext.Houses[num].StartFlyersGeneration(0, _cts.Token));
                }
                await Task.WhenAll(tasks);

                //extra generation if there were errors
                if (!_cts.IsCancellationRequested)
                {
                    for (int i = 0; i < serviceContext.TotalHouses; i++)
                    {
                        int generatedCount = tasks[i].Result;
                        if (generatedCount != serviceContext.TotalAccounts)
                        {
                            generatedCount = await Task.Run(() => 
                                serviceContext.Houses[i].StartFlyersGeneration(generatedCount, _cts.Token));
                            if (generatedCount != serviceContext.Houses[i].FlatCount)
                            {
                                throw new Exception("Генерация завершилась с ошибкой после 2 попыток, повторите операцию");
                            }
                        }
                    }
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                _cts?.Dispose();
                Processing = false;
            }
        }

        /// <summary>
        /// Сanceling process of generating flyers
        /// </summary>
        internal static void CancelGeneration()
        {
            if (Processing)
            {
                Cancelled = true;
                _cts?.Cancel();
            }
        }

        internal static void UpdateInfo()
        {
            UpdateProgress(++_processIndicator * _interval);
        }
    }
}
