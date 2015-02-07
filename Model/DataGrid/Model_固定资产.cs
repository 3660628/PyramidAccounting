using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Model.DataGrid
{
    class Model_固定资产
    {
        private string serialNumber;
        private string sourceName;
        private string unit;
        private decimal amount;
        private decimal price;
        private int usedYear;
        private DateTime buyDate;
        private string deparment;
        private DateTime clearDate;
        private string voucherNo;
        private string comments;
        private int deleteMark;

        private string m_价格千万;

        public string 价格千万
        {
            get { return m_价格千万; }
            set { m_价格千万 = value; }
        }
        private string m_价格百万;

        public string 价格百万
        {
            get { return m_价格百万; }
            set { m_价格百万 = value; }
        }
        private string m_价格十万;

        public string 价格十万
        {
            get { return m_价格十万; }
            set { m_价格十万 = value; }
        }
        private string m_价格万;

        public string 价格万
        {
            get { return m_价格万; }
            set { m_价格万 = value; }
        }
        private string m_价格千;

        public string 价格千
        {
            get { return m_价格千; }
            set { m_价格千 = value; }
        }
        private string m_价格百;

        public string 价格百
        {
            get { return m_价格百; }
            set { m_价格百 = value; }
        }
        private string m_价格十;

        public string 价格十
        {
            get { return m_价格十; }
            set { m_价格十 = value; }
        }
        private string m_价格元;

        public string 价格元
        {
            get { return m_价格元; }
            set { m_价格元 = value; }
        }
        private string m_价格角;

        public string 价格角
        {
            get { return m_价格角; }
            set { m_价格角 = value; }
        }
        private string m_价格分;

        public string 价格分
        {
            get { return m_价格分; }
            set { m_价格分 = value; }
        }

        private string m_购置年;

        public string 购置年
        {
            get { return m_购置年; }
            set { m_购置年 = value; }
        }
        private string m_购置月;

        public string 购置月
        {
            get { return m_购置月; }
            set { m_购置月 = value; }
        }
        private string m_购置日;

        public string 购置日
        {
            get { return m_购置日; }
            set { m_购置日 = value; }
        }

        private string m_移除年;

        public string 移除年
        {
            get { return m_移除年; }
            set { m_移除年 = value; }
        }
        private string m_移除月;

        public string 移除月
        {
            get { return m_移除月; }
            set { m_移除月 = value; }
        }
        private string m_移除日;

        public string 移除日
        {
            get { return m_移除日; }
            set { m_移除日 = value; }
        }

        public string 编号
        {
            get { return serialNumber; }
            set { serialNumber = value; }
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

        public decimal 数量
        {
            get { return amount; }
            set { amount = value; }
        }

        public decimal 价格
        {
            get { return price; }
            set { price = value; }
        }

        public int 使用年限
        {
            get { return usedYear; }
            set { usedYear = value; }
        }

        public DateTime 购置日期
        {
            get { return buyDate; }
            set { buyDate = value; }
        }

        public string 使用部门
        {
            get { return deparment; }
            set { deparment = value; }
        }

        public DateTime 报废日期
        {
            get { return clearDate; }
            set { clearDate = value; }
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
