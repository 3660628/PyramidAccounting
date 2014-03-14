using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PA.Helper.DataBase;

namespace PA.Model.ComboBox
{
    class ListCommon
    {
        private List<string> list;
        private string sql = string.Empty;
        public List<string> GetSubjectList()
        {
            list = new List<string>();
            sql = "select subject_id,subject_name from t_subject where used_mark=0 order by id";
            DataBase db = new DataBase();
            DataTable dt = db.Query(sql).Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string str = dt.Rows[i][0]+" "+dt.Rows[i][1];
                list.Add(str);
            }
            return list;
        }
    }
}
