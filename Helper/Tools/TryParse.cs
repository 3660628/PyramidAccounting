using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Helper.Tools
{
    class TryParse
    {
        public static decimal DecimalParse(string input)
        {
            decimal result = 0m;
            decimal.TryParse(input, out result);
            return result;
        }
    }
}
