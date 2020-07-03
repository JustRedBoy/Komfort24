using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using Range = Microsoft.Office.Interop.Word.Range;

namespace GenerateFlyers
{
    class Program
    {
        async static System.Threading.Tasks.Task Main(string[] args)
        {
            if (args == null || args.Length != 2) return;

            GoogleSheets googleSheets = new GoogleSheets();
            IList<IList<object>> info = await googleSheets.GetHouseInfoAsync(args[0]);
            IList<object> rates = await googleSheets.GetRatesAsync(args[0]);

            var wordApp = new Application
            {
                Visible = false
            };

            Document doc = wordApp.Documents.Open(Environment.CurrentDirectory + $"\\FlyerTemplate.docx");
            doc.SaveAs(Environment.CurrentDirectory + $"\\{args[0]}.docx");
            doc.Close();

            Document wordDoc = wordApp.Documents.Open(Environment.CurrentDirectory + $"\\{args[0]}.docx");
            using var pipeWrite = new AnonymousPipeClientStream(PipeDirection.Out, args[1]);
            using var sw = new StreamWriter(pipeWrite)
            {
                AutoFlush = true
            };

            int count = args[0] == "24_2" ? 97 : 96;
            wordDoc.Tables[1].Range.Cut();

            for (int i = 0; i < count; i++)
            {
                Paste(wordApp, wordDoc);
                WordReplace(wordDoc, "{NM}", info[i][2]);
                WordReplace(wordDoc, "{AD}", info[i][1]);
                WordReplace(wordDoc, "{MT}", rates[6]);
                WordReplace(wordDoc, "{FA}", $"ул. Пишоновская, {args[0].Replace('_', '/')} кв. {info[i][0]}");
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
                WordReplace(wordDoc, "{TT}", "");
                sw.WriteLine(args[0]);
            }
            wordDoc.Save();
            wordDoc.Close();
            wordApp.Quit();
        }

        private static void Paste(Application app, Document doc)
        {
            Range rng = doc.Content;
            rng.SetRange(rng.Start, rng.Start);
            rng.Select();
            app.Selection.Paste();
        }

        private static void WordReplace(Document doc, string replace, object replaceWith)
        {
            doc.Content.Find.Execute(FindText: replace, ReplaceWith: replaceWith);
        }

        private static double GetNumberValue(object obj)
        {
            return string.IsNullOrEmpty(obj.ToString()) ? 0.0 : double.Parse(obj.ToString());
        }
    }
}
