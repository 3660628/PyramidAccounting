using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Model.DataGrid
{
    class Model_账套
    {
        private string id;
        private int rowid;
        private string book_name;
        private string commpany_name;
        private string book_date;
        private DateTime create_date;
        private string _create_date;
        private string accounting_system;
        private int delete_mark;
        private int period;
        private int sys_index;

        public int 制度索引
        {
            get { return sys_index; }
            set { sys_index = value; }
        }

        public int 当前期
        {
            get { return period; }
            set { period = value; }
        }
        public string 创建日期字符串
        {
            get { return _create_date; }
            set { _create_date = value; }
        }
        public int 序号
        {
            get { return rowid; }
            set { rowid = value; }
        }
        public string 启用期间
        {
            get { return book_date; }
            set { book_date = value; }
        }

        public string 单位名称
        {
            get { return commpany_name; }
            set { commpany_name = value; }
        }

        public int 删除标志
        {
            get { return delete_mark; }
            set { delete_mark = value; }
        }

        public string 会计制度
        {
            get { return accounting_system; }
            set { accounting_system = value; }
        }

        public DateTime 创建时间
        {
            get { return create_date; }
            set { create_date = value; }
        }

        public string 账套名称
        {
            get { return book_name; }
            set { book_name = value; }
        }

        public string ID
        {
            get { return id; }
            set { id = value; }
        }
    }
}
