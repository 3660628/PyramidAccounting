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

        public void Insert(List<Model_帐套> list)
        {
            List<string> sqlList = new List<string>();
            Console.WriteLine(list.Count);
            db.InsertPackage(DBTablesName.T_BOOKS, list.OfType<object>().ToList());
            sql = "create table " + DBTablesName.T_VOUCHER + " as select * from t_voucher where 1=0";
            sqlList.Add(sql);
            sql = "create table " + DBTablesName.T_VOUCHER_DETAIL + " as select * from  t_voucher_detail  where 1=0";
            sqlList.Add(sql);
            sql = "create table " + DBTablesName.T_SUBJECT + " as select * from  t_subject";
            sqlList.Add(sql);
            sql = "create table " + DBTablesName.T_RECORD + " as select * from  t_record where 1=0";
            sqlList.Add(sql);
            db.BatchOperate(sqlList);
            
        }
    }
}
