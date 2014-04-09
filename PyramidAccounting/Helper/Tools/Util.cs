using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PA.Helper.Tools
{
    class Util
    {
        UsbController usb = new UsbController();

        public bool IsNumber(String strNumber)
        {
            return Regex.IsMatch(strNumber, @"^[+-]?\d*$");
        }



    }
}
