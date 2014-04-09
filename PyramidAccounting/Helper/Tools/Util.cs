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

        /// <summary>
        /// 金额转换算法
        /// </summary>
        /// <param name="value">转换数</param>
        /// <param name="size">位数</param>
        /// <returns>List</returns>
        public List<string> Turn(string value, int size)
        {
            List<string> list = new List<string>();
            int length = value.Length;
            if (value.Equals("0"))
            {
                for (int i = 0; i < size; i++)
                {
                    list.Add(string.Empty);
                }
                return list;
            }
            value = value.Replace(".", "");
            string s = string.Empty;
            char[] cc = value.ToCharArray();

            if (value.IndexOf(".") > 0)
            {
                for (int j = 0; j < size - length; j++)
                {
                    list.Add(s);
                }
                for (int i = 0; i < length; i++)
                {
                    s = cc[i].ToString();
                    list.Add(s);
                }
            }
            else
            {
                for (int j = 0; j < size - 2 - length; j++)
                {
                    list.Add(s);
                }
                for (int i = 0; i < length; i++)
                {
                    s = cc[i].ToString();
                    list.Add(s);
                }
                list.Add("0");
                list.Add("0");
            }
            return list;
        }

    }
}
