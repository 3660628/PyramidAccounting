using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.Helper.DataBase;
using PA.Model.DataGrid;
using PA.Helper.DataDefind;
using System.Data;

namespace PA.ViewModel
{
    class ViewModel_固定资产
    {
        DataBase db = new DataBase();
        private string sql = string.Empty;

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public List<Model_固定资产> GetAllSource()
        {
            sql = "";
            List<Model_固定资产> list = new List<Model_固定资产>();
            DataSet ds = db.Query(sql);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Model_固定资产 m = new Model_固定资产();
                    DataRow d = dt.Rows[i];
                    m.序号 = i + 1;
                    m.名称及规格 = d[1].ToString();
                    m.单位 = d[2].ToString();
                    m.数量 = d[3].ToString();
                    m.价格 = d[4].ToString();
                    m.使用年限 = Convert.ToInt32(d[5]);
                    m.购置日期 = Convert.ToDateTime(d[6].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    m.使用部门 = d[7].ToString();
                    m.报废日期 = Convert.ToDateTime(d[8].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    m.凭证编号 = d[9].ToString();
                    m.凭证编号 = d[10].ToString();
                    list.Add(m);
                }
            }
            return list;
        }
    }
}
