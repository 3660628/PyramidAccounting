using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PA.Helper.DataBase;
using PA.Helper.DataDefind;

namespace PA.Model.ComboBox
{
    class ComboBox_科目
    {
        private List<string> list;
        private string sql = string.Empty;
        public List<string> GetSubjectList(string condition)
        {
            list = new List<string>();
            if (string.IsNullOrEmpty(condition))
            {
                sql = "select subject_id,subject_name from " + DBTablesName.T_SUBJECT + " where used_mark=0 and parent_id=0 order by id";
            }
            else
            {
                sql = "select subject_id,subject_name from " + DBTablesName.T_SUBJECT + " where used_mark=0  and parent_id=0 " + "and subject_id like '" + condition +
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
        public List<string> GetChildSubjectList(string condition,string id)
        {
            list = new List<string>();
            if (string.IsNullOrEmpty(condition))
            {
                sql = "select subject_id,subject_name from " + DBTablesName.T_SUBJECT + " where parent_id LIKE '" + id + "%' order by subject_id";
            }
            else
            {
                sql = "select subject_id,subject_name from " + DBTablesName.T_SUBJECT + " where parent_id LIKE '" + id + "%' and subject_id like '" + condition +
                        "%' order by subject_id";
            }
            DataBase db = new DataBase();
            DataTable dt = db.Query(sql).Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string str = dt.Rows[i][0] + "\t" + dt.Rows[i][1];
                list.Add(str);
            }
            return list;
        }
        /// <summary>
        /// 不显示三级科目
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="id"></param>
        /// <param name="isTwo"></param>
        /// <returns></returns>
        public List<string> GetChildSubjectList(string condition, string id, bool isTwo)
        {
            list = new List<string>();
            if (string.IsNullOrEmpty(condition))
            {
                sql = "select subject_id,subject_name from " + DBTablesName.T_SUBJECT + " where parent_id='" + id + "' order by subject_id";
            }
            else
            {
                sql = "select subject_id,subject_name from " + DBTablesName.T_SUBJECT + " where parent_id='" + id + "' and subject_id like '" + condition +
                        "%' order by subject_id";
            }
            DataBase db = new DataBase();
            DataTable dt = db.Query(sql).Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string str = dt.Rows[i][0] + "\t" + dt.Rows[i][1];
                list.Add(str);
            }
            return list;
        }
    }
}
