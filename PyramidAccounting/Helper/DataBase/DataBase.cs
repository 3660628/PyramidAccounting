using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using PA.Model.DataGrid;
using PA.Model.Database;
using PA.Helper.DataDefind;

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
                Log.Write(e.Message);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return result;
        }
        /// <summary>
        /// 获取一个值
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public string GetSelectValue(string sql)
        {
            return GetAllData(sql).Split('\t')[0].Split(',')[0];
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
                Log.Write(e.Message);
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
            string newTabname = TableName;
            if (TableName.LastIndexOf("_") > 1)
            {
                newTabname = TableName.Substring(0, TableName.LastIndexOf("_"));//TableName.Replace("_" + CommonInfo.账薄号,"");
            }
            string sql = "";
            try
            {
                switch (newTabname.ToUpper())
                {
                    case "T_BOOKS":
                        #region T_BOOKS
                        sql = PA.Helper.DataDefind.SqlString.Insert_T_BOOKS;
                        List<Model_账套> bookList = Values.OfType<Model_账套>().ToList();
                        foreach (Model_账套 list in bookList)
                        {
                            SQLiteCommand cmd = new SQLiteCommand();
                            cmd.CommandText = sql;
                            cmd.Parameters.AddWithValue("@ID", list.ID);
                            cmd.Parameters.AddWithValue("@BOOK_NAME", list.账套名称);
                            cmd.Parameters.AddWithValue("@COMPANY_NAME", list.单位名称);
                            cmd.Parameters.AddWithValue("@BOOK_TIME",list.启用期间);
                            cmd.Parameters.AddWithValue("@CREATE_DATE", list.创建时间);
                            cmd.Parameters.AddWithValue("@ACCOUNTING_SYSTEM", list.会计制度);
                            cmd.Parameters.AddWithValue("@PERIOD", list.当前期);
                            cmd.Connection = conn;
                            cmd.ExecuteNonQuery();
                        }
                        #endregion
                        break;
                    case "T_VOUCHER"://弃用，整合成InsertVoucherAll()
                        #region T_VOUCHER
                        //sql = PA.Helper.DataDefind.SqlString.Insert_T_VOUCHER;
                        //List<Model_凭证单> 凭证单List = Values.OfType<Model_凭证单>().ToList();
                        //foreach (Model_凭证单 list in 凭证单List)
                        //{
                        //    SQLiteCommand cmd = new SQLiteCommand();
                        //    cmd.CommandText = sql;
                        //    cmd.Parameters.AddWithValue("@ID", list.ID);
                        //    cmd.Parameters.AddWithValue("@PERIOD", list.当前期);
                        //    cmd.Parameters.AddWithValue("@OP_TIME", list.制表时间);
                        //    cmd.Parameters.AddWithValue("@SUBSIDIARY_COUNTS", list.附属单证数);
                        //    cmd.Parameters.AddWithValue("@FEE_DEBIT", list.合计借方金额);
                        //    cmd.Parameters.AddWithValue("@FEE_CREDIT", list.合计贷方金额);
                        //    cmd.Parameters.AddWithValue("@ACCOUNTANT", list.会计主管);
                        //    cmd.Parameters.AddWithValue("@BOOKEEPER", list.制单人);
                        //    cmd.Parameters.AddWithValue("@REVIEWER", list.复核);
                        //    cmd.Parameters.AddWithValue("@REVIEW_MARK", list.审核标志);
                        //    cmd.Parameters.AddWithValue("@DELETE_MARK", list.删除标志);
                        //    cmd.Connection = conn;
                        //    cmd.ExecuteNonQuery();
                        //}
                        #endregion
                        break;
                    case "T_VOUCHERDETAIL"://弃用，整合成InsertVoucherAll()
                        #region T_VOUCHERDETAIL
                        //sql = PA.Helper.DataDefind.SqlString.Insert_T_VOUCHER_DETAIL;
                        //List<Model_凭证明细> 凭证明细List = Values.OfType<Model_凭证明细>().ToList();
                        //foreach (Model_凭证明细 list in 凭证明细List)
                        //{
                        //    SQLiteCommand cmd = new SQLiteCommand();
                        //    cmd.CommandText = sql;
                        //    cmd.Parameters.AddWithValue("@VID", list.序号);
                        //    cmd.Parameters.AddWithValue("@PARENTID", list.父节点ID);
                        //    cmd.Parameters.AddWithValue("@WORD", list.凭证字);
                        //    cmd.Parameters.AddWithValue("@VOUCHER_NO", list.凭证号);
                        //    cmd.Parameters.AddWithValue("@ABSTRACT", list.摘要);
                        //    cmd.Parameters.AddWithValue("@SUBJECT_ID", list.科目编号);
                        //    cmd.Parameters.AddWithValue("@DETAIL", list.子细目);
                        //    cmd.Parameters.AddWithValue("@BOOKKEEP_MARK", list.记账);
                        //    cmd.Parameters.AddWithValue("@DEBIT", list.借方);
                        //    cmd.Parameters.AddWithValue("@CREDIT", list.贷方);
                        //    cmd.Connection = conn;
                        //    cmd.ExecuteNonQuery();
                        //}
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
                            cmd.Parameters.AddWithValue("@AUTHORITY", list.权限值);
                            cmd.Parameters.AddWithValue("@CREATE_TIME", list.创建日期);
                            cmd.Parameters.AddWithValue("@COMMENTS", list.用户说明);
                            cmd.Connection = conn;
                            cmd.ExecuteNonQuery();
                        }

                        #endregion
                        break;
                    case "T_RECORD":
                        #region T_RECORD
                        sql = PA.Helper.DataDefind.SqlString.Insert_T_RECORD;
                        List<Model_操作日志> 记录List = Values.OfType<Model_操作日志>().ToList();
                        foreach (Model_操作日志 list in 记录List)
                        {
                            SQLiteCommand cmd = new SQLiteCommand();
                            cmd.CommandText = sql;
                            cmd.Parameters.AddWithValue("@OP_TIME", list.日期);
                            cmd.Parameters.AddWithValue("@USERNAME", list.用户名);
                            cmd.Parameters.AddWithValue("@REALNAME", list.姓名);
                            cmd.Parameters.AddWithValue("@LOG", list.日志);
                            cmd.Connection = conn;
                            cmd.ExecuteNonQuery();
                        }

                        #endregion
                        break;
                    case "T_YEARFEE":
                    #region T_YEARFEE

                    #endregion
                        break;
                    case "T_SUBJECT":   //科目管理
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
                            cmd.Parameters.AddWithValue("@PARENT_ID", list.父ID);
                            cmd.Parameters.AddWithValue("@USED_MARK", list.是否启用);
                            cmd.Parameters.AddWithValue("@Borrow_Mark", list.借贷标记?1:-1);
                            cmd.Connection = conn;
                            cmd.ExecuteNonQuery();
                        }
                        #endregion
                        break;
                    case "T_FIXEDASSETS":   
                        #region 固定资产
                        sql = PA.Helper.DataDefind.SqlString.Insert_T_FIXEDASSETS;
                        List<Model_固定资产> 固定资产List = Values.OfType<Model_固定资产>().ToList();
                        foreach (Model_固定资产 list in 固定资产List)
                        {
                            SQLiteCommand cmd = new SQLiteCommand();
                            cmd.CommandText = sql;
                            cmd.Parameters.AddWithValue("@ID", list.编号);
                            cmd.Parameters.AddWithValue("@NAME", list.名称及规格);
                            cmd.Parameters.AddWithValue("@UNIT", list.单位);
                            cmd.Parameters.AddWithValue("@AMOUNT", list.数量);
                            cmd.Parameters.AddWithValue("@PRICE", list.价格);
                            cmd.Parameters.AddWithValue("@USED_YEAR", list.使用年限);
                            cmd.Parameters.AddWithValue("@BUY_DATE", list.购置日期);
                            cmd.Parameters.AddWithValue("@DEPARMENT", list.使用部门);
                            cmd.Parameters.AddWithValue("@CLEAR_DATE", list.报废日期);
                            cmd.Parameters.AddWithValue("@VOUCHER_NO", list.凭证编号);
                            cmd.Parameters.AddWithValue("@COMMENTS", list.备注);
                            cmd.Connection = conn;
                            cmd.ExecuteNonQuery();
                        }

                        #endregion
                        break;
                }
                strans.Commit();
                flag = true;
            }
            catch(Exception ee)
            {
                strans.Rollback();
                Log.Write(ee.Message);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return flag;
        }
        /// <summary>
        /// 插入凭证两张数据库表
        /// </summary>
        /// <param name="Voucher"></param>
        /// <param name="VoucherDetail"></param>
        /// <returns></returns>
        public bool InsertVoucherAll(List<object> Voucher, List<object> VoucherDetail)
        {
            bool flag = false;
            SQLiteConnection conn = DBInitialize.getDBConnection();
            conn.Open();
            SQLiteTransaction strans = conn.BeginTransaction();
            string sql = "";
            try
            {
                #region T_VOUCHER
                sql = PA.Helper.DataDefind.SqlString.Insert_T_VOUCHER;
                List<Model_凭证单> 凭证单List = Voucher.OfType<Model_凭证单>().ToList();
                foreach (Model_凭证单 list in 凭证单List)
                {
                    SQLiteCommand cmd = new SQLiteCommand();
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@ID", list.ID);
                    cmd.Parameters.AddWithValue("@PERIOD", list.当前期);
                    cmd.Parameters.AddWithValue("@OP_TIME", list.制表时间);
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

                #region T_VOUCHERDETAIL
                sql = PA.Helper.DataDefind.SqlString.Insert_T_VOUCHER_DETAIL;
                List<Model_凭证明细> 凭证明细List = VoucherDetail.OfType<Model_凭证明细>().ToList();
                foreach (Model_凭证明细 list in 凭证明细List)
                {
                    SQLiteCommand cmd = new SQLiteCommand();
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@VID", list.序号);
                    cmd.Parameters.AddWithValue("@PARENTID", list.父节点ID);
                    cmd.Parameters.AddWithValue("@WORD", list.凭证字);
                    cmd.Parameters.AddWithValue("@VOUCHER_NO", list.凭证号);
                    cmd.Parameters.AddWithValue("@ABSTRACT", list.摘要);
                    cmd.Parameters.AddWithValue("@SUBJECT_ID", list.科目编号);
                    cmd.Parameters.AddWithValue("@DETAIL", list.子细目ID);
                    cmd.Parameters.AddWithValue("@BOOKKEEP_MARK", list.记账);
                    cmd.Parameters.AddWithValue("@DEBIT", list.借方);
                    cmd.Parameters.AddWithValue("@CREDIT", list.贷方);
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                }
                #endregion
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

        //public bool UpdatePackage(List<UpdateParm> lists)
        //{
        //    bool flag = false;
        //    string sql = string.Empty;
        //    SQLiteConnection conn = DBInitialize.getDBConnection();
        //    conn.Open();
        //    SQLiteTransaction strans = conn.BeginTransaction();
        //    try
        //    {
        //        foreach (UpdateParm list in lists)
        //        {
        //            sql = PA.Helper.DataDefind.SqlString.Update_Sql;
        //            SQLiteCommand cmd = new SQLiteCommand();
        //            sql = sql.Replace("@tableName", list.TableName);
        //            sql = sql.Replace("@key", list.Key);
        //            sql = sql.Replace("@value", list.Value);
        //            sql = sql.Replace("@whereParm", list.WhereParm);
        //            cmd.CommandText = sql;
        //            cmd.Connection = conn;
        //            cmd.ExecuteNonQuery();
        //        }
        //        strans.Commit();
        //        flag = true;
        //    }
        //    catch(Exception ee)
        //    {
        //        strans.Rollback();
        //        Log.Write(ee.Message);
        //        Log.Write(sql);
        //    }
        //    finally
        //    {
        //        conn.Close();
        //        conn.Dispose();
        //    }
        //    return flag;
        //}

    }
}
