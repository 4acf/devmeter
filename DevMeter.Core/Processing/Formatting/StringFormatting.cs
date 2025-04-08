using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMeter.Core.Processing.Formatting
{
    public static class StringFormatting
    {

        public static string CommaString(int x)
        {
            //this function exists because i find calling it much more readable than seeing this everywhere
            return $"{x:n0}";
        }

        public static string DivideStrings(string dividendString, string divisorString)
        {

            dividendString = dividendString.Replace(",", "");
            divisorString = divisorString.Replace(",", "");

            if (!int.TryParse(dividendString, out var dividend))
            {
                return string.Empty;
            }

            if (!int.TryParse(divisorString, out var divisor))
            {
                return string.Empty;
            }

            int quotient = dividend / divisor;

            return string.Format($"{quotient:n0}");

        }
    }
}
