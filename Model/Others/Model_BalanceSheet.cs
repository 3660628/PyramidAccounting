using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Model.Others
{
    class Model_BalanceSheet
    {
        private int number;
        private string name;
        private int departmentType;
        private int type;


        /// <summary>
        /// 部门编号(资产部,负债部)
        /// </summary>
        public int DepartmentType
        {
            get { return departmentType; }
            set { departmentType = value; }
        }
        /// <summary>
        /// 类型编号12345
        /// </summary>
        public int Type
        {
            get { return type; }
            set { type = value; }
        }
        /// <summary>
        /// 编号
        /// </summary>
        public int Number
        {
            get { return number; }
            set { number = value; }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
