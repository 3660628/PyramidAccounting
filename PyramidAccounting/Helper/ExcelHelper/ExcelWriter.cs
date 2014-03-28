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
    public class ExcelWriter
    {
        private string Path = AppDomain.CurrentDomain.BaseDirectory;
        xls.Application xlApp;
        xls.Workbook xlWorkBook;
        xls.Worksheet xlWorkSheet;
        object misValue = System.Reflection.Missing.Value;

        public ExcelWriter()
        {
            try
            {
                xlApp = new xls.Application();
            }
            catch (Exception)
            {
                Console.WriteLine("找不到EXCEL");
            }
        }

        /// <summary>
        /// 记账凭证
        /// </summary>
        public void ExportVouchers()
        {
            string SourceXls = Path + @"Data\打印\记账凭证模板.xls";
            string ExportXls = Path + @"Data\打印\记账凭证export.xls";
            File.Copy(SourceXls, ExportXls, true);
            xlWorkBook = xlApp.Workbooks.Open(ExportXls);
            xlWorkSheet = (xls.Worksheet)xlWorkBook.Worksheets.get_Item(1);






            xlApp.Visible = true;

            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                Console.WriteLine("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
