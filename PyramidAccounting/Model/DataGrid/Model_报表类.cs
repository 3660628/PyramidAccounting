using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Model.DataGrid
{
    class Model_报表类
    {
        private string yearfee;

        public string 年初数
        {
            get { return yearfee; }
            set { yearfee = value; }
        }
        private string monthfee;

        public string 期末数
        {
            get { return monthfee; }
            set { monthfee = value; }
        }

        private string _fee;

        public string 本期数
        {
            get { return _fee; }
            set { _fee = value; }
        }

        private string totalfee;

        public string 累计数
        {
            get { return totalfee; }
            set { totalfee = value; }
        }
    }
}
