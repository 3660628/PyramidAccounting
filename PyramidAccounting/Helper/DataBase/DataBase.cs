using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using PA.Model.DataGrid;
using PA.Model.Database;

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
                Log.Write(e.Message + "\n 错误SQL语句：" + sql);
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
                Log.Write(e.Message);
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
            bool flag = false;
            SQLiteConnection conn = DBInitialize.getDBConnection();
            conn.Open();
            SQLiteTransaction strans = conn.BeginTransaction();
            string sql = "";
            try
            {
                switch (TableName.ToUpper())
                {
                    case "T_BOOKS":
                        #region T_BOOKS
                        sql = PA.Helper.DataDefind.SqlString.Insert_T_BOOKS;
                        List<Model_帐套> bookList = Values.OfType<Model_帐套>().ToList();
                        foreach (Model_帐套 list in bookList)
                        {
                            SQLiteCommand cmd = new SQLiteCommand();
                            cmd.CommandText = sql;
                            cmd.Parameters.AddWithValue("@ID", list.ID);
                            cmd.Parameters.AddWithValue("@BOOK_NAME", list.账套名称);
                            cmd.Parameters.AddWithValue("@COMPANY_NAME", list.单位名称);
                            cmd.Parameters.AddWithValue("@MONEY_TYPE", list.本位币);
                            cmd.Parameters.AddWithValue("@CREATE_DATE", list.日期);
                            cmd.Parameters.AddWithValue("@ACCOUNTING_SYSTEM", list.会计制度);
                            cmd.Connection = conn;
                            cmd.ExecuteNonQuery();
                        }
                        #endregion
                        break;
                    case "T_VOUCHER":
                        #region T_VOUCHER
                        sql = PA.Helper.DataDefind.SqlString.Insert_T_VOUCHER;
                        List<Model_凭证单> 凭证单List = Values.OfType<Model_凭证单>().ToList();
                        foreach (Model_凭证单 list in 凭证单List)
                        {
                            SQLiteCommand cmd = new SQLiteCommand();
                            cmd.CommandText = sql;
                            cmd.Parameters.AddWithValue("@VOUCHER_NO", list.凭证号);
                            cmd.Parameters.AddWithValue("@OP_TIME", list.制表时间);
                            cmd.Parameters.AddWithValue("@WORD", list.字);
                            cmd.Parameters.AddWithValue("@NUMBER", list.号);
                            cmd.Parameters.AddWithValue("@SUBSIDIARY_COUNTS", list.附属单证数);
                            cmd.Parameters.AddWithValue("@FEE_DEBIT", list.合计借方金额);
                            cmd.Parameters.AddWithValue("@FEE_CREDIT", list.合计贷方金额);
                            cmd.Parameters.AddWithValue("@ACCOUNTANT", list.会计主管);
                            cmd.Parameters.AddWithValue("@BOOKEEPER", list.制单人);
                            cmd.Parameters.AddWithValue("@REVIEWER", list.复核);
                            cmd.Parameters.AddWithValue("@REVIEW_MARK", list.审核标志);
                            cmd.Parameters.AddWithValue("@DELETE_MARK", list.删除标志);
                            cmd.Connection = conn;
                            cmd.ExecuteNonQuery();
                        }
                        #endregion
                        break;
                    case "T_VOUCHER_DETAIL":
                        #region T_VOUCHER_DETAIL
                        sql = PA.Helper.DataDefind.SqlString.Insert_T_VOUCHER_DETAIL;
                        List<Model_凭证明细> 凭证明细List = Values.OfType<Model_凭证明细>().ToList();
                        foreach (Model_凭证明细 list in 凭证明细List)
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
                            cmd.Connection = conn;
                            cmd.ExecuteNonQuery();
                        }
                        #endregion
                        break;
                    case "T_SUBJECT":
                        #region T_SUBJECT
                        sql = PA.Helper.DataDefind.SqlString.Insert_T_SUBJECT;
                        List<Model_科目管理> 科目管理List = Values.OfType<Model_科目管理>().ToList();
                        foreach (Model_科目管理 list in 科目管理List)
                        {
                            SQLiteCommand cmd = new SQLiteCommand();
                            cmd.CommandText = sql;
                            cmd.Parameters.AddWithValue("@SID", list.序号);
                            cmd.Parameters.AddWithValue("@SUBJECT_ID", list.科目编号);
                            cmd.Parameters.AddWithValue("@SUBJECT_TYPE", list.类别);
                            cmd.Parameters.AddWithValue("@SUBJECT_NAME", list.科目名称);
                            cmd.Parameters.AddWithValue("@FEE", list.年初金额);
                            cmd.Parameters.AddWithValue("@PARENT_ID", list.父ID);
                            cmd.Parameters.AddWithValue("@USED_MARK", list.是否启用);
                            cmd.Connection = conn;
                            cmd.ExecuteNonQuery();
                        }
                        #endregion
                        break;
                    case "T_SUBJECT_TYPE":
                        #region T_SUBJECT_TYPE


                        #endregion
                        break;
                    case "T_USER":
                        #region T_USER
                        sql = PA.Helper.DataDefind.SqlString.Insert_T_USER;
                        List<Model_用户> 用户List = Values.OfType<Model_用户>().ToList();
                        foreach (Model_用户 list in 用户List)
                        {
                            SQLiteCommand cmd = new SQLiteCommand();
                            cmd.CommandText = sql;
                            cmd.Parameters.AddWithValue("@USERNAME", list.用户名);
                            cmd.Parameters.AddWithValue("@REALNAME", list.真实姓名);
                            cmd.Parameters.AddWithValue("@PASSWORD", list.密码);
                            cmd.Parameters.AddWithValue("@PHONE_NO", list.手机号码);
                            cmd.Parameters.AddWithValue("@AUTHORITY", list.权限);
                            cmd.Parameters.AddWithValue("@CREATE_TIME", list.创建日期);
                            cmd.Parameters.AddWithValue("@COMMENTS", list.用户说明);
                            cmd.Connection = conn;
                            cmd.ExecuteNonQuery();
                        }

                        #endregion
                        break;
                    case "T_RECORD":
                        #region T_RECORD


                        #endregion
                        break;
                }
                strans.Commit();
                flag = true;
            }
            catch(Exception ee)
            {
                strans.Rollback();
                Console.WriteLine(ee.ToString());
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return flag;
        }
        public bool UpdatePackage(List<UpdateParm> lists)
        {
            bool flag = false;
            string sql = string.Empty;
            SQLiteConnection conn = DBInitialize.getDBConnection();
            conn.Open();
            SQLiteTransaction strans = conn.BeginTransaction();
            try
            {
                foreach (UpdateParm list in lists)
                {
                    sql = PA.Helper.DataDefind.SqlString.Update_Sql;
                    SQLiteCommand cmd = new SQLiteCommand();
                    sql = sql.Replace("@tableName", list.TableName);
                    sql = sql.Replace("@key", list.Key);
                    sql = sql.Replace("@value", list.Value);
                    sql = sql.Replace("@whereParm", list.WhereParm);
                    cmd.CommandText = sql;
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                }
                strans.Commit();
                flag = true;
            }
            catch(Exception ee)
            {
                strans.Rollback();
                Log.Write(ee.Message);
                Log.Write(sql);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return flag;
        }
        public DataSet SelectPackage(string TableName)
        {
            return SelectPackage(TableName,"");
        }
        public DataSet SelectPackage(string TableName, string WhereParm)
        {
            string sql = "Select * from " + TableName;
            if (WhereParm != "")
            {
                sql += " where " + WhereParm;
            }
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
        public bool DeletePackage(string TableName, List<string> whereParm)
        {
            bool flag = false;
            string sql = string.Empty;
            SQLiteConnection conn = DBInitialize.getDBConnection();
            conn.Open();
            SQLiteTransaction strans = conn.BeginTransaction();
            try
            {
                foreach (string parm in whereParm)
                {
                    sql = PA.Helper.DataDefind.SqlString.Delete_Sql;
                    SQLiteCommand cmd = new SQLiteCommand();
                    sql = sql.Replace("@tableName", TableName);
                    sql = sql.Replace("@whereParm", parm);
                    cmd.CommandText = sql;
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                }
                strans.Commit();
                flag = true;
            }
            catch (Exception ee)
            {
                strans.Rollback();
                Log.Write(ee.Message);
                Log.Write(sql);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return flag;
        }
    }
}
