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
                sql = "select subject_id,subject_name from " + DBTablesName.T_SUBJECT + " where used_mark=0 " + "and subject_id like '" + condition +
                        "%' and parent_id=0  order by id";
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
        /// <summary>
        /// 获取所有科目的最低级科目（子细目）
        /// </summary>
        /// <returns></returns>
        public List<string> GetChildSubjectList(string condition)
        {
            List<string> lists = new List<string>();
            string parm = string.Empty;
            if (!string.IsNullOrEmpty(condition))
            {
                parm = " and a.subject_id like '" + condition + "%'";
            }
            string sql  = " SELECT                                                                  "
                        + "     a.SUBJECT_ID,                                                       "
                        + "     a.subject_name,                                                     "
                        + "     b.SUBJECT_ID ParentID,                                              "
                        + "     b.subject_name ParentName,                                          "
                        + "     c.SUBJECT_ID ParentID,                                              "
                        + "     c.subject_name ParentName,                                          "
                        + "     d.SUBJECT_ID ParentID,                                              "
                        + "     d.subject_name ParentName                                           "
                        + " FROM                                                                    "
                        + "     T_SUBJECT_0 a                                                       "
                        + " LEFT JOIN T_SUBJECT_0 b ON substr(a.SUBJECT_ID, 0, 4) = b.SUBJECT_ID    "
                        + " LEFT JOIN T_SUBJECT_0 c ON substr(a.SUBJECT_ID, 0, 6) = c.SUBJECT_ID    "
                        + " LEFT JOIN T_SUBJECT_0 d ON substr(a.SUBJECT_ID, 0, 8) = d.SUBJECT_ID    "
                        + " WHERE                                                                   "
                        + "     a.SUBJECT_ID NOT IN (                                               "
                        + "         SELECT                                                          "
                        + "             PARENT_ID                                                   "
                        + "         FROM                                                            "
                        + "             T_SUBJECT_0                                                 "
                        + "         GROUP BY                                                        "
                        + "             PARENT_ID                                                   "
                        + "     )                                                                   "
                        + parm
                        + " ORDER BY                                                                "
                        + "     a.SUBJECT_ID                                                        ";
            DataBase db = new DataBase();
            Console.WriteLine(sql);
            DataTable dt = db.Query(sql).Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string str  = dt.Rows[i][0] + "\t" 
                            + dt.Rows[i][1] + "\t"  //name
                            + dt.Rows[i][2] + "\t"
                            + dt.Rows[i][3] + "\t"  //name
                            + dt.Rows[i][4] + "\t" 
                            + dt.Rows[i][5] + "\t"  //name
                            + dt.Rows[i][6] + "\t" 
                            + dt.Rows[i][7];        //name
                lists.Add(str);
            }
            return lists;
        }
    }
}
