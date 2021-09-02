using Models;
using System;
using System.Collections.Generic;

namespace Desktop.Extensions
{
    internal static class Report2Extensions
    {
        internal static void TransitionToNewMonth(this Report2 report)
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
            report.WerRepair = 0.0;
            report.WerCash = 0.0;
            report.WerBank = 0.0;
        }

        internal static (List<object> heating, List<object> wer) GetObjects(this Report2 report, int row = -1)
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
                report.HeatingBank,
                report.HeatingCash, 
                row == -1 ? report.HeatingEndDebit.ToString() : $"=ЕСЛИ(M{row}-N{row}-O{row}>0; M{row}-N{row}-O{row};0)",
                row == -1 ? report.HeatingEndCredit.ToString() : $"=ЕСЛИ(M{row}-N{row}-O{row}>0; 0;(M{row}-N{row}-O{row})*(-1))"
            };

            List<object> wer = new List<object>()
            {
                report.WerStartDebit,
                report.WerStartCredit,
                report.WerSquare, 
                row == -1 ? report.WerForMonth.ToString() : row < 15 ? $"=G$3*F{row}" : $"=G$6*F{row}",
                row == -1 ? report.RepairForMonth.ToString() : $"=H$6*F{row}",
                report.LivingPersons,
                row == -1 ? report.GarbageForMonth.ToString() : $"=J$6*I{row}",
                report.WaterCurrentValue,
                report.WaterPreviousValue, 
                row == -1 ? report.WaterValue.ToString() : $"=K{row}-L{row}", 
                row == -1 ? report.WaterForMonth.ToString() : $"=N$6*M{row}",
                row == -1 ? report.WerWaterForService.ToString() : $"=G{row}+H{row}+J{row}+N{row}", 
                report.WerPreviliges, 
                row == -1 ? report.WerTotal.ToString() : $"=D{row}+O{row}-E{row}-P{row}",
                report.WerRepair,
                report.WerBank,
                report.WerCash,
                row == -1 ? report.WerEndDebit.ToString() : $"=ЕСЛИ(Q{row}-R{row}-S{row}-T{row}>0; Q{row}-R{row}-S{row}-T{row};0)", 
                row == -1 ? report.WerEndCredit.ToString() : $"=ЕСЛИ(Q{row}-R{row}-S{row}-T{row}>0; 0;(Q{row}-R{row}-S{row}-T{row})*(-1))"
            };
            return (heating, wer);
        }

        private static string GetFormula(this Report2 report, int row) =>
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
