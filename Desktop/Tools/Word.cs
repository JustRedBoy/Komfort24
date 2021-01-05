using Microsoft.Office.Interop.Word;
using Tools.Extensions;
using System;
using System.Collections.Generic;
using Desktop.Models;
using Models;
using Tools;

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
            content.Text = "\nБухгалтер ЧП СК Комфорт Одесса              Крепак Н.В.";
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

        internal void FormationFlayer(Document doc, Account account, Rates rates)
        {
            Paste(doc);
            WordReplace(doc, "{NM}", account.Owner);
            WordReplace(doc, "{AD}", account.AccountId);
            WordReplace(doc, "{YR}", DateTime.Now.Year);
            WordReplace(doc, "{MT}", Date.GetNameCurMonth());
            WordReplace(doc, "{FA}", account.House.FullAdress + $" кв. {account.FlatNumber}");
            WordReplace(doc, "{MS}", Date.GetShortMonth(DateTime.Now.Month));
            WordReplace(doc, "{ME}", Date.GetShortMonth(DateTime.Now.AddMonths(1).Month));
            WordReplace(doc, "{YS}", Date.GetShortYear(DateTime.Now.Year));
            WordReplace(doc, "{YE}", Date.GetShortYear(DateTime.Now.AddMonths(1).Year));
            var currentReport = account.CurrentReport;
            WordReplace(doc, "{HSS}", Math.Round(currentReport.HeatingStartDebit - currentReport.HeatingStartCredit, 2)); // debet - credit
            WordReplace(doc, "{CHV}", string.IsNullOrEmpty(currentReport.HeatingType) ? "-" : currentReport.HeatingCurrentValue.ToString()); // - or value
            WordReplace(doc, "{PHV}", string.IsNullOrEmpty(currentReport.HeatingType) ? "-" : currentReport.HeatingPreviousValue.ToString()); // - or value
            WordReplace(doc, "{HV}", string.IsNullOrEmpty(currentReport.HeatingType) ? "-" : currentReport.HeatingValue.ToString()); // - or value
            WordReplace(doc, "{HR}", string.IsNullOrEmpty(currentReport.HeatingType) ? rates.CentralHeatingRate : rates.CustomHeatingRate); // central or custom
            WordReplace(doc, "{FH}", Math.Round(currentReport.HeatingForService - currentReport.HeatingPreviliges, 2)); // forHeating - privileges
            WordReplace(doc, "{HP}", Math.Round(currentReport.HeatingCash + currentReport.HeatingBank, 2)); // cashbox + bank
            WordReplace(doc, "{HSE}", Math.Round(currentReport.HeatingEndDebit - currentReport.HeatingEndCredit, 2)); // debet - credit
            WordReplace(doc, "{WRSS}", Math.Round(currentReport.WerStartDebit - currentReport.WerStartCredit, 2)); // debet - credit
            string flatNumber = account.FlatNumber.Contains('/') ? account.FlatNumber[0..^2] : account.FlatNumber;
            WordReplace(doc, "{WRR}", flatNumber.ToInt() < 7 ? rates.SpecialWerRate : rates.GeneralWerRate); // special or general
            WordReplace(doc, "{FWR}", Math.Round(currentReport.WerForMonth - currentReport.WerPreviliges, 2)); // forWer - privileges
            WordReplace(doc, "{WRP}", Math.Round(currentReport.WerCash + currentReport.WerBank - currentReport.WaterForMonth, 2)); // cashbox + bank - forWater
            WordReplace(doc, "{WRSE}", Math.Round(currentReport.WerEndDebit - currentReport.WerEndCredit, 2)); // debet - credit
            WordReplace(doc, "{CWV}", currentReport.WaterCurrentValue); // current water value
            WordReplace(doc, "{PWV}", currentReport.WaterPreviousValue); // prev water value
            WordReplace(doc, "{WV}", currentReport.WaterValue);  // water value
            WordReplace(doc, "{WTR}", rates.WaterRate); // water rate
            WordReplace(doc, "{FWT}", currentReport.WaterForMonth); // for water
            WordReplace(doc, "{WTP}", currentReport.WaterForMonth); // water payment
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

            table.Cell(1, 1).Width = 43f;
            table.Cell(1, 2).Width = 45f;
            table.Cell(1, 3).Width = 55f;
            table.Cell(1, 4).Width = 45f;
            table.Cell(1, 5).Width = 45f;
            table.Cell(1, 6).Width = 45f;
            table.Cell(1, 7).Width = 45f;
            table.Cell(1, 8).Width = 50f;
            table.Cell(1, 9).Width = 45f;
            table.Cell(1, 10).Width = 45f;
            table.Cell(1, 11).Width = 55f;

            table.Cell(1, 1).Range.Text = "{DT}";
            table.Cell(1, 2).Range.Text = "{HSS}";
            table.Cell(1, 3).Range.Text = "{WSS}";
            table.Cell(1, 4).Range.Text = "{FH}";
            table.Cell(1, 5).Range.Text = "{FWR}";
            table.Cell(1, 6).Range.Text = "{FWT}";
            table.Cell(1, 7).Range.Text = "{HP}";
            table.Cell(1, 8).Range.Text = "{WRP}";
            table.Cell(1, 9).Range.Text = "{WTP}";
            table.Cell(1, 10).Range.Text = "{HSE}";
            table.Cell(1, 11).Range.Text = "{WSE}";

            for (int i = 1; i <= 11; i++)
            {
                table.Cell(1, i).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            }

            table.Rows.Add(table.Rows[1]);
            table.Rows[1].Range.Font.Bold = -1;
            table.Cell(1, 1).Range.Text = "Месяц";
            table.Cell(1, 2).Range.Text = "Отопл. итого на начало месяца";
            table.Cell(1, 3).Range.Text = "Кв-та итого на начало месяца";
            table.Cell(1, 4).Range.Text = "Начисл. за отопл.";
            table.Cell(1, 5).Range.Text = "Начисл. за кв-ту";
            table.Cell(1, 6).Range.Text = "Начисл. за воду";
            table.Cell(1, 7).Range.Text = "Оплата отопл.";
            table.Cell(1, 8).Range.Text = "Оплата кв-та";
            table.Cell(1, 9).Range.Text = "Оплата холод. вода";
            table.Cell(1, 10).Range.Text = "Отопл. итого к оплате";
            table.Cell(1, 11).Range.Text = "Кв-та итого к оплате";

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
