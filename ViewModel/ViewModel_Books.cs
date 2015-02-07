using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PA.Model.DataGrid;
using PA.Helper.DataDefind;
using PA.Helper.DataBase;
using PA.Helper.SQLHelper;

namespace PA.ViewModel
{
    class ViewModel_Books
    {
        private DataBase db = new DataBase();
        private string sql = string.Empty;
        private SQLReader sr = new SQLReader();


        /// <summary>
        /// 获取账套数据的方法
        /// </summary>
        /// <returns>一个账套数据集合</returns>
        public Model_账套 GetData()
        {
            sql = "select * from " + DBTablesName.T_BOOKS + " where id='" + CommonInfo.账薄号 + "'";
            DataTable dt = db.Query(sql).Tables[0];
            DataRow dr = dt.Rows[0];
            Model_账套 m = new Model_账套();
            m.ID = dr[0].ToString();
            m.账套名称 = dr[1].ToString();
            m.启用期间 = dr[3].ToString();
            m.创建日期字符串 = dr[4].ToString();
            m.会计制度 = dr[5].ToString();
            m.当前期 = Convert.ToInt32(dr[6].ToString());
            return m;
        }
        /// <summary>
        /// 更新账套名称方法
        /// </summary>
        /// <param name="m">传入的账套</param>
        /// <param name="type">0:更新账套名称，1：进行删除</param>
        /// <returns></returns>
        public bool UpdateBookName(Model_账套 m,int type)
        {
            string _sql = string.Empty;
            switch (type)
            {
                case 0:
                    _sql = " set book_name='" + m.账套名称 + "'";
                    break;
                case 1:
                    _sql = " set delete_mark=1";
                    break;
            }
            sql = "update " + DBTablesName.T_BOOKS + _sql + " where id='" + CommonInfo.账薄号 + "'";
            return db.Excute(sql);
        }
        /// <summary>
        /// 插入方法
        /// </summary>
        /// <param name="list"></param>
        public bool Insert(List<Model_账套> list)
        {
            List<string> sqlList = new List<string>();
            db.InsertPackage(DBTablesName.T_BOOKS, list.OfType<object>().ToList());
            sql = sr.ReadSQL(0, "T_VOUCHER", DBTablesName.T_VOUCHER);
            sqlList.Add(sql);
            sql = sr.ReadSQL(1, "T_VOUCHERDETAIL", DBTablesName.T_VOUCHER_DETAIL);
            sqlList.Add(sql);
            sql = sr.ReadSQL(3, "T_RECORD", DBTablesName.T_RECORD);
            sqlList.Add(sql);
            sql = sr.ReadSQL(4, "T_FEE", DBTablesName.T_FEE);
            sqlList.Add(sql);
            return  db.BatchOperate(sqlList);
        }

        /// <summary>
        /// 判断当前帐套是否存在
        /// </summary>
        /// <param name="bookName"></param>
        /// <returns></returns>
        public bool IsBookNameExist(string bookName)
        {
            sql = "select 1 from " + DBTablesName.T_BOOKS + " where book_name='" + bookName +"' and delete_mark=0";
            return db.IsExist(sql);
        }
        /// <summary>
        /// 获取当前制度索引
        /// </summary>
        /// <returns></returns>
        public string GetStandardIndex()
        {
            return GetValue().Split('\t')[0].Split(',')[2];
        }
        /// <summary>
        /// 获取当前期数
        /// </summary>
        /// <returns></returns>
        public int GetPeriod()
        {
            int i = 0;
            string str = GetValue().Split('\t')[0].Split(',')[1];
            int.TryParse(str, out i);
            return i;
        }
        

        /// <summary>
        /// 更新期数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool UpdatePeriod(int value)
        {
            sql = "update " + DBTablesName.T_BOOKS + " set period=" + value + " where id='" + CommonInfo.账薄号 + "'";
            return db.Excute(sql);
        }
        /// <summary>
        /// 获取账套创建日期和期数
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            sql = "select book_time,period,book_index from " + DBTablesName.T_BOOKS + " where id='" + CommonInfo.账薄号 + "'";
            return db.GetAllData(sql);
        }

        public string GetYear()
        {
            return GetValue().Split('\t')[0].Substring(0,4);
        }
    }
}
