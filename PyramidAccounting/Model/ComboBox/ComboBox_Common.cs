using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PA.Model.DataGrid;
using PA.Helper.DataBase;

namespace PA.Model.ComboBox
{
    class ComboBox_Common
    {
        private string sql = string.Empty;
        private DataSet ds = new DataSet();
        private DataBase db = new DataBase();
        public List<Model_帐套> GetComboBox_帐套()
        {
            List<Model_帐套> list = new List<Model_帐套>();
            Model_帐套 def = new Model_帐套();
            def.ID = "0";
            def.帐套名称 = "新建帐套";
            list.Add(def);
            sql = "select id,book_name from t_books where delete_mark=0 order by create_date desc";
            ds = db.Query(sql);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    Model_帐套 m = new Model_帐套();
                    m.ID = dr[0].ToString();
                    m.帐套名称 = dr[1].ToString();
                    list.Add(m);
                }
            }
            return list;
        }
        public List<string> GetComboBox_会计制度()
        {
            List<string> list = new List<string>();
            list.Add("事业单位会计制度(2013年)");
            return list;
        }

        public List<string> GetComboBox_本位币()
        {
            List<string> list = new List<string>();
            list.Add("RMB");
            return list;
        }
    }
}
