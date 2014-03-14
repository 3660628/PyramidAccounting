using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using PA.Model.DataGrid;

namespace PA.Helper.DataBase
{
    class DataBase
    {
        /// <summary>
        /// Insert语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool Excute(string sql)
        {
            bool flag = false;
            SQLiteConnection conn = DBInitialize.getDBConnection();
            conn.Open();
            try
            {
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                flag = true;
            }
            catch (SQLiteException e)
            {
                //Log.Write(e.Message + "\n 错误SQL语句：" + sql);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return flag;
        }

        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="sqlList"></param>
        /// <returns></returns>
        public bool BatchOperate(List<string> sqlList)
        {
            bool flag = false;
            SQLiteConnection conn = DBInitialize.getDBConnection();
            conn.Open();
            SQLiteTransaction strans = conn.BeginTransaction();
            try
            {

                foreach (string sql in sqlList)
                {
                    SQLiteCommand cmd = new SQLiteCommand();
                    cmd.CommandText = sql;
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                }
                strans.Commit();
                flag = true;
            }
            catch (SQLiteException e)
            {
                strans.Rollback();
                //Log.Write(e.Message);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return flag;
        }

        public string GetAllData(string sql)
        {
            SQLiteConnection conn = DBInitialize.getDBConnection();
            StringBuilder sb = new StringBuilder();
            string result = "";
            conn.Open();
            try
            {
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string dbType = reader.GetDataTypeName(i);
                            switch (dbType)
                            {
                                case "DATE":
                                    sb = sb.Append(reader.GetDateTime(i).ToString("yyyy-MM-dd")).Append(",");
                                    break;
                                default:
                                    sb = sb.Append(reader.GetValue(i)).Append(",");
                                    break;
                            }
                        }
                        sb.Append("\t");
                    }
                    result = sb.ToString().ToString();
                }
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("异常:" + e.Message);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return result;
        }
        /// <summary>
        /// 根据查询的SQL返回一个dataset
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet Query(string sql)
        {
            DataSet ds = new DataSet();
            SQLiteConnection conn = DBInitialize.getDBConnection();
            conn.Open();
            try
            {
                SQLiteDataAdapter my = new SQLiteDataAdapter(sql, conn);
                my.Fill(ds);
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("SQL{0}:" + sql);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return ds;
        }


        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool IsExist(string sql)
        {
            bool flag = false;
            if (GetAllData(sql).Length > 0)
            {
                flag = true;
            }
            return flag;
        }

        public bool InsertPackage(string TableName, List<object> Values)
        {
            string sql = "";

            SQLiteConnection conn = DBInitialize.getDBConnection();
            conn.Open();
            SQLiteTransaction strans = conn.BeginTransaction();

            switch (TableName.ToUpper())
            {
                case "T_VOUCHER_DETAIL":
                    sql = PA.Helper.DataDefind.SqlString.Insert_T_VOUCHER_DETAIL;
                    List<Model_凭证明细>  EntityList = Values.OfType<Model_凭证明细>().ToList();
                    foreach (Model_凭证明细 list in EntityList)
                    {
                        SQLiteCommand cmd = new SQLiteCommand();
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@VID", list.序号);
                        cmd.Parameters.AddWithValue("@PARENTID", list.父节点ID);
                        cmd.Parameters.AddWithValue("@ABSTRACT", list.摘要);
                        cmd.Parameters.AddWithValue("@SUBJECT_ID", list.科目编号);
                        cmd.Parameters.AddWithValue("@DETAIL", list.子细目);
                        cmd.Parameters.AddWithValue("@BOOKKEEP_MARK", list.记账);
                        cmd.Parameters.AddWithValue("@DEBIT", list.借方);
                        cmd.Parameters.AddWithValue("@CREDIT", list.贷方);
                        cmd.Parameters.AddWithValue("@BOOK_ID", list.账套ID);
                        cmd.Connection = conn;
                        cmd.ExecuteNonQuery();
                    }
                    break;
            }
            strans.Commit();
            


            return false;
        }
        public bool UpdatePackage(string TableName, string key, string value, string whereParm)
        {
            string sql = PA.Helper.DataDefind.SqlString.Update_Sql;
            SQLiteConnection conn = DBInitialize.getDBConnection();
            conn.Open();
            SQLiteTransaction strans = conn.BeginTransaction();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@tableName", TableName);
            cmd.Parameters.AddWithValue("@key", key);
            cmd.Parameters.AddWithValue("@value", value);
            cmd.Parameters.AddWithValue("@whereParm", whereParm);
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();
            strans.Commit();
            return false;
        }
        public DataSet SelectPackage(string TableName)
        {
            return SelectPackage(TableName,"");
        }
        public DataSet SelectPackage(string TableName, string WhereParm)
        {
            string sql = "Select * from " + TableName + " " + WhereParm;
            DataSet ds = new DataSet();
            SQLiteConnection conn = DBInitialize.getDBConnection();
            conn.Open();
            try
            {
                SQLiteDataAdapter my = new SQLiteDataAdapter(sql, conn);
                my.Fill(ds);
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("SQL{0}:" + sql);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return ds;
        }
    }
}
