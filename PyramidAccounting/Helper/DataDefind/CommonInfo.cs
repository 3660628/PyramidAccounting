using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Helper.DataDefind
{
    class CommonInfo
    {
        private static string username;

        public static string 用户名
        {
            get { return username; }
            set { username = value; }
        }

        private static string bookid;

        public static string 账薄号
        {
            get { return CommonInfo.bookid; }
            set { CommonInfo.bookid = value; }
        }

    }
}
