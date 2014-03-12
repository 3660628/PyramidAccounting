using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Model.EventArgss
{
    public class StringEventArgs : EventArgs
    {
        private string str;

        public string Str
        {
            get { return str; }
            set { str = value; }
        }
    }
}
