
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Model.DataGrid
{
    class Model_事业报表类
    {
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

        private string fee1;

        public string 本期数1
        {
            get { return fee1; }
            set { fee1 = value; }
        }
        private string totalfee1;

        public string 累计数1
        {
            get { return totalfee1; }
            set { totalfee1 = value; }
        }

        private string fee2;

        public string 本期数2
        {
            get { return fee2; }
            set { fee2 = value; }
        }
        private string totalfee2;

        public string 累计数2
        {
            get { return totalfee2; }
            set { totalfee2 = value; }
        }
        private string ID;

        public string 编号
        {
            get { return ID; }
            set { ID = value; }
        }
    }
}
