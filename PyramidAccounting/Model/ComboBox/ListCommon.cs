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
        public List<string> GetSubjectList(string condition)
        {
            list = new List<string>();
            if (string.IsNullOrEmpty(condition))
            {
                sql = "select subject_id,subject_name from t_subject where used_mark=0 order by id";
            }
            else
            {
                sql = "select subject_id,subject_name from t_subject where used_mark=0 " + "and subject_id like '" + condition +
                        "%' order by id";
            }
            DataBase db = new DataBase();
            DataTable dt = db.Query(sql).Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string str = dt.Rows[i][0]+"\t"+dt.Rows[i][1];
                list.Add(str);
            }
            return list;
        }
    }
}
