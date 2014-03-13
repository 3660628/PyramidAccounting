using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xls = Microsoft.Office.Interop.Excel;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace PA.Helper.ExcelHelper
{
    class ExcelReader
    {
        xls.Application xlApp;
        xls.Workbook xlWorkBook;
        xls.Worksheet xlWorkSheet;
        object misValue = System.Reflection.Missing.Value;

        public DataSet ExcelDataSource(string filepath, string sheetname)
        {
            string strConn;
            strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filepath + ";Extended Properties=Excel 8.0;";
            OleDbConnection conn = new OleDbConnection(strConn);
            OleDbDataAdapter oada = new OleDbDataAdapter("select * from [" + sheetname + "$]", strConn);
            DataSet ds = new DataSet();
            oada.Fill(ds);
            return ds;
        }
    }
}
