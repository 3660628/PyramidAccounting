using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.Model.DataGrid;
using PA.Helper.DataBase;
using System.Data;
using PA.Model.Database;

namespace PA.ViewModel
{
    class ViewModel_科目管理
    {
        DataBase db = new DataBase();
        public List<Model_科目管理> GetData(int type)
        {
            string sql = "select * from t_subject where subject_type=" + type + " order by id,used_mark";
            DataTable dt = db.Query(sql).Tables[0];
            List<Model_科目管理> list = new List<Model_科目管理>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Model_科目管理 m = new Model_科目管理();
                DataRow d = dt.Rows[i];
                m.ID = Convert.ToInt32(d[0].ToString());
                m.序号 = d[1].ToString();
                m.科目编号 = d[2].ToString();
                m.科目名称 = d[4].ToString();
                m.年初金额 = d[5].ToString();
                m.Used_mark = Convert.ToInt32(d[7].ToString());
                m.是否启用 = m.Used_mark == 0 ? true : false;
                list.Add(m);
            }
            return list;
        }
        public void Update(List<Model_科目管理> list)
        {
            //do sth
            List<UpdateParm> upList = new List<UpdateParm>();
            Console.WriteLine(list.Count);
            foreach (Model_科目管理 m in list)
            {
                UpdateParm up = new UpdateParm();
                up.TableName = "t_subject";
                up.Key = "fee";
                up.Value = m.年初金额;
                up.WhereParm = "id=" + m.ID;
                upList.Add(up);
                UpdateParm up2 = new UpdateParm();
                up2.TableName = "t_subject";
                up2.Key = "used_mark";
                up2.Value = m.Used_mark.ToString();
                up2.WhereParm = "id=" + m.ID;
                upList.Add(up2);
            }
            Console.WriteLine(upList.Count);
            db.UpdatePackage(upList);
        }
    }
}
