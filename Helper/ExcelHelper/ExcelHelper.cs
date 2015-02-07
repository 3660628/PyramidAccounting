using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Helper.ExcelHelper
{
    class ExcelHelper
    {
        /// <summary>
        /// 将string类型的金额改成倒序List数组
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public List<string> TransMoney(string parm)
        {
            List<string> result = new List<string>();
            string m1 = string.Empty;
            if (parm.IndexOf('.') > 0)
            {
                m1 = parm.Split('.')[0];
                string m2 = parm.Split('.')[1];
                result.Add((m2.Length == 2) ? m2.Substring(1, 1) : "0");
                result.Add(m2.Substring(0, 1));
            }
            else
            {
                m1 = parm;
                result.Add("0");
                result.Add("0");
            }
            for (int i = m1.Length - 1; i >= 0; i--)
            {
                result.Add(m1.Substring(i, 1));
            }
            return result;
        }
    }
}
