using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.Model.DataGrid;
using PA.Helper.DataBase;
using System.Data;

namespace PA.ViewModel
{
    class ViewModel_科目管理
    {
        public List<Model_科目管理> GetData(int type)
        {
            string sql = "select * from t_subject where subject_type=" + type;
            DataBase db = new DataBase();
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
                m.是否使用 = Convert.ToInt32(d[7].ToString()) == 0 ? true : false;
                list.Add(m);
            }
            return list;
        }
    }
}
