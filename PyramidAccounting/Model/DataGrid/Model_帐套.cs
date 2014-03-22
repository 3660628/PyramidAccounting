using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Model.DataGrid
{
    class Model_帐套
    {
        private string id;
        private string book_name;
        private string commpany_name;
        private string book_date;
        private DateTime create_time;
        private string accounting_system;
        private int delete_mark;

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
            get { return create_time; }
            set { create_time = value; }
        }

        public string 帐套名称
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
