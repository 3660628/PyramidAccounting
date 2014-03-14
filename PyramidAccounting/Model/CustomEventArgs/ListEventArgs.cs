using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Model.CustomEventArgs
{
    public class ListEventArgs:EventArgs
    {
        private List<string> list;

        public List<string> List
        {
            get { return list; }
            set { list = value; }
        }
    }
}
