using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devmeter.core.Processing.Formatting
{
    public static class StringFormatting
    {
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
