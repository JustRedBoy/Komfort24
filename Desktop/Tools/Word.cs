using Microsoft.Office.Interop.Word;
using Tools;
using System;
using System.Collections.Generic;
using Desktop.Models;

namespace Desktop.Tools
{
    /// <summary>
    /// Class for working with Microsoft Word
    /// </summary>
    internal class Word
    {
        internal Application Application { get; }

        internal Word()
        {
            Application = new Application
            {
                Visible = false
            };
        }

        internal Document CreateReportsDocument()
        {
            Document doc = CreateDocument();

            doc.Content.Font.Size = 13f;
            doc.Content.Font.Name = "Calibri";
            return doc;
        }
        internal Document CreateDocument()
        {
            return Application.Documents.Add();
        }
        internal Document OpenDocument(string path, bool isReadonly = false)
        {
            return Application.Documents.Open(path, ReadOnly: isReadonly);
        }
        internal void SaveDocumentAs(Document doc, string path)
        {
            doc.SaveAs(path);
        }
        internal void SaveDocument(Document doc)
        {
            doc.Save();
        }
        internal void CopyDocument(string sourceFile, string copiedFile)
        {
            Document doc = Application.Documents.Open(sourceFile, ReadOnly: true);
            doc.SaveAs(copiedFile);
            doc.Close();
        }
        internal void PrintDocument(Document doc)
        {
            doc.PrintOut();
        }
        internal void CloseDocument(Document doc)
        {
            doc.Close();
        }
        internal void Quit()
        {
            Application.Quit();
        }
        internal void FormationReportsDocument(Document doc, List<PrintReport> printReports)
        {
            CreateTemplateReportsTable(doc);

            string name = string.IsNullOrEmpty(printReports[0].Owner) ?
                "\"Имя владельца\"" : printReports[0].Owner;
            doc.Tables[1].Cell(1, 1).Range.Text =
                $"Платежи для {name}  (лицевой счет: {printReports[0].AccountId})";

            doc.Tables[1].Rows[3].Range.Cut();
            foreach (var report in printReports)
            {
                FormingReport(doc, report);
            }

            Microsoft.Office.Interop.Word.Range content = doc.Content;
            content.SetRange(content.End, content.End);
            content.Text = "\n\nБухгалтер ЧП СК Комфорт Одесса              Крепак Н.В.";
        }
        private void FormingReport(Document doc, PrintReport printReport)
        {
            Paste(doc, false);
            WordReplace(doc, "{HSS}", printReport.HeatingStateStart);
            WordReplace(doc, "{WSS}", printReport.WerStateStart);
            WordReplace(doc, "{FH}", printReport.ForHeating);
            WordReplace(doc, "{FWR}", printReport.ForWer);
            WordReplace(doc, "{FWT}", printReport.ForWater);
            WordReplace(doc, "{HP}", printReport.HeatingPayment);
            WordReplace(doc, "{WRP}", printReport.WerPayment);
            WordReplace(doc, "{WTP}", printReport.WaterPayment);
            WordReplace(doc, "{HSE}", printReport.HeatingStateEnd);
            WordReplace(doc, "{WSE}", printReport.WerStateEnd);
            WordReplace(doc, "{DT}", printReport.MonthAndYear);
        }
        internal void FormationFlayer(Document doc, IList<object> info, IList<object> rates, string house)
        {
            Paste(doc);
            WordReplace(doc, "{NM}", info[2]);
            WordReplace(doc, "{AD}", info[1]);
            WordReplace(doc, "{MT}", rates[6]);
            WordReplace(doc, "{FA}", $"ул. Пишоновская, {house} кв. {info[0]}");
            WordReplace(doc, "{MS}", Date.GetShortMonth(DateTime.Now.AddMonths(-1).Month));
            WordReplace(doc, "{ME}", Date.GetShortMonth(DateTime.Now.Month));
            WordReplace(doc, "{YS}", Date.GetShortYear(DateTime.Now.AddMonths(-1).Year));
            WordReplace(doc, "{YE}", Date.GetShortYear(DateTime.Now.Year));
            WordReplace(doc, "{HSS}", Math.Round(info[3].ToDouble() - info[4].ToDouble(), 2)); // debet - credit
            WordReplace(doc, "{CHV}", string.IsNullOrEmpty(info[6].ToString()) ? "-" : info[7].ToString()); // - or value
            WordReplace(doc, "{PHV}", string.IsNullOrEmpty(info[6].ToString()) ? "-" : info[8].ToString()); // - or value
            WordReplace(doc, "{HV}", string.IsNullOrEmpty(info[6].ToString()) ? "-" : info[9].ToString()); // - or value
            WordReplace(doc, "{HR}", string.IsNullOrEmpty(info[6].ToString()) ? rates[1] : rates[2]); // central or custom
            WordReplace(doc, "{FH}", Math.Round(info[10].ToDouble() - info[11].ToDouble(), 2)); // forHeating - privileges
            WordReplace(doc, "{HP}", Math.Round(info[13].ToDouble() + info[14].ToDouble(), 2)); // cashbox + bank
            WordReplace(doc, "{HSE}", Math.Round(info[15].ToDouble() - info[16].ToDouble(), 2)); // debet - credit
            WordReplace(doc, "{WRSS}", Math.Round(info[17].ToDouble() - info[18].ToDouble(), 2)); // debet - credit
            string flatNumber = info[0].ToString();
            WordReplace(doc, "{WRR}", (flatNumber.Contains('/') ? flatNumber[0..^2] : flatNumber).ToInt() < 7 ? rates[4] : rates[3]); // special or general
            WordReplace(doc, "{FWR}", Math.Round(info[20].ToDouble() - info[26].ToDouble(), 2)); // forWer - privileges
            WordReplace(doc, "{WRP}", Math.Round(info[28].ToDouble() + info[29].ToDouble() - info[24].ToDouble(), 2)); // cashbox + bank - forWater
            WordReplace(doc, "{WRSE}", Math.Round(info[30].ToDouble() - info[31].ToDouble(), 2)); // debet - credit
            WordReplace(doc, "{CWV}", info[21].ToDouble()); // current water value
            WordReplace(doc, "{PWV}", info[22].ToDouble()); // prev water value
            WordReplace(doc, "{WV}", info[23].ToDouble());  // water value
            WordReplace(doc, "{WTR}", rates[0]); // water rate
            WordReplace(doc, "{FWT}", info[24].ToDouble()); // for water
            WordReplace(doc, "{WTP}", info[24].ToDouble()); // water payment
            WordReplace(doc, "{GSS}", "-");
            WordReplace(doc, "{GR}", "-");
            WordReplace(doc, "{FG}", "-");
            WordReplace(doc, "{GP}", "-");
            WordReplace(doc, "{GSE}", "-");
        }
        private void CreateTemplateReportsTable(Document doc)
        {
            doc.PageSetup.LeftMargin = 50f;
            Microsoft.Office.Interop.Word.Range tableRange = doc.Content;
            tableRange.SetRange(tableRange.Start, tableRange.Start);
            Table table = doc.Tables.Add(tableRange, 1, 11);

            table.Rows[1].Height = 16f;
            table.Range.Font.Size = 9f;
            table.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            table.Range.Paragraphs.SpaceAfter = 0f;
            table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
            table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

            table.Cell(1, 1).Width = 45f;
            table.Cell(1, 2).Width = 55f;
            table.Cell(1, 3).Width = 45f;
            table.Cell(1, 4).Width = 45f;
            table.Cell(1, 5).Width = 45f;
            table.Cell(1, 6).Width = 45f;
            table.Cell(1, 7).Width = 50f;
            table.Cell(1, 8).Width = 45f;
            table.Cell(1, 9).Width = 45f;
            table.Cell(1, 10).Width = 55f;
            table.Cell(1, 11).Width = 43f;

            table.Cell(1, 1).Range.Text = "{HSS}";
            table.Cell(1, 2).Range.Text = "{WSS}";
            table.Cell(1, 3).Range.Text = "{FH}";
            table.Cell(1, 4).Range.Text = "{FWR}";
            table.Cell(1, 5).Range.Text = "{FWT}";
            table.Cell(1, 6).Range.Text = "{HP}";
            table.Cell(1, 7).Range.Text = "{WRP}";
            table.Cell(1, 8).Range.Text = "{WTP}";
            table.Cell(1, 9).Range.Text = "{HSE}";
            table.Cell(1, 10).Range.Text = "{WSE}";
            table.Cell(1, 11).Range.Text = "{DT}";

            for (int i = 1; i <= 11; i++)
            {
                table.Cell(1, i).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            }

            table.Rows.Add(table.Rows[1]);
            table.Rows[1].Range.Font.Bold = -1;
            table.Cell(1, 1).Range.Text = "Долг1О";
            table.Cell(1, 2).Range.Text = "Долг1СД+В";
            table.Cell(1, 3).Range.Text = "ЗаО";
            table.Cell(1, 4).Range.Text = "ЗаСД";
            table.Cell(1, 5).Range.Text = "ЗаВ";
            table.Cell(1, 6).Range.Text = "ОплатаО";
            table.Cell(1, 7).Range.Text = "ОплатаСД";
            table.Cell(1, 8).Range.Text = "ОплатаВ";
            table.Cell(1, 9).Range.Text = "Долг2О";
            table.Cell(1, 10).Range.Text = "Долг2СД+В";
            table.Cell(1, 11).Range.Text = "Месяц";

            table.Rows.Add(table.Rows[1]);
            for (int i = 0; i < 10; i++)
            {
                table.Cell(1, 1).Merge(table.Cell(1, 2));
            }
        }
        private void Paste(Document doc, bool start = true)
        {
            Microsoft.Office.Interop.Word.Range range = doc.Content;
            if (start)
            {
                range.SetRange(range.Start, range.Start);
            }
            else
            {
                range.SetRange(range.End, range.End);
            }
            range.Select();
            Application.Selection.Paste();
        }
        private void WordReplace(Document doc, string replace, object replaceWith)
        {
            doc.Content.Find.Execute(FindText: replace, ReplaceWith: replaceWith);
        }
    }
}
