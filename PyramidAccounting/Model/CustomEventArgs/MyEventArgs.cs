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

        

        #region GETSET
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
        /// 跳转坐标：竖坐标（一级Tab）
        /// </summary>
        public int Y
        {
            get { return y; }
            set { y = value; }
        }
        /// <summary>
        /// 跳转坐标：横坐标（二级Tab）
        /// </summary>
        public int X
        {
            get { return x; }
            set { x = value; }
        }
        #endregion
    }
}
