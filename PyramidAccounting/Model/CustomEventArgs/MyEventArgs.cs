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

        public string Str
        {
            get { return str; }
            set { str = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }
    }
}
