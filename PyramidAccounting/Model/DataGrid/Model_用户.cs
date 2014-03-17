using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Model.DataGrid
{
    class Model_用户
    {
        private int userid;

        public int ID
        {
            get { return userid; }
            set { userid = value; }
        }
        private string username;
        private string realname;
        private string password;
        private string phone_no;
        private int authority;
        private DateTime create_time;
        private string comments;

        public string 用户说明
        {
            get { return comments; }
            set { comments = value; }
        }

        public DateTime 创建日期
        {
            get { return create_time; }
            set { create_time = value; }
        }

        public int 权限
        {
            get { return authority; }
            set { authority = value; }
        }

        public string 手机号码
        {
            get { return phone_no; }
            set { phone_no = value; }
        }

        public string 密码
        {
            get { return password; }
            set { password = value; }
        }

        public string 真实姓名
        {
            get { return realname; }
            set { realname = value; }
        }

        public string 用户名
        {
            get { return username; }
            set { username = value; }
        }
    }
}
