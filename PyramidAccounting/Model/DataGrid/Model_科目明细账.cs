using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Model.DataGrid
{
    class Model_科目明细账
    {
        private string month;

        public string 月
        {
            get { return month; }
            set { month = value; }
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
        private string day_page;

        public string 日页
        {
            get { return day_page; }
            set { day_page = value; }
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
        private string mark;

        public string 借或贷
        {
            get { return mark; }
            set { mark = value; }
        }
        private string excess;

        public string 余额
        {
            get { return excess; }
            set { excess = value; }
        }
    }
}
