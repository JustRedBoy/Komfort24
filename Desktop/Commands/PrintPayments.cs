using System;
using System.Collections.Generic;
using Models;
using Desktop.Tools;
using System.IO;

namespace Desktop.Commands
{
    internal static class PrintPayments
    {
        internal static bool Processing { get; set; } = false;

        /// <summary>
        /// Starting the payments printing process
        /// </summary>
        /// <param name="payments">Payments to print</param>
        internal static void Print(List<Payment> payments) 
        {
            if (payments == null || payments.Count <= 0) return;
            Processing = true;

            Word word = new Word();
            var document = word.CreatePaymentsDocument();
            try
            {
                word.FormationPaymentsDocument(document, payments);
                word.SaveDocumentAs(document, Environment.CurrentDirectory + "\\PrintPayments.docx");
                word.PrintDocument(document);
            }
            finally
            {
                word.CloseDocument(document);
                word.Quit();

                File.Delete(Environment.CurrentDirectory + "\\PrintPayments.docx");
                Processing = false;
            }
        }
    }
}
