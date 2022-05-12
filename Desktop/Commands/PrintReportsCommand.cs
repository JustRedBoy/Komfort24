using System;
using System.Collections.Generic;
using Desktop.Tools;
using System.IO;
using Desktop.Models;
using Models;
using System.Linq;

namespace Desktop.Commands
{
    internal static class PrintReportsCommand
    {
        internal static bool Processing { get; set; } = false;

        /// <summary>
        /// Starting the reports printing process
        /// </summary>
        /// <param name="reports">Reports to print</param>
        internal static void Print(IEnumerable<ArchiveReport2> reports) 
        {
            if (reports == null || reports.Count() <= 0) return;
            Processing = true;

            List<PrintReport> printReports = new List<PrintReport>();

            int countRecords = reports.Count();
            countRecords = countRecords < 36 ? countRecords : 36;

            for (int i = 0; i < countRecords; i++)
            {
                printReports.Add(new PrintReport(reports.ElementAt(i)));
            }

            Word word = new Word();
            var document = word.CreateReportsDocument();
            try
            {
                word.FormationReportsDocument(document, printReports);
                word.SaveDocumentAs(document, Environment.CurrentDirectory + "\\PrintReports.docx");
                word.PrintDocument(document);
            }
            finally
            {
                word.CloseDocument(document);
                word.Quit();

                File.Delete(Environment.CurrentDirectory + "\\PrintReports.docx");
                Processing = false;
            }
        }
    }
}
