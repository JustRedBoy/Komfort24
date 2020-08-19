using GoogleLib;
using Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Desktop.Tools;
using Models;

namespace Desktop.Commands
{
    public static class GenerationFlyers
    {
        internal delegate void GenerationHandler(int value);
        internal static event GenerationHandler UpdateProgress;

        internal static bool Processing { get; set; } = false;
        internal static bool IsCancelled { get; set; } = false;

        private static int _generated = 0;
        private static int Generated
        {
            get { return _generated; }
            set
            {
                _generated = value;
                if (_generated != 0)
                {
                    UpdateProgress?.Invoke(_generated);
                }
            }
        }

        private static CancellationTokenSource _cts;

        /// <summary>
        /// Starting process of generating flyers
        /// </summary>
        internal static async Task<bool> StartGenerationAsync()
        {
            _cts = new CancellationTokenSource();
            Processing = true;
            IsCancelled = false;

            string folderPath = Environment.CurrentDirectory + $"\\Листовки за {Date.GetNamePrevMonth()}";
            Directory.CreateDirectory(folderPath);
            try
            {
                //start generation in 6 tasks
                Task<int>[] tasks = new Task<int>[6];
                for (int i = 0; i < Houses.Count; i++) 
                {
                    int num = i;
                    tasks[i] = Task.Run(() => Start(Houses.GetHouseInfo(num).fullHouseNumber, 0, _cts.Token));
                }
                await Task.WhenAll(tasks);

                //extra generation if there were errors
                if (!_cts.IsCancellationRequested)
                {
                    for (int i = 0; i < Houses.Count; i++)
                    {
                        int generatedCount = tasks[i].Result;
                        if (generatedCount != Houses.GetNumFlats(i))
                        {
                            generatedCount = await Task.Run(() => Start(Houses.GetHouseInfo(i).fullHouseNumber, generatedCount, _cts.Token));
                            if (generatedCount != Houses.GetNumFlats(i))
                            {
                                throw new Exception("Генерация завершилась с ошибкой после 2 попыток, повторите операцию");
                            }
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                _cts?.Dispose();
                Generated = 0;
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
                IsCancelled = true;
                _cts?.Cancel();
            }
        }

        private static int Start(string house, int startNum, CancellationToken token = default)
        {
            GoogleSheets googleSheets = new GoogleSheets();
            IList<IList<object>> info;
            IList<object> rates;
            try
            {
                info = googleSheets.GetHouseInfo(house);
                rates = googleSheets.GetRates(house);
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }

            Word word = new Word();
            string folderPath = Environment.CurrentDirectory + $"\\Листовки за {Date.GetNamePrevMonth()}";
            string filePath = folderPath + $"\\{house.Replace('/', '_')}.docx";
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
                for (; generatedInHouse < Houses.GetNumFlats(house); generatedInHouse++)
                {
                    if (token.IsCancellationRequested)
                    {
                        return generatedInHouse;
                    }
                    word.FormationFlayer(doc, info[generatedInHouse], rates, house);
                    Generated++;
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
    }
}
