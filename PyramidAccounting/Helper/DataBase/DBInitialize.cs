using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using PA.Helper.XMLHelper;
using PA.Helper.ExcelHelper;
using PA.Model.Others;
using PA.Helper.DataDefind;
using PA.Helper.Tools;

namespace PA.Helper.DataBase
{
    public class DBInitialize
    {
        public static string dataSource = "Data\\" + new XMLReader().ReadXML("数据库");
        private static string dbPassword = Secure.GetMD5_32(new UsbController().getSerialNumberFromDriveLetter());
        private XMLReader xr = new XMLReader();
        private XMLWriter xw = new XMLWriter();
        public DBInitialize()
        {
            Log.Write("DBInitialize");
        }

        /*
        public void Initialize()
        {
            //新建数据库
            if (dataSource.Equals("Data\\"))
            {
                dataSource += "PyramidAccounting.db";
            }
            SQLiteConnection.CreateFile(dataSource);
            SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
            SQLiteConnection conn = new SQLiteConnection();
            connstr.DataSource = dataSource;
            conn.ConnectionString = connstr.ToString();
            conn.Open();
            conn.Close();

            DataInitialize();
        }

        private void DataInitialize()
        {

            DataBase db = new DataBase();
            List<string> dataList = new List<string>();
            dataList = getSqlList(Properties.Resources.DatabaseTable);
            db.BatchOperate(dataList);

            dataList.Clear();
            dataList = GetSubjectSqlList();
            db.BatchOperate(dataList);

            //插入数据
            dataList.Clear();
            dataList = getSqlList(Properties.Resources.DatabaseData);
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //获取U盘唯一码
            UsbController usb = new UsbController();
            string _str = usb.getSerialNumberFromDriveLetter();
            string sql = "insert into t_systeminfo (op_time,rkey,value,comments) values ('" + date
                + "','" + (int)ENUM.EM_KEY.软件版本 + "','" + (int)ENUM.EM_SOFTWARESTATE.未注册 + "','第一次运行显示未注册'),('" + date
                + "','" + (int)ENUM.EM_KEY.注册码 + "','','注册码'),('" + date
                + "','" + (int)ENUM.EM_KEY.U盘标识 + "','" + _str + "','USB')";
            dataList.Add(sql);
            db.BatchOperate(dataList);
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
        
        private List<string> GetSubjectSqlList()
        {
            List<string> list = new List<string>();
            ExcelReader er = new ExcelReader();
            DirectoryInfo theFolder = new DirectoryInfo("Data\\科目");
            int i = 0;
            foreach (FileInfo newFile in theFolder.GetFiles())
            {
                List<Model_BalanceSheet> BalanceSheetDatas = new List<Model_BalanceSheet>();
                BalanceSheetDatas = er.ReadBalanceSheet(newFile.FullName);
                string baseTableName = "T_SUBJECT";
                string tableName = baseTableName + "_" + i;
                string table_Sql = new Helper.SQLHelper.SQLReader().ReadSQL(2, baseTableName,tableName);
                list.Add(table_Sql);
                foreach (Model_BalanceSheet m in BalanceSheetDatas)
                {
                    string sql = "insert into " + tableName + "(subject_id,subject_type,subject_name) values ('" + m.Number + "'," + m.Type + ",'" + m.Name + "')";
                    list.Add(sql);
                }
                i++;
            }
            return list;
        }
        */
        /// <summary>
        /// 获取数据库链接
        /// </summary>
        /// <returns></returns>
        public static SQLiteConnection GetDBConnection()
        {
            
            SQLiteConnection conn = new SQLiteConnection();
            SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
            connstr.DataSource = dataSource;
            if (new XMLReader().ReadXML("注册").Equals("true"))
            {
                conn.SetPassword(dbPassword);
            }
            conn.ConnectionString = connstr.ToString();
            return conn;
        }
        /// <summary>
        /// 清除数据库密码
        /// </summary>
        public static void ClearPassword()
        {
            SQLiteConnection conn = GetDBConnection();
            try
            {
                conn.Open();
                conn.ChangePassword("");
            }
            catch (Exception e)
            {
                Log.Write(e.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 修改数据库密码
        /// </summary>
        /// <returns></returns>
        public static bool ChangeDBPassword()
        {
            bool flag = false;
            SQLiteConnection conn = GetDBConnection();
            try
            {
                conn.Open();
                conn.ChangePassword(dbPassword);
                flag = true;
            }
            catch (Exception ee)
            {
                Log.Write(ee.Message);
            }
            finally
            {
                conn.Close();
            }
            return flag;
        }
        
    }
}
