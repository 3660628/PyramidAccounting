using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PA.Model.DataGrid;
using PA.Helper.DataDefind;
using PA.Helper.DataBase;

namespace PA.ViewModel
{
    class ViewModel_Books
    {
        private DataBase db = new DataBase();
        private string sql = string.Empty;
        /// <summary>
        /// 插入方法
        /// </summary>
        /// <param name="list"></param>
        public void Insert(List<Model_帐套> list)
        {
            List<string> sqlList = new List<string>();
            Console.WriteLine(list.Count);
            db.InsertPackage(DBTablesName.T_BOOKS, list.OfType<object>().ToList());
            sql = "create table " + DBTablesName.T_VOUCHER + " as select * from t_voucher where 1=0";
            sqlList.Add(sql);
            sql = "create table " + DBTablesName.T_VOUCHER_DETAIL + " as select * from  t_voucher_detail  where 1=0";
            sqlList.Add(sql);
            sql = "create table " + DBTablesName.T_RECORD + " as select * from  t_record where 1=0";
            sqlList.Add(sql);
            db.BatchOperate(sqlList);
            
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
        /// 获取单位名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns>单位名称</returns>
        public string GetCompanyName(string id)
        {
            sql = "select company_name from " + DBTablesName.T_BOOKS + " where delete_mark=0 and id='" + id + "'";
            return db.GetAllData(sql).Split('\t')[0].Split(',')[0];
        }
    }
}
