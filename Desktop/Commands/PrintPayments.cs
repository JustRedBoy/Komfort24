using Microsoft.Office.Interop.Word;
using Range = Microsoft.Office.Interop.Word.Range;
using System;
using System.Collections.Generic;
using Desktop.Models;

namespace Desktop.Commands
{
    public static class PrintPayments
    {
        public static bool Processing { get; set; } = false;

        public static void Print(List<Payment> payments) 
        {
            if (payments == null || payments.Count <= 0) return;
            Processing = true;
            var wordApp = new Application
            {
                Visible = false
            };

            Document doc = wordApp.Documents.Open(Environment.CurrentDirectory + $"\\Resources\\PrintPaymentsTemplate.docx");
            doc.Tables[1].Range.Cut();

            foreach (var payment in payments)
            {
                Paste(wordApp, doc);
                WordReplace(doc, "{FWR}", payment.ForWer);
                WordReplace(doc, "{FWTR}", payment.ForWater);
                WordReplace(doc, "{FH}", payment.ForHeating);
                WordReplace(doc, "{TT}", payment.Total);
                WordReplace(doc, "{MT}", payment.Month);
                WordReplace(doc, "{YR}", payment.Year);
            }

            doc.Tables[1].Rows.Add(doc.Tables[1].Rows[1]);
            doc.Tables[1].Rows[1].Range.Font.Bold = -1;
            doc.Tables[1].Rows[1].Cells[1].Range.Text = "За содержание дома";
            doc.Tables[1].Rows[1].Cells[2].Range.Text = "За воду";
            doc.Tables[1].Rows[1].Cells[3].Range.Text = "За отопление";
            doc.Tables[1].Rows[1].Cells[4].Range.Text = "Всего";
            doc.Tables[1].Rows[1].Cells[5].Range.Text = "Месяц";
            doc.Tables[1].Rows[1].Cells[6].Range.Text = "Год";

            doc.Tables[1].Rows.Add(doc.Tables[1].Rows[1]);
            doc.Tables[1].Rows[1].Cells[1].Merge(doc.Tables[1].Rows[1].Cells[2]);
            doc.Tables[1].Rows[1].Cells[1].Merge(doc.Tables[1].Rows[1].Cells[2]);
            doc.Tables[1].Rows[1].Cells[1].Merge(doc.Tables[1].Rows[1].Cells[2]);
            doc.Tables[1].Rows[1].Cells[1].Merge(doc.Tables[1].Rows[1].Cells[2]);
            doc.Tables[1].Rows[1].Cells[1].Merge(doc.Tables[1].Rows[1].Cells[2]);
            string name = string.IsNullOrEmpty(payments[0].FlatOwner) ?
                "\"Имя владельца\"" : payments[0].FlatOwner;
            doc.Tables[1].Rows[1].Cells[1].Range.Text = $"Платежи для {name}  (лицевой счет: {payments[0].AccountId})";

            Range rng = doc.Content;
            rng.SetRange(rng.End, rng.End);
            rng.Text = "\n\nБухгалтер ЧП СК Комфорт Одесса              Крепак Н.В.";
            doc.SaveAs(Environment.CurrentDirectory + "\\PrintPayments.docx");
            doc.PrintOut();

            doc.Close();
            wordApp.Quit();

            System.IO.File.Delete(Environment.CurrentDirectory + "\\PrintPayments.docx");
            Processing = false;
        }

        private static void Paste(Application app, Document doc)
        {
            Range rng = doc.Content;
            rng.SetRange(rng.End, rng.End);
            rng.Select();
            app.Selection.Paste();
        }

        private static void WordReplace(Document doc, string replace, object replaceWith)
        {
            doc.Content.Find.Execute(FindText: replace, ReplaceWith: replaceWith);
        }
    }
}
