using System;

namespace GoogleLib.Tools
{
    public static class Date
    {
        public static string GetPrevDate()
        {
            return $"{GetNamePrevMonth()} {DateTime.Now.Year}";
        }

        public static string GetNameCurMonth()
        {
            return GetNameMonth(DateTime.Now.Month);
        }

        public static string GetNamePrevMonth()
        {
            return GetNameMonth(DateTime.Now.AddMonths(-1).Month);
        }

        public static int GetNumMonth(string month) => month switch
        {
            "январь" => 1,
            "ферваль" => 2,
            "март" => 3,
            "апрель" => 4,
            "май" => 5,
            "июнь" => 6,
            "июль" => 7,
            "август" => 8,
            "сентябрь" => 9,
            "октябрь" => 10,
            "ноябрь" => 11,
            "декабрь" => 12,
            _ => throw new ArgumentException("Недопустимый месяц")
        };

        public static string GetNameMonth(int month) => month switch
        {
            1 => "январь",
            2 => "ферваль",
            3 => "март",
            4 => "апрель",
            5 => "май",
            6 => "июнь",
            7 => "июль",
            8 =>"август",
            9 => "сентябрь",
            10 => "октябрь",
            11 => "ноябрь",
            12 => "декабрь",
            _ => throw new ArgumentException("Недопустимый месяц")
        };
    }
}
