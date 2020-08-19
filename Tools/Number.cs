using System;

namespace Tools
{
    public static class Number
    {
        /// <summary>
        /// Convert object to double
        /// </summary>
        /// <param name="obj">Convertible object</param>
        /// <returns>Double value</returns>
        public static double GetDouble(object obj, int digits = 2)
        {
            return string.IsNullOrEmpty(obj.ToString()) ? 0.0 : 
                Math.Round(double.Parse(obj.ToString()), digits);
        }

        /// <summary>
        /// Convert object to int
        /// </summary>
        /// <param name="obj">Convertible object</param>
        /// <returns>Int value</returns>
        public static int GetInt(object obj)
        {
            return string.IsNullOrEmpty(obj.ToString()) ? 0 : int.Parse(obj.ToString());
        }
    }
}
