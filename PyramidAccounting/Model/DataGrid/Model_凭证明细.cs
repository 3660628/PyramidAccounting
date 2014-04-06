using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PA.Model.DataGrid
{
    class Model_凭证明细 : INotifyPropertyChanged
    {
        private int id;
        private int vid;
        private string parentid;
        private string word;
        private string voucher_ID;
        private string abstract_comments;
        private string subject_id;
        private string subject_name;//新增，不存数据库
        private string detail_id;//新增
        private string deal;//不存数据库
        private int bookkeep_mark;
        private decimal debit;
        private decimal credit;

        public string 子细目ID
        {
            get { return detail_id; }
            set { detail_id = value; }
        }
        public string 主科目名
        {
            get { return subject_name; }
            set { subject_name = value; NotifyPropertyChanged("主科目名"); }
        }
        public string 凭证字
        {
            get { return word; }
            set { word = value; }
        }
        public string 凭证号
        {
            get { return voucher_ID; }
            set { voucher_ID = value; }
        }
        public int 序号
        {
            get { return vid; }
            set { vid = value; }
        }

        public decimal 贷方
        {
            get { return credit; }
            set { credit = value; NotifyPropertyChanged("贷方"); }
        }

        public decimal 借方
        {
            get { return debit; }
            set { debit = value; NotifyPropertyChanged("借方"); }
        }

        public int 记账
        {
            get { return bookkeep_mark; }
            set { bookkeep_mark = value; NotifyPropertyChanged("记账"); }
        }

        public string 子细目
        {
            get { return deal; }
            set { deal = value; NotifyPropertyChanged("子细目"); }
        }

        public string 科目编号
        {
            get { return subject_id; }
            set { subject_id = value; NotifyPropertyChanged("科目编号"); }
        }

        public string 摘要
        {
            get { return abstract_comments; }
            set { abstract_comments = value; }
        }

        public string 父节点ID
        {
            get { return parentid; }
            set { parentid = value; }
        }

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
