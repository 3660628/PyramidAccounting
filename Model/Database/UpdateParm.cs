using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Model.Database
{
    class UpdateParm
    {
        private string tableName;
        private string key;
        private string value;
        private string whereParm;

        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }

        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public string WhereParm
        {
            get { return whereParm; }
            set { whereParm = value; }
        }
    }
}
