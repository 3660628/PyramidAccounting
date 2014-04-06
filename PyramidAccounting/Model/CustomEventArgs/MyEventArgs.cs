using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Model.CustomEventArgs
{
    public class MyEventArgs : EventArgs
    {
        private int y;
        private int x;
        private string str;
        private string type;
        private bool isCommit;

        #region GETSET
        public bool IsCommit
        {
            get { return isCommit; }
            set { isCommit = value; }
        }
        public string 操作类型
        {
            get { return type; }
            set { type = value; }
        }
        public string Str
        {
            get { return str; }
            set { str = value; }
        }
        /// <summary>
        /// 跳转坐标：竖坐标（一级Tab）,0开始
        /// </summary>
        public int Y
        {
            get { return y; }
            set { y = value; }
        }
        /// <summary>
        /// 跳转坐标：横坐标（二级Tab），0开始
        /// </summary>
        public int X
        {
            get { return x; }
            set { x = value; }
        }
        #endregion
    }
}
