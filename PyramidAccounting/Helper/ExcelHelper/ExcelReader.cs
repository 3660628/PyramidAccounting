using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xls = Microsoft.Office.Interop.Excel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using PA.Model.Others;

namespace PA.Helper.ExcelHelper
{
    class ExcelReader
    {
        //xls.Application xlApp;
        //xls.Workbook xlWorkBook;
        //xls.Worksheet xlWorkSheet;
        object misValue = System.Reflection.Missing.Value;

        /// <summary>
        /// 读取Excel内容到DataSet
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <param name="sheetname">Sheet表名</param>
        /// <returns></returns>
        public DataSet ExcelDataSource(string filepath, string sheetname)
        {
            string strConn;
            strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filepath + ";Extended Properties=Excel 8.0;";
            OleDbConnection conn = new OleDbConnection(strConn);
            OleDbDataAdapter oada = new OleDbDataAdapter("select * from [" + sheetname + "$]", strConn);
            DataSet ds = new DataSet();
            try
            {
                oada.Fill(ds);
            }
            catch(Exception ee)
            {
                PA.Helper.DataBase.Log.Write("ExcelDataSource Fill ERROR::filepath:" + filepath + ";sheetname:" + sheetname + "\n\t" + ee.ToString());
                return null;
            }
            finally
            {
                oada.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return ds;
        }

        /// <summary>
        /// 读取科目表
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public List<Model_BalanceSheet> ReadBalanceSheet(string filepath)
        {
            //debug
            //filepath = AppDomain.CurrentDomain.BaseDirectory + "Data\\资产负债表\\资产负债表-行政.xls";
            string sheetname = "科目";

            List<Model_BalanceSheet> BalanceSheetDatas = new List<Model_BalanceSheet>();
            Model_BalanceSheet BalanceSheetData = new Model_BalanceSheet();
            string Type = "0";

            string strConn;
            strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filepath + ";Extended Properties=Excel 8.0;";
            OleDbConnection conn = new OleDbConnection(strConn);
            OleDbDataAdapter oada = new OleDbDataAdapter("select * from [" + sheetname + "$]", strConn);
            DataSet ds = new DataSet();
            oada.Fill(ds);
            DataTable dt = ds.Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                BalanceSheetData = new Model_BalanceSheet();
                if (dr[0].ToString().Length <= 2)
                {
                    Type = dr[0].ToString();
                }
                else
                {
                    BalanceSheetData.Number = int.Parse(dr[0].ToString());
                    BalanceSheetData.Name = dr[1].ToString();
                    BalanceSheetData.Type = int.Parse(Type);
                    BalanceSheetData.DepartmentType = (Type == "1" || Type == "5") ? 1 : 2;
                    BalanceSheetDatas.Add(BalanceSheetData);
                }
            }
            conn.Close();
            return BalanceSheetDatas;
        }
    }
}
