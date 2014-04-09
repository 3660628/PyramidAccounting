using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PA.Helper.DataBase;

namespace PA.Helper.Tools
{
    class Util
    {
        public bool IsNumber(String strNumber)
        {
            return Regex.IsMatch(strNumber, @"^[+-]?\d*$");
        }

    }
}
