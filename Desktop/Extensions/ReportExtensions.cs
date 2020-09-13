using Models;
using System;
using System.Collections.Generic;

namespace Desktop.Extensions
{
    internal static class ReportExtensions
    {
        internal static void TransitionToNewMonth(this Report report)
        {
            report.HeatingStartDebit = report.HeatingEndDebit;
            report.HeatingStartCredit = report.HeatingEndCredit;
            report.HeatingPreviousValue = report.HeatingCurrentValue;
            report.HeatingPreviliges = 0.0;
            report.HeatingCash = 0.0;
            report.HeatingBank = 0.0;

            report.WerStartDebit = report.WerEndDebit;
            report.WerStartCredit = report.WerEndCredit;
            report.WaterPreviousValue = report.WaterCurrentValue;
            report.WerPreviliges = 0.0;
            report.WerCash = 0.0;
            report.WerBank = 0.0;
        }

        internal static (List<object> heating, List<object> wer) GetObjects(this Report report, int row = -1)
        {
            List<object> heating = new List<object>
            {
                report.HeatingStartDebit, 
                report.HeatingStartCredit, 
                report.HeatingSquare, 
                report.HeatingType,
                report.HeatingCurrentValue,
                report.HeatingPreviousValue, 
                row == -1 ? report.HeatingValue.ToString() : $"=H{row}-I{row}", 
                row == -1 ? report.HeatingForService.ToString() : report.GetFormula(row),
                report.HeatingPreviliges, 
                row == -1 ? report.HeatingTotal.ToString() : $"=D{row}+K{row}-E{row}-L{row}", 
                report.HeatingCash, 
                report.HeatingBank, 
                row == -1 ? report.HeatingEndDebit.ToString() : $"=ЕСЛИ(M{row}-N{row}-O{row}>0; M{row}-N{row}-O{row};0)",
                row == -1 ? report.HeatingEndCredit.ToString() : $"=ЕСЛИ(M{row}-N{row}-O{row}>0; 0;(M{row}-N{row}-O{row})*(-1))"
            };

            List<object> wer = new List<object>()
            {
                report.WerStartDebit,
                report.WerStartCredit,
                report.WerSquare, 
                row == -1 ? report.WerForMonth.ToString() : row < 15 ? $"=G$3*F{row}" : $"=G$6*F{row}",
                report.WaterCurrentValue,
                report.WaterPreviousValue, row == -1 ? report.WaterValue.ToString() : $"=H{row}-I{row}", 
                row == -1 ? report.WaterForMonth.ToString() : $"=K$6*J{row}",
                row == -1 ? report.WerWaterForService.ToString() : $"=K{row}+G{row}", 
                report.WerPreviliges, 
                row == -1 ? report.WerTotal.ToString() : $"=D{row}+L{row}-E{row}-M{row}",
                report.WerCash,
                report.WerBank,
                row == -1 ? report.WerEndDebit.ToString() : $"=ЕСЛИ(N{row}-O{row}-P{row}>0; N{row}-O{row}-P{row};0)", 
                row == -1 ? report.WerEndCredit.ToString() : $"=ЕСЛИ(N{row}-O{row}-P{row}>0; 0;(N{row}-O{row}-P{row})*(-1))"
            };
            return (heating, wer);
        }

        private static string GetFormula(this Report report, int row) =>
            report.HeatingType.ToLower() switch
        {
            "гкал" => $"=J{row}*K$3*1,1",
            "гдж" => $"=J{row}*K$3*1,1/4,187",
            "мвт" => $"=J{row}*K$3*0,86*1,1",
            "квт" => $"=J{row}*K$3*1,1/1162,2",
            "" => $"=F{row}*K$6",
            _ => throw new ArgumentException("Неправильный тип счётчика")
        };
    }
}
