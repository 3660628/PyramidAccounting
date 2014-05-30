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
        public List<string> Turn(string value, int size, bool flag)
        {
            List<string> list = new List<string>();
            int length = 0;
            if (flag)
            {
                value = value.Replace("-", "");
            }
            if (value.Equals("0"))
            {
                for (int i = 0; i < size; i++)
                {
                    list.Add(string.Empty);
                }
                return list;
            }
            string s = string.Empty;

            if (value.IndexOf(".") > 0)
            {
                string temp = value.Split('.')[1];
                if (temp.Length == 1)
                {
                    value += "0";
                }
                else if (temp.Length > 2)
                {
                    temp = temp.Substring(0, 2);
                    value = value.Split('.')[0] + temp;
                }
                else if (value.StartsWith("0."))
                {
                    value = value.Split('.')[1];
                }
                value = value.Replace(".", "");
                length = value.Length;
                char[] cc = value.ToCharArray();
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
                length = value.Length;
                char[] dd = value.ToCharArray();
                for (int j = 0; j < size - 2 - length; j++)
                {
                    list.Add(s);
                }
                for (int i = 0; i < length; i++)
                {
                    s = dd[i].ToString();
                    list.Add(s);
                }
                list.Add("0");
                list.Add("0");
            }
            return list;
        }

        public List<string> Turn(string value, int size)
        {
            return Turn(value,size,true);
        }

    }
}
