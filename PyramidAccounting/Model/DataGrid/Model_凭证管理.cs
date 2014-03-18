using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Model.DataGrid
{
    class Model_凭证管理
    {
        private string voucher_no;
        private string op_time; //DateTime
        private string abstract_comments;
        private string subject_id;
        private string detail;
        private string debit;
        private string credit;
        private string current_time;
        private string review_status;

        public string 审核状态
        {
            get { return review_status; }
            set { review_status = value; }
        }

        public string 当前期数
        {
            get { return current_time; }
            set { current_time = value; }
        }

        public string 贷方金额
        {
            get { return credit; }
            set { credit = value; }
        }

        public string 借方金额
        {
            get { return debit; }
            set { debit = value; }
        }

        public string 科目名称
        {
            get { return detail; }
            set { detail = value; }
        }

        public string 科目编号
        {
            get { return subject_id; }
            set { subject_id = value; }
        }

        public string 摘要
        {
            get { return abstract_comments; }
            set { abstract_comments = value; }
        }
        public string 凭证号
        {
            get { return voucher_no; }
            set { voucher_no = value; }
        }


        public string 制表时间
        {
            get { return op_time; }
            set { op_time = value; }
        }
    }
}
