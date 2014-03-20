using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Model.DataGrid
{
    class Model_年初金额
    {
        private string subject_id;
        private string fee;
        private string bookid;

        public string 账套ID
        {
            get { return bookid; }
            set { bookid = value; }
        }

        public string 年初金额
        {
            get { return fee; }
            set { fee = value; }
        }


        public string 科目编号
        {
            get { return subject_id; }
            set { subject_id = value; }
        }

    }
}
