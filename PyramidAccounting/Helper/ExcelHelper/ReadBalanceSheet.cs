using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.Model.Others;
using xls = Microsoft.Office.Interop.Excel;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace PA.Helper.ExcelHelper
{
    /// <summary>
    /// 读资产负债表
    /// </summary>
    class ReadBalanceSheet
    {
        public List<Model_BalanceSheet> Read(string filepath, string sheetname)
        {
            //debug
            filepath = AppDomain.CurrentDomain.BaseDirectory + "Data\\资产负债表\\资产负债表-行政.xls";
            sheetname = "1997";

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
                {
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
                
            }
            return BalanceSheetDatas;
        }
    }
}
