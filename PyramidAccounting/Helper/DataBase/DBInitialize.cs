using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;

namespace PA.Helper.DataBase
{
    public class DBInitialize
    {
        public static string dataSource = "Data\\test.db";
        public static string dbPassword = "";

        public DBInitialize()
        {
            Console.WriteLine("DBInitialize");
        }
        public void Initialize()
        {
            //新建数据库
            SQLiteConnection.CreateFile(dataSource);
            SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
            SQLiteConnection conn = new SQLiteConnection();
            connstr.DataSource = dataSource;
            conn.ConnectionString = connstr.ToString();

            conn.Open();
            conn.Close();

            DataBase db = new DataBase();
            List<string> tableList = new List<string>();
            tableList = getSqlList(Properties.Resources.DatabaseTable);
            db.BatchOperate(tableList);
        }

        public static SQLiteConnection getDBConnection()
        {
            SQLiteConnection conn = new SQLiteConnection();
            SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
            connstr.DataSource = dataSource;
            conn.ConnectionString = connstr.ToString();
            //conn.SetPassword(password);
            return conn;
        }
        /// <summary>
        /// 获取SQL创建脚步方法，封装成List<string>
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public List<string> getSqlList(string fileName)
        {
            string filePath = fileName;
            List<string> sqlList = new List<string>();
            {
                string str = filePath;
                string[] arr = str.Split(';');
                foreach (string s in arr)
                {
                    sqlList.Add(s);
                }
            }
            return sqlList;
        }
    }
}
