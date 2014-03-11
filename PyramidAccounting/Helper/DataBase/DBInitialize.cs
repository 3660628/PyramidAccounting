using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;

namespace PA.Data
{
    public class DBInitialize
    {
        public static string dataSource = "";
        public static string dbPassword = "";

        public DBInitialize()
        {
            //新建数据库
            SQLiteConnection.CreateFile(dataSource);
            SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
            SQLiteConnection conn = new SQLiteConnection();
            connstr.DataSource = dataSource;
            conn.ConnectionString = connstr.ToString();

            conn.Open();
            conn.Close();


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
