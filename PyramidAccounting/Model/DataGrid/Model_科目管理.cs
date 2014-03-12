using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Model.DataGrid
{
    class Model_科目管理
    {
        private int id;
        private string subject_id;
        private string subject_type;
        private string subject_name;
        private string direction;
        private decimal fee;
        private bool used_mark;

        public bool 是否使用
        {
            get { return used_mark; }
            set { used_mark = value; }
        }

        public decimal 期初金额
        {
            get { return fee; }
            set { fee = value; }
        }

        public string 方向
        {
            get { return direction; }
            set { direction = value; }
        }

        public string 科目名称
        {
            get { return subject_name; }
            set { subject_name = value; }
        }

        public string 类别
        {
            get { return subject_type; }
            set { subject_type = value; }
        }

        public string 科目编号
        {
            get { return subject_id; }
            set { subject_id = value; }
        }

        public int ID
        {
            get { return id; }
            set { id = value; }
        }
    }
}
