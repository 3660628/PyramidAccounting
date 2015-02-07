using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Model.DataGrid
{
    class Model_操作日志
    {
        private int id;
        private int rowid;
        private string op_time;
        private string username;
        private string realname;
        private string log;

        public string 日期
        {
            get { return op_time; }
            set { op_time = value; }
        }
        public string 日志
        {
            get { return log; }
            set { log = value; }
        }

        public string 姓名
        {
            get { return realname; }
            set { realname = value; }
        }

        public string 用户名
        {
            get { return username; }
            set { username = value; }
        }

        public int 序号
        {
            get { return rowid; }
            set { rowid = value; }
        }

        public int ID
        {
            get { return id; }
            set { id = value; }
        }
    }
}
