using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Helper.DataDefind
{
    class CommonInfo
    {
        private static string username;
        private static string realname;
        private static string authority;
        private static int authority_value;
        private static int system_index;

        public static int 权限值
        {
            get { return CommonInfo.authority_value; }
            set { CommonInfo.authority_value = value; }
        }
        /// <summary>
        /// 012345 按会计制度下拉框索引值设定
        /// </summary>
        public static int 制度索引
        {
            get { return CommonInfo.system_index; }
            set { CommonInfo.system_index = value; }
        }

        public static string 用户权限
        {
            get { return CommonInfo.authority; }
            set { CommonInfo.authority = value; }
        }

        public static string 真实姓名
        {
            get { return CommonInfo.realname; }
            set { CommonInfo.realname = value; }
        }

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
