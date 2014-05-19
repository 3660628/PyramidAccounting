using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using xls = Microsoft.Office.Interop.Excel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using PA.Model.DataGrid;
using PA.Helper.DataDefind;

namespace PA.Helper.ExcelHelper
{
    /// <summary>
    /// 事业单位报表导出Excel
    /// </summary>
    class InstitutionsExcelWriter
    {
        private string Path = AppDomain.CurrentDomain.BaseDirectory;
        private object misValue = System.Reflection.Missing.Value;
        private string DateNow = "";

        public InstitutionsExcelWriter()
        {
            DateNow = DateTime.Now.ToString("_yyyyMMddHHmmss");
        }

        #region 3.报表
        /// <summary>
        /// 资产负债表（事业）
        /// </summary>
        /// <param name="ParmPeroid"></param>
        /// <param name="People"></param>
        /// <param name="Date"></param>
        /// <returns></returns>
        public string ExportBalanceSheet(int ParmPeroid, string People, string Date)
        {
            string result = "";
            #region init Excel
            xls.Application xlApp = null;
            xls.Workbook xlWorkBook;
            xls.Worksheet xlWorkSheet;
            string SourceXls = Path + @"Data\打印\资产负债表（事业）模板.xls";
            string ExportXls = Path + @"Excel\打印\资产负债表（事业）" + DateNow + ".xls";
            try
            {
                File.Copy(SourceXls, ExportXls, true);
            }
            catch (FileNotFoundException)
            {
                return "模板文件未找到";
            }
            catch (IOException)
            {
                return "文件锁定，请关闭Excel再试";
            }
            try
            {
                xlApp = new xls.Application();
            }
            catch (Exception)
            {
                return "找不到EXCEL软件";
            }
            xlWorkBook = xlApp.Workbooks.Open(ExportXls);
            xlWorkSheet = (xls.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            #endregion

            #region fill data
            List<Model_报表类> data = new PA.ViewModel.ViewModel_ReportManager().GetBalanceSheet(ParmPeroid);
            if (data.Count < 1)
            {
                return "没有数据";
            }
            decimal dy = 0;
            decimal dn = 0;
            decimal sumy1 = 0;
            decimal sumn1 = 0;
            decimal sumy2 = 0;
            decimal sumn2 = 0;
            decimal sumy3 = 0;
            decimal sumn3 = 0;
            decimal sumy4 = 0;
            decimal sumn4 = 0;
            decimal sumy5 = 0;
            decimal sumn5 = 0;

            int x = 1, y = 1;
            DataSet ds;
            if (!new PA.Helper.ExcelHelper.ExcelReader().ExcelDataSource(SourceXls, "Sheet1", out ds))
            {
                return "出错了，请联系管理员。";
            }
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                foreach (DataColumn dc in ds.Tables[0].Columns)
                {
                    string key = dr[dc].ToString();
                    foreach (Model_报表类 a in data)
                    {
                        if (key == "y" + a.编号)
                        {
                            xlWorkSheet.Cells[y + 1, x] = a.年初数;
                            xlWorkSheet.Cells[y + 1, x + 1] = a.期末数;
                            decimal.TryParse(a.年初数, out dy);
                            decimal.TryParse(a.期末数, out dn);
                            if (a.编号.StartsWith("1"))
                            {
                                sumy1 += dy;
                                sumn1 += dn;
                            }
                            else if (a.编号.StartsWith("2"))
                            {
                                sumy2 += dy;
                                sumn2 += dn;
                            }
                            else if (a.编号.StartsWith("3"))
                            {
                                sumy3 += dy;
                                sumn3 += dn;
                            }
                            else if (a.编号.StartsWith("4"))
                            {
                                sumy4 += dy;
                                sumn4 += dn;
                            }
                            else if (a.编号.StartsWith("5"))
                            {
                                sumy5 += dy;
                                sumn5 += dn;
                            }
                        }
                    }
                    x++;
                }
                y++;
                x = 1;
            }
            xlWorkSheet.Cells[18, "C"] = sumy1;
            xlWorkSheet.Cells[18, "D"] = sumn1;
            xlWorkSheet.Cells[30, "C"] = sumy5;
            xlWorkSheet.Cells[30, "D"] = sumn5;
            xlWorkSheet.Cells[16, "G"] = sumy2;
            xlWorkSheet.Cells[16, "H"] = sumn2;
            xlWorkSheet.Cells[25, "G"] = sumy3;
            xlWorkSheet.Cells[25, "H"] = sumn3;
            xlWorkSheet.Cells[34, "G"] = sumy4;
            xlWorkSheet.Cells[34, "H"] = sumn4;

            xlWorkSheet.Cells[35, "C"] = sumy1 + sumy5;
            xlWorkSheet.Cells[35, "D"] = sumn1 + sumn5;
            xlWorkSheet.Cells[35, "G"] = sumy2 + sumy3 + sumy4;
            xlWorkSheet.Cells[35, "H"] = sumn2 + sumn3 + sumn4;

            xlWorkSheet.Cells[36, "A"] = "单位负责人：" + CommonInfo.真实姓名;
            xlWorkSheet.Cells[36, "C"] = "填表人：" + CommonInfo.用户权限 + "\t" + CommonInfo.真实姓名;
            xlWorkSheet.Cells[36, "F"] = "填表日期：" + DateTime.Now.ToLongDateString();
            #endregion

            xlApp.Visible = true;
            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
            return result;
        }
        /// <summary>
        /// 收入支出总表（事业）
        /// </summary>
        /// <param name="ParmPeroid"></param>
        /// <returns></returns>
        public string ExportIncomeAndExpenditure(int ParmPeroid)
        {
            string result = "";
            #region init Excel
            xls.Application xlApp = null;
            xls.Workbook xlWorkBook;
            xls.Worksheet xlWorkSheet;
            string SourceXls = Path + @"Data\打印\收入支出总表（事业）模板.xls";
            string ExportXls = Path + @"Excel\打印\收入支出总表（事业）" + DateNow + ".xls";
            try
            {
                File.Copy(SourceXls, ExportXls, true);
            }
            catch (FileNotFoundException)
            {
                return "模板文件未找到";
            }
            catch (IOException)
            {
                return "文件锁定，请关闭Excel再试";
            }
            try
            {
                xlApp = new xls.Application();
            }
            catch (Exception)
            {
                return "找不到EXCEL软件";
            }
            xlWorkBook = xlApp.Workbooks.Open(ExportXls);
            xlWorkSheet = (xls.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            #endregion

            #region fill data
            List<Model_报表类> data = new PA.ViewModel.ViewModel_ReportManager().GetIncomeAndExpenses(ParmPeroid, new ViewModel.ViewModel_科目管理().GetOneSubjectList());
            if (data.Count <= 0)
            {
                return "没有数据";
            }

            decimal dy = 0;
            decimal dn = 0;
            decimal insumm1 = 0;
            decimal insumy1 = 0;
            decimal insumm2 = 0;
            decimal insumy2 = 0;

            int x = 1, y = 1;
            DataSet ds;
            if (!new PA.Helper.ExcelHelper.ExcelReader().ExcelDataSource(SourceXls, "Sheet1", out ds))
            {
                return "出错了，请联系管理员。";
            }
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                foreach (DataColumn dc in ds.Tables[0].Columns)
                {
                    string key = dr[dc].ToString();
                    foreach (Model_报表类 m in data)
                    {
                        if (key == "inM" + m.编号)
                        {
                            xlWorkSheet.Cells[y + 1, x] = m.本期数;
                            xlWorkSheet.Cells[y + 1, x + 1] = m.累计数;
                            decimal.TryParse(m.累计数, out dy);
                            decimal.TryParse(m.本期数, out dn);
                            if (m.编号.StartsWith("4"))
                            {
                                insumm1 += dn;
                                insumy1 += dy;
                            }
                            else if (m.编号.StartsWith("5"))
                            {
                                insumy2 += dy;
                                insumm2 += dn;
                            }
                            else if (m.编号.StartsWith("3"))
                            {
                                xlWorkSheet.Cells[6, "H"] = m.累计数;
                            }
                        }
                    }
                    x++;
                }
                y++;
                x = 1;
            }
            xlWorkSheet.Cells[22, "B"] = insumm1;
            xlWorkSheet.Cells[22, "C"] = insumy1;
            xlWorkSheet.Cells[22, "E"] = insumm2;
            xlWorkSheet.Cells[22, "F"] = insumy2;

            #endregion

            xlApp.Visible = true;
            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
            return result;
        }
        /// <summary>
        /// 事业及经营支出明细表
        /// </summary>
        /// <param name="ParmPeroid"></param>
        /// <returns></returns>
        public string ExportAdministrativeExpensesSchedule(int ParmPeroid)
        {
            string result = "";
            #region init Excel
            xls.Application xlApp = null;
            xls.Workbook xlWorkBook;
            xls.Worksheet xlWorkSheet;
            string SourceXls = Path + @"Data\打印\事业及经营支出明细表.xls";
            string ExportXls = Path + @"Excel\打印\事业及经营支出明细表" + DateNow + ".xls";
            try
            {
                File.Copy(SourceXls, ExportXls, true);
            }
            catch (FileNotFoundException)
            {
                return "模板文件未找到";
            }
            catch (IOException)
            {
                return "文件锁定，请关闭Excel再试";
            }
            try
            {
                xlApp = new xls.Application();
            }
            catch (Exception)
            {
                return "找不到EXCEL软件";
            }
            xlWorkBook = xlApp.Workbooks.Open(ExportXls);
            xlWorkSheet = (xls.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            #endregion

            #region fill data
            List<Model_报表类> data = new PA.ViewModel.ViewModel_ReportManager().GetAdministrativeExpenseDetail(ParmPeroid, 501);
            if (data.Count <= 0)
            {
                return "没有数据";
            }





            #endregion

            xlApp.Visible = true;
            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
            return result;
        }
        #endregion

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
