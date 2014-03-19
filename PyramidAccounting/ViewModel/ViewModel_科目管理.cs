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
        public List<Model_科目管理> GetSujectData(int type)
        {
            string sql = "select a.fee,b.* from t_year_fee a left join " + DBTablesName.T_SUBJECT + " b on a.subject_id=b.subject_id where a.bookid='" + CommonInfo.账薄号 + "' and b.subject_type=" + type + " order by b.id,b.used_mark";
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
                m.Used_mark = Convert.ToInt32(d[8].ToString());
                m.是否启用 = m.Used_mark == 0 ? true : false;
                list.Add(m);
            }
            return list;
        }
        public List<Model_科目管理> GetChildSubjectData(string parent_id)
        {
            string sql = "select * from " + DBTablesName.T_SUBJECT + "  where parent_id='" + parent_id + "' order by id";
            DataTable dt = db.Query(sql).Tables[0];
            List<Model_科目管理> list = new List<Model_科目管理>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Model_科目管理 m = new Model_科目管理();
                DataRow d = dt.Rows[i];
                m.ID = Convert.ToInt32(d[0].ToString());
                m.科目编号 = d[2].ToString();
                m.科目名称 = d[4].ToString();
                list.Add(m);
            }
            return list;
        }
        public bool IsSaved()
        {
            bool flag = false;
            string sql = "select sum(fee) from " + DBTablesName.T_SUBJECT;
            string str = db.GetAllData(sql).Split('\t')[0];
            if (str.Equals(","))
            {
                return false;
            }
            else
            {
                    flag = true;
            }
            return flag;
        }
        public void Update(List<Model_科目管理> list)
        {
            List<UpdateParm> upList = new List<UpdateParm>();
            foreach (Model_科目管理 m in list)
            {
                UpdateParm up = new UpdateParm();
                up.TableName = DBTablesName.T_SUBJECT;
                up.Key = "fee";
                up.Value = m.年初金额;
                up.WhereParm = "id=" + m.ID;
                upList.Add(up);
                UpdateParm up2 = new UpdateParm();
                up2.TableName = DBTablesName.T_SUBJECT;
                up2.Key = "used_mark";
                up2.Value = m.Used_mark.ToString();
                up2.WhereParm = "id=" + m.ID;
                upList.Add(up2);
            }
            db.UpdatePackage(upList);
        }
        public void UpdateUsedMark(Model_科目管理 m)
        {
            List<UpdateParm> upList = new List<UpdateParm>();
            UpdateParm up2 = new UpdateParm();
            up2.TableName = DBTablesName.T_SUBJECT;
            up2.Key = "used_mark";
            up2.Value = m.Used_mark.ToString();
            up2.WhereParm = "id=" + m.ID;
            upList.Add(up2);
            db.UpdatePackage(upList);
        }

        public void UpdateChildSubject(Model_科目管理 m)
        {
            string sql = "update " + DBTablesName.T_SUBJECT + " set subject_id='" + m.科目编号 + "',subject_name='" + m.科目名称 + "' where id=" + m.ID;
            db.Excute(sql);
        }

        public void Insert(List<Model_科目管理> list)
        {
            db.InsertPackage(DBTablesName.T_SUBJECT, list.OfType<object>().ToList());
        }

        public void Delete(List<int> list)
        {
            List<string> sqlList = new List<string>();
            foreach(int i in list)
            {
                string sql = "delete from " + DBTablesName.T_SUBJECT + " where id=" + i;
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
