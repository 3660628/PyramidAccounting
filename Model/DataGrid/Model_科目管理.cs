using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PA.Model.DataGrid
{
    class Model_科目管理 : INotifyPropertyChanged
    {
        private int id;
        private string sid;
        private string subject_id;
        private string subject_type;
        private string subject_name;
        private string direction;
        private string fee;
        private int used_mark;
        private string parent_id;
        private bool mark;
        private bool Borrow_Mark;

        /// <summary>
        /// 数据库是Int类型，true=1.false=-1
        /// </summary>
        public bool 借贷标记
        {
            get { return Borrow_Mark; }
            set { Borrow_Mark = value; }
        }

        public string 父ID
        {
            get { return parent_id; }
            set { parent_id = value; }
        }
        public bool 是否启用
        {
            get { return mark; }
            set { mark = value; }
        }
        public string 序号
        {
            get { return sid; }
            set { sid = value; }
        }
        public int Used_mark
        {
            get { return used_mark; }
            set { used_mark = value; }
        }

        public string 年初金额
        {
            get { return fee; }
            set { fee = value; }
        }

        public string 方向
        {
            get { return direction; }
            set { direction = value; }
        }

        public string 科目名称
        {
            get { return subject_name; }
            set { subject_name = value; }
        }

        public string 类别
        {
            get { return subject_type; }
            set { subject_type = value; }
        }

        public string 科目编号
        {
            get { return subject_id; }
            set { subject_id = value; }
        }

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Helpers

        /// <summary>
        /// cell内容改变事件
        /// </summary>
        /// <param name="propertyName"></param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
