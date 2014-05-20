﻿using System;
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
            List<Model_报表类> data = new PA.ViewModel.ViewModel_ReportManager().GetAdministrativeExpenseDetail(ParmPeroid, 504);
            if (data.Count <= 0)
            {
                return "没有数据";
            }

            decimal dn = 0;

            //二级科目
            decimal b101 = 0;
            decimal b102 = 0;
            decimal b103 = 0;
            decimal b104 = 0;
            decimal b105 = 0;
            decimal b106 = 0;
            decimal b201 = 0;
            decimal b202 = 0;
            decimal b203 = 0;
            decimal b204 = 0;
            decimal b205 = 0;
            decimal b206 = 0;
            decimal b301 = 0;
            decimal b302 = 0;
            decimal b303 = 0;
            decimal b304 = 0;
            decimal b305 = 0;
            decimal b306 = 0;
            decimal b401 = 0;
            decimal b402 = 0;
            decimal b403 = 0;
            decimal b404 = 0;
            decimal b405 = 0;
            decimal b406 = 0;
            decimal b501 = 0;
            decimal b502 = 0;
            decimal b503 = 0;
            decimal b504 = 0;
            decimal b505 = 0;
            decimal b506 = 0;
            decimal b601 = 0;
            decimal b602 = 0;
            decimal b603 = 0;
            decimal b604 = 0;
            decimal b605 = 0;
            decimal b606 = 0;
            decimal b701 = 0;
            decimal b702 = 0;
            decimal b703 = 0;
            decimal b704 = 0;
            decimal b705 = 0;
            decimal b706 = 0;

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
                        if (key == "Label_A" + m.编号.Replace("（", "").Replace("）", ""))
                        {
                            xlWorkSheet.Cells[y + 1, x] = m.本期数;
                            xlWorkSheet.Cells[y + 1, x + 1] = m.累计数;
                            //xlWorkSheet.Cells[y + 1, x + 2] = m.本期数1;
                            //xlWorkSheet.Cells[y + 1, x + 3] = m.累计数1;
                            //xlWorkSheet.Cells[y + 1, x + 4] = m.本期数2;
                            //xlWorkSheet.Cells[y + 1, x + 5] = m.累计数2;
                        }
                    }
                    x++;
                }
                y++;
                x = 1;
            }

            //计算合计
            //工资福利支出
            for (int i = 8; i < 15; i++)
            {
                decimal temp101 = 0m, temp201 = 0m, temp301 = 0m, temp401 = 0m, temp501 = 0m, temp601 = 0m;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "B"]).Text, out temp101);
                b101 += temp101;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "C"]).Text, out temp201);
                b102 += temp201;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "D"]).Text, out temp301);
                b103 += temp301;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "E"]).Text, out temp401);
                b104 += temp401;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "F"]).Text, out temp501);
                b105 += temp501;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "G"]).Text, out temp601);
                b106 += temp601;
            }
            //商品和服务支出
            for (int i = 16; i < 33; i++)
            {
                decimal temp101 = 0m, temp201 = 0m, temp301 = 0m, temp401 = 0m, temp501 = 0m, temp601 = 0m;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "B"]).Text, out temp101);
                b201 += temp101;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "C"]).Text, out temp201);
                b202 += temp201;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "D"]).Text, out temp301);
                b203 += temp301;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "E"]).Text, out temp401);
                b204 += temp401;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "F"]).Text, out temp501);
                b205 += temp501;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "G"]).Text, out temp601);
                b206 += temp601;
            }
            b201 += decimal.Parse(((xls.Range)xlWorkSheet.Cells[6, "I"]).Text);
            b202 += decimal.Parse(((xls.Range)xlWorkSheet.Cells[6, "J"]).Text);
            b203 += decimal.Parse(((xls.Range)xlWorkSheet.Cells[6, "K"]).Text);
            b204 += decimal.Parse(((xls.Range)xlWorkSheet.Cells[6, "L"]).Text);
            b205 += decimal.Parse(((xls.Range)xlWorkSheet.Cells[6, "M"]).Text);
            b206 += decimal.Parse(((xls.Range)xlWorkSheet.Cells[6, "N"]).Text);
            //对个人和家庭的补助
            for (int i = 8; i < 15; i++)
            {
                decimal T1 = 0m, T2 = 0m, T3 = 0m, T4 = 0m, T5 = 0m, T6 = 0m;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "I"]).Text, out T1);
                b301 += T1;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "J"]).Text, out T2);
                b302 += T2;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "K"]).Text, out T3);
                b303 += T3;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "L"]).Text, out T4);
                b304 += T4;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "M"]).Text, out T5);
                b305 += T5;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "N"]).Text, out T6);
                b306 += T6;
            }
            //对企事业单位的补贴
            for (int i = 16; i < 18; i++)
            {
                decimal T1 = 0m, T2 = 0m, T3 = 0m, T4 = 0m, T5 = 0m, T6 = 0m;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "I"]).Text, out T1);
                b401 += T1;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "J"]).Text, out T2);
                b402 += T2;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "K"]).Text, out T3);
                b403 += T3;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "L"]).Text, out T4);
                b404 += T4;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "M"]).Text, out T5);
                b405 += T5;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "N"]).Text, out T6);
                b406 += T6;
            }
            //债务利息支出
            for (int i = 19; i < 20; i++)
            {
                decimal T1 = 0m, T2 = 0m, T3 = 0m, T4 = 0m, T5 = 0m, T6 = 0m;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "I"]).Text, out T1);
                b501 += T1;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "J"]).Text, out T2);
                b502 += T2;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "K"]).Text, out T3);
                b503 += T3;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "L"]).Text, out T4);
                b504 += T4;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "M"]).Text, out T5);
                b505 += T5;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "N"]).Text, out T6);
                b506 += T6;
            }
            //其他资本性支出
            for (int i = 21; i < 30; i++)
            {
                decimal T1 = 0m, T2 = 0m, T3 = 0m, T4 = 0m, T5 = 0m, T6 = 0m;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "I"]).Text, out T1);
                b601 += T1;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "J"]).Text, out T2);
                b602 += T2;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "K"]).Text, out T3);
                b603 += T3;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "L"]).Text, out T4);
                b604 += T4;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "M"]).Text, out T5);
                b605 += T5;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "N"]).Text, out T6);
                b606 += T6;
            }
            //其他支出
            for (int i = 31; i < 33; i++)
            {
                decimal T1 = 0m, T2 = 0m, T3 = 0m, T4 = 0m, T5 = 0m, T6 = 0m;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "I"]).Text, out T1);
                b701 += T1;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "J"]).Text, out T2);
                b702 += T2;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "K"]).Text, out T3);
                b703 += T3;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "L"]).Text, out T4);
                b704 += T4;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "M"]).Text, out T5);
                b705 += T5;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i, "N"]).Text, out T6);
                b706 += T6;
            }

            xlWorkSheet.Cells[7, "B"] = b101;
            xlWorkSheet.Cells[7, "C"] = b102;
            xlWorkSheet.Cells[7, "D"] = b103;
            xlWorkSheet.Cells[7, "E"] = b104;
            xlWorkSheet.Cells[7, "F"] = b105;
            xlWorkSheet.Cells[7, "G"] = b106;

            xlWorkSheet.Cells[15, "B"] = b201;
            xlWorkSheet.Cells[15, "C"] = b202;
            xlWorkSheet.Cells[15, "D"] = b203;
            xlWorkSheet.Cells[15, "E"] = b204;
            xlWorkSheet.Cells[15, "F"] = b205;
            xlWorkSheet.Cells[15, "G"] = b206;

            xlWorkSheet.Cells[7, "I"] = b301;
            xlWorkSheet.Cells[7, "J"] = b302;
            xlWorkSheet.Cells[7, "K"] = b303;
            xlWorkSheet.Cells[7, "L"] = b304;
            xlWorkSheet.Cells[7, "M"] = b305;
            xlWorkSheet.Cells[7, "N"] = b306;

            xlWorkSheet.Cells[15, "I"] = b401;
            xlWorkSheet.Cells[15, "J"] = b402;
            xlWorkSheet.Cells[15, "K"] = b403;
            xlWorkSheet.Cells[15, "L"] = b404;
            xlWorkSheet.Cells[15, "M"] = b405;
            xlWorkSheet.Cells[15, "N"] = b406;

            xlWorkSheet.Cells[18, "I"] = b501;
            xlWorkSheet.Cells[18, "J"] = b502;
            xlWorkSheet.Cells[18, "K"] = b503;
            xlWorkSheet.Cells[18, "L"] = b504;
            xlWorkSheet.Cells[18, "M"] = b505;
            xlWorkSheet.Cells[18, "N"] = b506;

            xlWorkSheet.Cells[20, "I"] = b601;
            xlWorkSheet.Cells[20, "J"] = b602;
            xlWorkSheet.Cells[20, "K"] = b603;
            xlWorkSheet.Cells[20, "L"] = b604;
            xlWorkSheet.Cells[20, "M"] = b605;
            xlWorkSheet.Cells[20, "N"] = b606;

            xlWorkSheet.Cells[30, "I"] = b701;
            xlWorkSheet.Cells[30, "J"] = b702;
            xlWorkSheet.Cells[30, "K"] = b703;
            xlWorkSheet.Cells[30, "L"] = b704;
            xlWorkSheet.Cells[30, "M"] = b705;
            xlWorkSheet.Cells[30, "N"] = b706;

            xlWorkSheet.Cells[6, "B"] = (b101 + b201 + b301 + b401 + b501 + b601 + b701);
            xlWorkSheet.Cells[6, "C"] = (b102 + b202 + b302 + b402 + b502 + b602 + b702);
            xlWorkSheet.Cells[6, "D"] = (b103 + b203 + b303 + b403 + b503 + b603 + b703);
            xlWorkSheet.Cells[6, "E"] = (b104 + b204 + b304 + b404 + b504 + b604 + b704);
            xlWorkSheet.Cells[6, "F"] = (b105 + b205 + b305 + b405 + b505 + b605 + b705);
            xlWorkSheet.Cells[6, "G"] = (b106 + b206 + b306 + b406 + b506 + b606 + b706);

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