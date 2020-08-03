using GoogleLib;
using GoogleLib.Tools;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Range = Microsoft.Office.Interop.Word.Range;
using WinTasks = System.Threading.Tasks;

namespace Management.Commands
{
    public static class GenerationFlyers
    {
        #region Delegates and Events

        public delegate void GenerationHandler(int value);
        public static event GenerationHandler UpdateProgress;
        public static event GenerationHandler CompletedGeneration;

        #endregion

        #region Fileds and Properties

        public static bool Processing { get; set; } = false;
        public static bool IsCancelled { get; set; } = false;

        private static bool _completed = false;
        private static bool Completed 
        { 
            get { return _completed; }
            set 
            {
                _completed = value;
                if (_generated != 0)
                {
                    if (_completed)
                    {
                        CompletedGeneration?.Invoke(0);
                    }
                    else
                    {
                        CompletedGeneration?.Invoke(-1);
                    }
                }
            } 
        }

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
        public async static WinTasks.Task StartGenerationAsync()
        {
            _cts = new CancellationTokenSource();
            Completed = false;
            Processing = true;
            IsCancelled = false;

            string folderPath = Environment.CurrentDirectory + $"\\Листовки за {Date.GetNamePrevMonth()}";
            Directory.CreateDirectory(folderPath);

            //start generation in 6 tasks
            WinTasks.Task[] tasks = new WinTasks.Task[6];
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
                Completed = true;
            }
            else
            {
                Completed = false;
            }

            _cts?.Dispose();
            Processing = false;
            Generated = 0;
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
                _cts?.Dispose();
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
        private static void Start(string house, int startNum, CancellationToken token = default)
        {
            GoogleSheets googleSheets = new GoogleSheets();
            IList<IList<object>> info = googleSheets.GetHouseInfo(house);
            IList<object> rates = googleSheets.GetRates(house);

            var wordApp = new Application
            {
                Visible = false
            };

            string folderPath = Environment.CurrentDirectory + $"\\Листовки за {Date.GetNamePrevMonth()}";
            string filePath = folderPath + $"\\{house}.docx";
            if (startNum == 0)
            {
                Document docTemplate = wordApp.Documents.Open(Environment.CurrentDirectory + $"\\Resources\\FlyerTemplate.docx", ReadOnly: true);
                docTemplate.SaveAs(filePath);
                docTemplate.Close();
            }

            string logFilePath = folderPath + $"\\{house}.txt";
            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            Document wordDoc = wordApp.Documents.Open(filePath);

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
                        return;
                    }
                    Paste(wordApp, wordDoc);
                    WordReplace(wordDoc, "{NM}", info[i][2]);
                    WordReplace(wordDoc, "{AD}", info[i][1]);
                    WordReplace(wordDoc, "{MT}", rates[6]);
                    WordReplace(wordDoc, "{FA}", $"ул. Пишоновская, {house.Replace('_', '/')} кв. {info[i][0]}");
                    WordReplace(wordDoc, "{MS}", rates[7]);
                    WordReplace(wordDoc, "{ME}", rates[8]);
                    WordReplace(wordDoc, "{HSS}", Math.Round(GetNumberValue(info[i][3]) - GetNumberValue(info[i][4]), 2)); // debet - credit
                    WordReplace(wordDoc, "{CHV}", string.IsNullOrEmpty(info[i][6].ToString()) ? "-" : GetNumberValue(info[i][7]).ToString()); // - or value
                    WordReplace(wordDoc, "{PHV}", string.IsNullOrEmpty(info[i][6].ToString()) ? "-" : GetNumberValue(info[i][8]).ToString()); // - or value
                    WordReplace(wordDoc, "{HV}", string.IsNullOrEmpty(info[i][6].ToString()) ? "-" : GetNumberValue(info[i][9]).ToString()); // - or value
                    WordReplace(wordDoc, "{HR}", string.IsNullOrEmpty(info[i][6].ToString()) ? rates[1] : rates[2]); // central or custom
                    WordReplace(wordDoc, "{FH}", Math.Round(GetNumberValue(info[i][10]) - GetNumberValue(info[i][11]), 2)); // forHeating - privileges
                    WordReplace(wordDoc, "{HP}", Math.Round(GetNumberValue(info[i][13]) + GetNumberValue(info[i][14]), 2)); // cashbox + bank
                    WordReplace(wordDoc, "{HSE}", Math.Round(GetNumberValue(info[i][15]) - GetNumberValue(info[i][16]), 2)); // debet - credit
                    WordReplace(wordDoc, "{WRSS}", Math.Round(GetNumberValue(info[i][18]) - GetNumberValue(info[i][19]), 2)); // debet - credit
                    WordReplace(wordDoc, "{WRR}", i < 6 ? rates[4] : rates[3]); // special or general
                    WordReplace(wordDoc, "{FWR}", Math.Round(GetNumberValue(info[i][21]) - GetNumberValue(info[i][27]), 2)); // forWer - privilege
                    WordReplace(wordDoc, "{WRP}", Math.Round(GetNumberValue(info[i][29]) + GetNumberValue(info[i][30]) - GetNumberValue(info[i][25]), 2)); // cashbox + bank - forWater
                    WordReplace(wordDoc, "{WRSE}", Math.Round(GetNumberValue(info[i][31]) - GetNumberValue(info[i][32]), 2)); // debet - credit
                    WordReplace(wordDoc, "{CWV}", GetNumberValue(info[i][22])); // current water value
                    WordReplace(wordDoc, "{PWV}", GetNumberValue(info[i][23])); // prev water value
                    WordReplace(wordDoc, "{WV}", GetNumberValue(info[i][24]));  // water value
                    WordReplace(wordDoc, "{WTR}", rates[0]); // water rate
                    WordReplace(wordDoc, "{FWT}", GetNumberValue(info[i][25])); // for water
                    WordReplace(wordDoc, "{WTP}", GetNumberValue(info[i][25])); // water payment
                    WordReplace(wordDoc, "{GSS}", "-");
                    WordReplace(wordDoc, "{GR}", "-");
                    WordReplace(wordDoc, "{FG}", "-");
                    WordReplace(wordDoc, "{GP}", "-");
                    WordReplace(wordDoc, "{GSE}", "-");
                    generatedInHouse++;
                    Generated++;
                }
            }
            catch (Exception)
            {
                using var swError = new StreamWriter(folderPath + $"\\{house}.txt", false);
                swError.WriteLine(generatedInHouse);
            }
            finally
            {
                wordDoc.Save();
                wordDoc.Close();
                wordApp.Quit();
            }
        }

        /// <summary>
        /// Paste table to word document
        /// </summary>
        /// <param name="app">Word application</param>
        /// <param name="doc">Word document</param>
        private static void Paste(Application app, Document doc)
        {
            Range rng = doc.Content;
            rng.SetRange(rng.Start, rng.Start);
            rng.Select();
            app.Selection.Paste();
        }

        /// <summary>
        /// Replace template string
        /// </summary>
        /// <param name="doc">Word document</param>
        /// <param name="replace">Template string</param>
        /// <param name="replaceWith">Text to replace</param>
        private static void WordReplace(Document doc, string replace, object replaceWith)
        {
            doc.Content.Find.Execute(FindText: replace, ReplaceWith: replaceWith);
        }

        /// <summary>
        /// Convert object to double
        /// </summary>
        /// <param name="obj">Convertible object</param>
        /// <returns></returns>
        private static double GetNumberValue(object obj)
        {
            return string.IsNullOrEmpty(obj.ToString()) ? 0.0 : double.Parse(obj.ToString());
        }

        #endregion
    }
}
