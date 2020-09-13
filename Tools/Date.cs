using System;

namespace Tools
{
    /// <summary>
    /// Class for working with date
    /// </summary>
    public static class Date
    {
        /// <summary>
        /// Get a string with the name of the previous month and year
        /// </summary>
        public static string GetFullPrevMonth()
        {
            return $"{GetNamePrevMonth()} {DateTime.Now.Year}";
        }

        /// <summary>
        /// Get a string with the name of the current month
        /// </summary>
        public static string GetNameCurMonth()
        {
            return GetNameMonth(DateTime.Now.Month);
        }

        /// <summary>
        /// Get a string with the name of the previous month
        /// </summary>
        public static string GetNamePrevMonth()
        {
            return GetNameMonth(DateTime.Now.AddMonths(-1).Month);
        }

        /// <summary>
        /// Get a number of the month
        /// <param name="monthName">String of month</param>
        /// </summary>
        public static int GetNumMonth(string monthName) => monthName switch
        {
            "январь" => 1,
            "февраль" => 2,
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

        /// <summary>
        /// Get a string with the name of the month
        /// <param name="monthNum">Number of month</param>
        /// </summary>
        public static string GetNameMonth(int monthNum) => monthNum switch
        {
            1 => "январь",
            2 => "февраль",
            3 => "март",
            4 => "апрель",
            5 => "май",
            6 => "июнь",
            7 => "июль",
            8 => "август",
            9 => "сентябрь",
            10 => "октябрь",
            11 => "ноябрь",
            12 => "декабрь",
            _ => throw new ArgumentException("Недопустимый месяц")
        };

        /// <summary>
        /// Get a string with the number of the month (2 digits)
        /// <param name="numMonth">Number of month to convert</param>
        /// </summary>
        public static string GetShortMonth(int numMonth)
        {
            if (numMonth < 0 || numMonth > 12)
            {
                throw new ArgumentException("Некорректный номер месяца");
            }

            if (numMonth < 10)
            {
                return "0" + numMonth;
            }
            else
            {
                return numMonth.ToString();
            }
        }

        /// <summary>
        /// Get 2 last digits of year
        /// </summary>
        /// <param name="year">Year to convert</param>
        public static string GetShortYear(int year)
        {
            return year.ToString().Substring(2);
        }
    }
}
