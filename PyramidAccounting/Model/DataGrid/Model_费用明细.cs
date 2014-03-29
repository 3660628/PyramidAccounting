using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Model.DataGrid
{
    class Model_费用明细
    {
        private string year;

        public string 年
        {
            get { return year; }
            set { year = value; }
        }
        private string month;

        public string 月
        {
            get { return month; }
            set { month = value; }
        }
        private string voucher_no;

        public string 号数
        {
            get { return voucher_no; }
            set { voucher_no = value; }
        }
        private string day;

        public string 日
        {
            get { return day; }
            set { day = value; }
        }
        private string comments;

        public string 摘要
        {
            get { return comments; }
            set { comments = value; }
        }

        private string debt;

        public string 借方金额
        {
            get { return debt; }
            set { debt = value; }
        }
        private string credit;

        public string 贷方金额
        {
            get { return credit; }
            set { credit = value; }
        }
        private string fee;

        public string 余额
        {
            get { return fee; }
            set { fee = value; }
        }
    }
}
