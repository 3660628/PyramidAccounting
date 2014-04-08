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
        PA.Helper.DataBase.DataBase db = new DataBase.DataBase();
        public bool IsNumber(String strNumber)
        {
            return Regex.IsMatch(strNumber, @"^[+-]?\d*$");
        }

        public string GetVersionType()
        {
            string sql = "select julianday('now')-julianday(OP_TIME),value from t_systeminfo where rkey='999'";
            if (db.GetSelectValue(sql).Equals(""))
            {
                return "试用版";
            }
            else
            {
                return "正式版";
            }
        }
    }
}
