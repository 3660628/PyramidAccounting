using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.Helper.DataBase;
using PA.Model.DataGrid;
using PA.Helper.DataDefind;
using System.Data;
using PA.Helper.Tools;

namespace PA.ViewModel
{
    class ViewModel_固定资产
    {
        DataBase db = new DataBase();
        private string sql = string.Empty;
        private Util ut = new Util();

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public List<Model_固定资产> GetAllSource()
        {
            sql = "select * from " + DBTablesName.T_FIXEDASSETS ;
            List<Model_固定资产> list = new List<Model_固定资产>();
            DataSet ds = db.Query(sql);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Model_固定资产 m = new Model_固定资产();
                    DataRow d = dt.Rows[i];
                    m.编号 = d[0].ToString();
                    m.名称及规格 = d[1].ToString();
                    m.单位 = d[2].ToString();
                    m.数量 = d[3].ToString();
                    
                    string price = d[4].ToString();
                    List<string> _list = new List<string>();

                    _list = ut.Turn(price, 10);
                    m.价格千万 = _list[0];
                    m.价格百万 = _list[1];
                    m.价格十万 = _list[2];
                    m.价格万 = _list[3];
                    m.价格千 = _list[4];
                    m.价格百 = _list[5];
                    m.价格十 = _list[6];
                    m.价格元 = _list[7];
                    m.价格角 = _list[8];
                    m.价格分 = _list[9];

                    m.使用年限 = Convert.ToInt32(d[5]);
                    m.购置日期 = Convert.ToDateTime(d[6].ToString());
                    m.使用部门 = d[7].ToString();
                    m.报废日期 = Convert.ToDateTime(d[8].ToString());
                    m.凭证编号 = d[9].ToString();
                    m.备注 = d[10].ToString();
                    m.删除标志 = Convert.ToInt32(d[11]);
                    list.Add(m);
                }
            }
            return list;
        }

        /// <summary>
        ///根据ID获取某一个固定资产信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model_固定资产 GetSourceInfo(int id)
        {
            Model_固定资产 m = new Model_固定资产();
            sql = "select * from " + DBTablesName.T_FIXEDASSETS + " where userid=" + id;
            DataSet ds = db.Query(sql);
            DataRow d = ds.Tables[0].Rows[0];
            m.名称及规格 = d[1].ToString();
            m.单位 = d[2].ToString();
            m.数量 = d[3].ToString();
            m.价格 = d[4].ToString();
            m.使用年限 = Convert.ToInt32(d[5]);
            m.购置日期 = Convert.ToDateTime(d[6].ToString());
            m.使用部门 = d[7].ToString();
            m.报废日期 = Convert.ToDateTime(d[8].ToString());
            m.凭证编号 = d[9].ToString();
            m.备注 = d[10].ToString();
            return m;
        }

        /// <summary>
        /// 添加固定资产
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool Insert(List<Model_固定资产> list)
        {
            return db.InsertPackage(DBTablesName.T_FIXEDASSETS, list.OfType<object>().ToList());
        }

        /// <summary>
        /// 判断固定资产的编号是否已存在
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool ValidateIndex(string index)
        {
            sql = "select 1 from " + DBTablesName.T_FIXEDASSETS + " where name ='" + index + "' and delete_mark = 0";
            return db.IsExist(sql);
        }
    }
}
