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

namespace PA.Helper.ExcelHelper
{
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
            List<Model_报表类> data = new PA.ViewModel.ViewModel_ReportManager().GetBalanceSheet(ParmPeroid);
            if (data.Count < 1)
            {
                return "没有数据";
            }
            for (int i = 0; i < data.Count; i++)
            {
                if (i < 10)
                {
                    xlWorkSheet.Cells[6 + i, "C"] = data[i].年初数;
                    xlWorkSheet.Cells[6 + i, "D"] = data[i].期末数;
                    decimal.TryParse(data[i].年初数, out dy);
                    decimal.TryParse(data[i].期末数, out dn);
                    sumy1 += dy;
                    sumn1 += dn;
                }
                else if (i >= 10 && i < 16)
                {
                    xlWorkSheet.Cells[6 + i - 10, "G"] = data[i].年初数;
                    xlWorkSheet.Cells[6 + i - 10, "H"] = data[i].期末数;
                    decimal.TryParse(data[i].年初数, out dy);
                    decimal.TryParse(data[i].期末数, out dn);
                    sumy2 += dy;
                    sumn2 += dn;
                }
                else if (i >= 16 && i < 18)
                {
                    xlWorkSheet.Cells[i - 1, "G"] = data[i].年初数;
                    xlWorkSheet.Cells[i - 1, "H"] = data[i].期末数;
                    decimal.TryParse(data[i].年初数, out dy);
                    decimal.TryParse(data[i].期末数, out dn);
                    sumy3 += dy;
                    sumn3 += dn;
                }
                else if (i >= 18 && i < 21)
                {
                    xlWorkSheet.Cells[4 + i, "G"] = data[i].年初数;
                    xlWorkSheet.Cells[4 + i, "H"] = data[i].期末数;
                    decimal.TryParse(data[i].年初数, out dy);
                    decimal.TryParse(data[i].期末数, out dn);
                    sumy4 += dy;
                    sumn4 += dn;
                }
                else
                {
                    xlWorkSheet.Cells[i, "C"] = data[i].年初数;
                    xlWorkSheet.Cells[i, "D"] = data[i].期末数;
                    decimal.TryParse(data[i].年初数, out dy);
                    decimal.TryParse(data[i].期末数, out dn);
                    sumy5 += dy;
                    sumn5 += dn;
                }
            }
            xlWorkSheet.Cells[16, "C"] = sumy1;
            xlWorkSheet.Cells[16, "D"] = sumn1;
            xlWorkSheet.Cells[12, "G"] = sumy2;
            xlWorkSheet.Cells[12, "H"] = sumn2;
            xlWorkSheet.Cells[19, "G"] = sumy3;
            xlWorkSheet.Cells[19, "H"] = sumn3;
            xlWorkSheet.Cells[25, "G"] = sumy4;
            xlWorkSheet.Cells[25, "H"] = sumn4;
            xlWorkSheet.Cells[24, "C"] = sumy5;
            xlWorkSheet.Cells[24, "D"] = sumn5;
            xlWorkSheet.Cells[27, "C"] = sumy1 + sumy5;
            xlWorkSheet.Cells[27, "D"] = sumn1 + sumn5;
            xlWorkSheet.Cells[27, "G"] = sumy2 + sumy3 + sumy4;
            xlWorkSheet.Cells[27, "H"] = sumn2 + sumn3 + sumn4;
            xlWorkSheet.Cells[28, "D"] = People;
            xlWorkSheet.Cells[28, "E"] = Date;
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
            decimal b3 = 0;
            if (data.Count > 0)
            {
                xlWorkSheet.Cells[6, "B"] = data[0].本期数;
                xlWorkSheet.Cells[6, "C"] = data[0].累计数;
                decimal.TryParse(data[0].累计数, out dy);
                decimal.TryParse(data[0].本期数, out dn);
                insumm1 += dn;
                insumy1 += dy;
            }
            if (data.Count > 1)
            {
                xlWorkSheet.Cells[9, "B"] = data[1].本期数;
                xlWorkSheet.Cells[9, "C"] = data[1].累计数;
                decimal.TryParse(data[1].累计数, out dy);
                decimal.TryParse(data[1].本期数, out dn);
                insumm1 += dn;
                insumy1 += dy;
            }
            if (data.Count > 2)
            {
                xlWorkSheet.Cells[12, "B"] = data[2].本期数;
                xlWorkSheet.Cells[12, "C"] = data[2].累计数;
                decimal.TryParse(data[2].累计数, out dy);
                decimal.TryParse(data[2].本期数, out dn);
                insumm1 += dn;
                insumy1 += dy;
            }
            if (data.Count > 3)
            {
                xlWorkSheet.Cells[6, "E"] = data[3].本期数;
                xlWorkSheet.Cells[6, "F"] = data[3].累计数;
                decimal.TryParse(data[3].累计数, out dy);
                decimal.TryParse(data[3].本期数, out dn);
                insumy2 += dy;
                insumm2 += dn;
            }
            if (data.Count > 4)
            {
                xlWorkSheet.Cells[7, "E"] = data[4].本期数;
                xlWorkSheet.Cells[7, "F"] = data[4].累计数;
                decimal.TryParse(data[4].累计数, out dy);
                decimal.TryParse(data[4].本期数, out dn);
                insumy2 += dy;
                insumm2 += dn;
            }
            if (data.Count > 5)
            {
                xlWorkSheet.Cells[12, "E"] = data[5].本期数;
                xlWorkSheet.Cells[12, "F"] = data[5].累计数;
                decimal.TryParse(data[5].累计数, out dy);
                decimal.TryParse(data[5].本期数, out dn);
                insumy2 += dy;
                insumm2 += dn;
            }

            decimal.TryParse(data[data.Count - 1].累计数, out b3);

            xlWorkSheet.Cells[16, "B"] = insumm1;
            xlWorkSheet.Cells[16, "C"] = insumy1;

            xlWorkSheet.Cells[16, "E"] = insumm2;
            xlWorkSheet.Cells[16, "F"] = insumy2;

            data.Clear();
            data = new PA.ViewModel.ViewModel_ReportManager().GetIncomeAndExpensesForTwoSubject(ParmPeroid, new ViewModel.ViewModel_科目管理().GetIncomeAndOutSubjectList());
            if (data.Count > 0)
            {
                foreach (Model_报表类 a in data)
                {
                    if (a.编号 == "40101")
                    {
                        xlWorkSheet.Cells[7, "B"] = a.本期数;
                        xlWorkSheet.Cells[7, "C"] = a.累计数;
                    }
                    else if (a.编号 == "40102")
                    {
                        xlWorkSheet.Cells[8, "B"] = a.本期数;
                        xlWorkSheet.Cells[8, "C"] = a.累计数;
                    }
                    else if (a.编号 == "40401")
                    {
                        xlWorkSheet.Cells[10, "B"] = a.本期数;
                        xlWorkSheet.Cells[10, "C"] = a.累计数;
                    }
                    else if (a.编号 == "40402")
                    {
                        xlWorkSheet.Cells[11, "B"] = a.本期数;
                        xlWorkSheet.Cells[11, "C"] = a.累计数;
                    }
                }
            }
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
            //二级科目
            decimal b101 = 0;
            decimal b102 = 0;
            decimal b201 = 0;
            decimal b202 = 0;
            decimal b301 = 0;
            decimal b302 = 0;
            decimal b401 = 0;
            decimal b402 = 0;

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
                    if (dr[dc].ToString().StartsWith("@", false, null))
                    {
                        bool HasData = false;
                        string key = dr[dc].ToString().Replace("@", "");
                        foreach (Model_报表类 a in data)
                        {
                            if (key == a.编号)
                            {
                                xlWorkSheet.Cells[y + 1, x] = a.本期数;
                                xlWorkSheet.Cells[y + 1, x + 1] = a.累计数;
                                HasData = true;
                            }
                        }
                        if (!HasData)
                        {
                            xlWorkSheet.Cells[y + 1, x] = "";
                            xlWorkSheet.Cells[y + 1, x + 1] = "";
                        }
                    }
                    x++;
                }
                y++;
                x = 1;
            }

            //计算合计
            for (int i = 0; i < 7; i++)
            {
                decimal temp101 = 0m, temp102 = 0m;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i + 9, "B"]).Text, out temp101);
                b101 += temp101;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i + 9, "C"]).Text, out temp102);
                b102 += temp102;
            }
            for (int i = 17; i < 34; i++)
            {
                decimal temp201 = 0m, temp202 = 0m;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "B"]).Text, out temp201);
                b201 += temp201;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "C"]).Text, out temp202);
                b202 += temp202;
            }
            decimal temp201b = 0m, temp202b = 0m;
            decimal.TryParse(((xls.Range)xlWorkSheet.Cells[7, "E"]).Text, out temp201b);
            b201 += temp201b;
            decimal.TryParse(((xls.Range)xlWorkSheet.Cells[7, "F"]).Text, out temp202b);
            b202 += temp202b;
            for (int i = 0; i < 14; i++)
            {
                decimal temp301 = 0m, temp302 = 0m;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i + 9, "E"]).Text, out temp301);
                b301 += temp301;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i + 9, "F"]).Text, out temp302);
                b302 += temp302;
            }
            for (int i = 24; i < 30; i++)
            {
                decimal temp401 = 0m, temp402 = 0m;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "E"]).Text, out temp401);
                b401 += temp401;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "F"]).Text, out temp402);
                b402 += temp402;
            }

            xlWorkSheet.Cells[8, "B"] = b101;
            xlWorkSheet.Cells[8, "C"] = b102;
            xlWorkSheet.Cells[16, "B"] = b201;
            xlWorkSheet.Cells[16, "C"] = b202;
            xlWorkSheet.Cells[8, "E"] = b301;
            xlWorkSheet.Cells[8, "F"] = b302;
            xlWorkSheet.Cells[23, "E"] = b401;
            xlWorkSheet.Cells[23, "F"] = b402;

            xlWorkSheet.Cells[7, "B"] = (b101 + b201 + b301 + b401);
            xlWorkSheet.Cells[7, "C"] = (b102 + b202 + b302 + b402);
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
