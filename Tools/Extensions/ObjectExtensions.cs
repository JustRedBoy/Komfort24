using System;
using System.Globalization;

namespace Tools.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Convert object to double
        /// </summary>
        /// <param name="value">Convertible object</param>
        /// <param name="value">Вecimal places</param>
        /// <returns>Double value</returns>
        public static double ToDouble(this object value, int digits = 2)
        {
            return string.IsNullOrWhiteSpace(value.ToString()) ? 0.0 :
                Math.Round(double.Parse(GetStringWithoutSpaces(value.ToString()), 
                    CultureInfo.GetCultureInfo("ru-RU")), digits);
        }

        /// <summary>
        /// Convert object to int
        /// </summary>
        /// <param name="value">Convertible object</param>
        /// <returns>Int value</returns>
        public static int ToInt(this object value)
        {
            return string.IsNullOrWhiteSpace(value.ToString()) ? 0 : 
                int.Parse(GetStringWithoutSpaces(value.ToString()));
        }

        private static string GetStringWithoutSpaces(string source)
        {
            string result = "";
            for (int i = 0; i < source.Length; i++)
            {
                if(!char.IsWhiteSpace(source[i]))
                {
                    result += source[i];
                }
            }
            return result;
        }
    }
}
