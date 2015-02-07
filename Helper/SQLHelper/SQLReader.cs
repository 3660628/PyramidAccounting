using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PA.Helper.SQLHelper
{
    class SQLReader
    {
        /// <summary>
        /// 读SQL文件返回建表语句
        /// </summary>
        /// <param name="rowid">需要获取表的索引，详见建表语句</param>
        /// <param name="tableName">表名</param>
        /// <returns>转换好的SQL语句</returns>
        public string ReadSQL(int rowid,string tableName,string newTableName)
        {
            string str = Properties.Resources.ReadOnlySQL;
            string[] arr = str.Split(';');
            string sql = arr[rowid].Replace(tableName, newTableName);
            return sql;
        }
    }
}
