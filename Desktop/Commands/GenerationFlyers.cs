using GoogleLib;
using Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using WinTasks = System.Threading.Tasks;
using Desktop.Tools;

namespace Desktop.Commands
{
    public static class GenerationFlyers
    {
        #region Delegates and Events

        public delegate void GenerationHandler(int value);
        public static event GenerationHandler UpdateProgress;

        #endregion

        #region Fileds and Properties

        public static bool Processing { get; set; } = false;
        public static bool IsCancelled { get; set; } = false;

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

        #endregion

        #region Methods

        /// <summary>
        /// Starting process of generating flyers
        /// </summary>
        public static async WinTasks.Task<bool> StartGenerationAsync()
        {
            _cts = new CancellationTokenSource();
            Processing = true;
            IsCancelled = false;

            string folderPath = Environment.CurrentDirectory + $"\\Листовки за {Date.GetNamePrevMonth()}";
            Directory.CreateDirectory(folderPath);

            try
            {
                //start generation in 6 tasks
                WinTasks.Task<int>[] tasks = new WinTasks.Task<int>[6];
                for (int i = 0; i < 6; i++) 
                {
                    int num = i;
                    tasks[i] = WinTasks.Task.Run(() => Start(GetHouse(num), 0, _cts.Token));
                }
                await WinTasks.Task.WhenAll(tasks);

                //extra generation, if we had errors
                if (!_cts.IsCancellationRequested)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        string filePath = folderPath + $"\\{GetHouse(i)}.txt";
                        if (File.Exists(filePath))
                        {
                            string num;
                            using (StreamReader file = new StreamReader(filePath))
                            {
                                num = file.ReadLine();
                            }
                            File.Delete(filePath);
                            await WinTasks.Task.Run(() => Start(GetHouse(i), int.Parse(num), _cts.Token));
                            i--; // check errors in extra generation
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
                throw e.InnerException;
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
        public static void CancelGeneration()
        {
            if (Processing)
            {
                IsCancelled = true;
                _cts?.Cancel();
            }
        }

        /// <summary>
        /// Get house number
        /// </summary>
        /// <param name="number">Value from 0 to 5</param>
        /// <returns>House number</returns>
        private static string GetHouse(int number) => number switch
        {
            0 => "20_1",
            1 => "24_2",
            2 => "22_2",
            3 => "26_1",
            4 => "26_2",
            5 => "20_2",
            _ => throw new ArgumentException("Некорректный номер дома")
        };

        /// <summary>
        /// Generation flyers of house
        /// </summary>
        /// <param name="house">House number</param>
        /// <param name="startNum">Flat number to start generation process</param>
        /// <param name="token">Token to cancel</param>
        private static int Start(string house, int startNum, CancellationToken token = default)
        {
            GoogleSheets googleSheets = new GoogleSheets();
            IList<IList<object>> info = googleSheets.GetHouseInfo(house);
            IList<object> rates = googleSheets.GetRates(house);

            Word word = new Word();

            string folderPath = Environment.CurrentDirectory + $"\\Листовки за {Date.GetNamePrevMonth()}";
            string filePath = folderPath + $"\\{house}.docx";
            if (startNum == 0)
            {
                word.CopyDocument(Environment.CurrentDirectory + $"\\Resources\\FlyerTemplate.docx", filePath);
            }

            string logFilePath = folderPath + $"\\{house}.txt";
            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            var wordDoc = word.OpenDocument(filePath);

            int countNum = house == "24_2" ? 97 : 96;
            int generatedInHouse = 0;
            if (startNum == 0)
            {
                wordDoc.Tables[1].Range.Cut();
            }
            try
            {
                for (int i = startNum; i < countNum; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        return i;
                    }
                    word.FormationFlayer(wordDoc, info[i], rates, house);
                    generatedInHouse++;
                    Generated++;
                }
                return generatedInHouse;
            }
            catch (Exception)
            {
                using var swError = new StreamWriter(folderPath + $"\\{house}.txt", false);
                swError.WriteLine(generatedInHouse);
                return generatedInHouse;
            }
            finally
            {
                word.Save(wordDoc);
                word.CloseDocument(wordDoc);
                word.Quit();
            }
        }

        #endregion
    }
}
