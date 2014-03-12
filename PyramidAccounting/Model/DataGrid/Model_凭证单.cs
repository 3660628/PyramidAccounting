using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Model.DataGrid
{
    class Model_凭证单
    {
        private int id;
        private string voucher_no;
        private DateTime op_time;
        private string word;
        private int number;
        private int subsidiary_counts;
        private decimal fee_debit;
        private decimal fee_credit;
        private string accountant;
        private string bookkeeper;
        private string reviewer;
        private int review_mark;
        private int delete_mark;
        private string book_id;

        public string 账套ID
        {
            get { return book_id; }
            set { book_id = value; }
        }

        public int 删除标志
        {
            get { return delete_mark; }
            set { delete_mark = value; }
        }

        public int 审核标志
        {
            get { return review_mark; }
            set { review_mark = value; }
        }

        public string 审核
        {
            get { return reviewer; }
            set { reviewer = value; }
        }

        public string 记账
        {
            get { return bookkeeper; }
            set { bookkeeper = value; }
        }

        public string 会计主管
        {
            get { return accountant; }
            set { accountant = value; }
        }

        public decimal 合计贷方金额
        {
            get { return fee_credit; }
            set { fee_credit = value; }
        }

        public decimal 合计借方金额
        {
            get { return fee_debit; }
            set { fee_debit = value; }
        }

        public int 附属单证数
        {
            get { return subsidiary_counts; }
            set { subsidiary_counts = value; }
        }

        public int 号
        {
            get { return number; }
            set { number = value; }
        }

        public string 字
        {
            get { return word; }
            set { word = value; }
        }

        public DateTime 制表时间
        {
            get { return op_time; }
            set { op_time = value; }
        }

        public string 凭证号
        {
            get { return voucher_no; }
            set { voucher_no = value; }
        }

        public int ID
        {
            get { return id; }
            set { id = value; }
        }
    }
}
