using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Model.DataGrid
{
    class Model_固定资产
    {
        private int rowid;
        private string sourceName;
        private string unit;
        private string amount;
        private string price;
        private int usedYear;
        private string buyDate;
        private string deparment;
        private string createDate;
        private string voucherNo;
        private string comments;
        private int deleteMark;

        public int 序号
        {
            get { return rowid; }
            set { rowid = value; }
        }

        public string 名称及规格
        {
            get { return sourceName; }
            set { sourceName = value; }
        }

        public string 单位
        {
            get { return unit; }
            set { unit = value; }
        }

        public string 数量
        {
            get { return amount; }
            set { amount = value; }
        }

        public string 价格
        {
            get { return price; }
            set { price = value; }
        }

        public int 使用年限
        {
            get { return usedYear; }
            set { usedYear = value; }
        }

        public string 购置日期
        {
            get { return buyDate; }
            set { buyDate = value; }
        }

        public string 使用部门
        {
            get { return deparment; }
            set { deparment = value; }
        }

        public string 报废日期
        {
            get { return createDate; }
            set { createDate = value; }
        }

        public string 凭证编号
        {
            get { return voucherNo; }
            set { voucherNo = value; }
        }

        public string 备注
        {
            get { return comments; }
            set { comments = value; }
        }

        public int 删除标志
        {
            get { return deleteMark; }
            set { deleteMark = value; }
        }
    }
}
