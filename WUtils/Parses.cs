using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace WUtils
{
    /// <summary>
    /// Conversor de valores
    /// </summary>
    public static class Parses
    {
        /// <summary>
        /// Converte uma string para Double obedecendo a Cultura
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ToDouble(this string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    return 0;
                }

                var separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                var valueConverted = double.Parse(Regex.Replace(value.Trim(), "[.,]", separator));
            
                return valueConverted;
            }
            catch (Exception e)
            {
                //CoreLog.WriteLog($"Erro na conversão ToDouble: -> {value} <-\n\t{e}");
                return 0;
            }
            
        }





    }

}