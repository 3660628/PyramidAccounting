using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.Model.DataGrid;
using PA.Helper.DataBase;
using System.Data;
using PA.Model.Database;
using PA.Helper.DataDefind;

namespace PA.ViewModel
{
    class ViewModel_科目管理
    {
        DataBase db = new DataBase();
        public List<Model_科目管理> GetAllSubject()
        {
            List<Model_科目管理> list = new List<Model_科目管理>();
            string sql = "select subject_id from " + DBTablesName.T_SUBJECT;
            DataTable dt = db.Query(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                Model_科目管理 m = new Model_科目管理();
                m.科目编号 = dr[0].ToString();
                list.Add(m);
            }
            return list;
        }
        public List<Model_科目管理> GetSujectData(int type)
        {
            string sql = "select a.fee,b.* from " + DBTablesName.T_SUBJECT + " b left join t_yearfee a on a.bookid='" + CommonInfo.账薄号 + "' and a.subject_id=b.subject_id where  b.subject_type=" + type + " order by b.id,b.used_mark";
            DataTable dt = db.Query(sql).Tables[0];
            List<Model_科目管理> list = new List<Model_科目管理>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Model_科目管理 m = new Model_科目管理();
                DataRow d = dt.Rows[i];
                m.ID = Convert.ToInt32(d[1].ToString());
                m.序号 = "" + (i + 1);
                m.科目编号 = d[3].ToString();
                m.科目名称 = d[5].ToString();
                m.年初金额 = d[0].ToString();
                m.Used_mark = Convert.ToInt32(d[7].ToString());
                m.是否启用 = m.Used_mark == 0 ? true : false;
                int 借贷标记result = 1;
                int.TryParse(d[8].ToString(), out 借贷标记result);
                m.借贷标记 = (借贷标记result == 1) ? true : false;
                list.Add(m);
            }
            return list;
        }
        public List<Model_科目管理> GetChildSubjectData(string parent_id)
        {
            string sql = "select a.*,b.fee from " + DBTablesName.T_SUBJECT + " a left join t_yearfee b on a.subject_id=b.subject_id  where b.bookid='"
                + CommonInfo.账薄号 + "' and a.parent_id='" + parent_id + "' order by a.id";
            Console.WriteLine(sql);
            DataTable dt = db.Query(sql).Tables[0];
            List<Model_科目管理> list = new List<Model_科目管理>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Model_科目管理 m = new Model_科目管理();
                DataRow d = dt.Rows[i];
                m.ID = Convert.ToInt32(d[0].ToString());
                m.科目编号 = d[2].ToString();
                m.科目名称 = d[4].ToString();
                m.年初金额 = d[8].ToString();
                list.Add(m);
            }
            return list;
        }
       
        public void UpdateUsedMark(Model_科目管理 m)
        {
            string sql = "update " + DBTablesName.T_SUBJECT + " set used_mark=" + m.Used_mark + " where id=" + m.ID;
            db.Excute(sql);
        }

        public void UpdateBorrowMark(Model_科目管理 m)
        {
            string sql = "update " + DBTablesName.T_SUBJECT + " set Borrow_Mark=" + ((m.借贷标记)?1:-1) + " where id=" + m.ID;
            db.Excute(sql);
        }

        public void UpdateChildSubject(Model_科目管理 m)
        {
            List<string> sqlList = new List<string>();
            string sql1 = "update " + DBTablesName.T_SUBJECT + " set subject_id='" + m.科目编号 + "',subject_name='" + m.科目名称 + "' where id=" + m.ID;
            sqlList.Add(sql1);
            string sql2 = "update " + DBTablesName.T_YEAR_FEE + " set fee='" + m.年初金额 + "',subject_id='" + m.科目编号 +"' where parentid='" + m.父ID + "' and subject_id='" + m.科目编号 + "' and bookid='" + CommonInfo.账薄号 + "'";
            sqlList.Add(sql2);
            string sql3 = " update T_YEARFEE set fee = (select total(fee) from T_YEARFEE where parentid=" + m.父ID + ") where subject_id=" + m.父ID;
            sqlList.Add(sql3);
            db.BatchOperate(sqlList);
        }

        public void Insert(List<Model_科目管理> list)
        {
            db.InsertPackage(DBTablesName.T_SUBJECT, list.OfType<object>().ToList());
            List<string> sqlList = new List<string>();
            foreach(Model_科目管理 m in list)
            {
                string sql = "insert into t_yearfee values ('" + m.科目编号 + "','" + m.年初金额 + "','" + m.父ID + "','" + CommonInfo.账薄号 + "')";
                sqlList.Add(sql);
            }
            db.BatchOperate(sqlList);
        }

        public void Delete(List<Model_科目管理> list)
        {
            List<string> sqlList = new List<string>();
            foreach (Model_科目管理 m in list)
            {
                string sql = "delete from " + DBTablesName.T_SUBJECT + " where id=" + m.ID;
                sqlList.Add(sql);
                sql = "delete from " + DBTablesName.T_YEAR_FEE + " where subject_id='" + m.科目编号 + "' and parentid='" + m.父ID + "' and bookid='" + CommonInfo.账薄号 + "'";
                sqlList.Add(sql);
            }
            db.BatchOperate(sqlList);
        }

        public string GetSubjectID(string name)
        {
            string sql = "select subject_id from " + DBTablesName.T_SUBJECT + " where subject_name='" + name + "' and parent_id = 0";
            return db.GetAllData(sql).Split('\t')[0].Split(',')[0];
        }
    }
}
