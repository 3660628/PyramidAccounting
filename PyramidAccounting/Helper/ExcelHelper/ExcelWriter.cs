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
    public class ExcelWriter
    {
        private string Path = AppDomain.CurrentDomain.BaseDirectory;
        private xls.Application xlApp;
        private xls.Workbook xlWorkBook;
        private xls.Worksheet xlWorkSheet;
        private object misValue = System.Reflection.Missing.Value;

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
        public void ExportVouchers(Guid guid)
        {
            int SheetNum = new PA.ViewModel.ViewModel_凭证管理().GetPageNum(guid);
            Model_凭证单 Voucher = new PA.ViewModel.ViewModel_凭证管理().GetVoucher(guid);
            List<Model_凭证明细> VoucherDetails = new PA.ViewModel.ViewModel_凭证管理().GetVoucherDetails(guid);

            string SourceXls = Path + @"Data\打印\记账凭证模板.xls";
            string ExportXls = Path + @"Data\打印\记账凭证export.xls";
            File.Copy(SourceXls, ExportXls, true);
            xlWorkBook = xlApp.Workbooks.Open(ExportXls);
            xlWorkSheet = (xls.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            //fill Voucher
            int x = 1, y = 1;
            DataSet ds = new PA.Helper.ExcelHelper.ExcelReader().ExcelDataSource(ExportXls, "Sheet1");
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                foreach (DataColumn dc in ds.Tables[0].Columns)
                {
                    if (dr[dc].ToString().StartsWith("@", false, null))
                    {
                        string key = dr[dc].ToString().Replace("@", "");
                        System.Reflection.PropertyInfo[] propertiesVoucher = Voucher.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                        foreach (System.Reflection.PropertyInfo item in propertiesVoucher)
                        {
                            if (item.Name == key)
                            {
                                if (item.Name == "合计借方金额" || item.Name == "合计贷方金额")
                                {
                                    //xlWorkSheet.Cells[y + 1, x] = "";
                                    //if (SheetNow == SheetNum)
                                    //{
                                    //    string money = item.GetValue(Voucher, null).ToString();
                                    //    List<string> ListMoney = TransMoney(money);
                                    //    for (int i = ListMoney.Count - 1; i >= 0; i--)
                                    //    {
                                    //        xlWorkSheet.Cells[y + 1, x + 10 - i] = ListMoney[i];
                                    //    }
                                    //}
                                }
                                else if (item.Name == "制表时间")
                                {
                                    string format = "yyyy年MM月dd日";
                                    xlWorkSheet.Cells[y + 1, x] = DateTime.Parse(item.GetValue(Voucher, null).ToString()).ToString(format, DateTimeFormatInfo.InvariantInfo);
                                }
                                else
                                {
                                    xlWorkSheet.Cells[y + 1, x] = item.GetValue(Voucher, null);
                                }
                            }
                        }
                    }
                    x++;
                }
                y++;
                x = 1;
            }
            //copy sheet while SheetNum>1 after fill Voucher Data
            for (int i = 1; i < SheetNum; i++)
            {
                xlWorkSheet.Copy(Type.Missing, xlWorkBook.Sheets[i]);
                xlWorkBook.Worksheets.get_Item(i + 1).Name = "Sheet" + (i + 1);
            }
            //fill VoucherDetails
            for (int i = 0; i < SheetNum; i++)
            {
                xlWorkSheet = (xls.Worksheet)xlWorkBook.Worksheets.get_Item(i+1);
                int xDetails = 1, yDetails = 1;
                DataSet dsDetails = new PA.Helper.ExcelHelper.ExcelReader().ExcelDataSource(ExportXls, "Sheet"+(i+1));
                foreach (DataRow dr in dsDetails.Tables[0].Rows)
                {
                    foreach (DataColumn dc in dsDetails.Tables[0].Columns)
                    {
                        if (dr[dc].ToString().StartsWith("@", false, null))
                        {
                            string key = dr[dc].ToString().Replace("@", "");
                            if (key.StartsWith("摘要", false, null) || key.StartsWith("科目", false, null) || key.StartsWith("子细目", false, null) || key.StartsWith("记账", false, null))
                            {
                                int id = int.Parse(key.Substring(key.Length - 1, 1)) - 1;
                                if (id < VoucherDetails.Count)
                                {
                                    if (key.StartsWith("摘要", false, null))
                                    {
                                        xlWorkSheet.Cells[yDetails + 1, xDetails] = VoucherDetails[id + (i * 6)].摘要;
                                    }
                                    else if (key.StartsWith("科目", false, null))
                                    {
                                        xlWorkSheet.Cells[yDetails + 1, xDetails] = VoucherDetails[id + (i * 6)].科目编号;
                                    }
                                    else if (key.StartsWith("子细目", false, null))
                                    {
                                        xlWorkSheet.Cells[yDetails + 1, xDetails] = VoucherDetails[id + (i * 6)].子细目;
                                    }
                                    else if (key.StartsWith("记账", false, null))
                                    {
                                        xlWorkSheet.Cells[yDetails + 1, xDetails] = (VoucherDetails[id + (i * 6)].记账 == 1) ? "√" : "";
                                    }
                                }
                            }
                            else if (key.StartsWith("借方", false, null) || key.StartsWith("贷方", false, null))
                            {
                                xlWorkSheet.Cells[yDetails + 1, xDetails] = "";
                                int id = int.Parse(key.Substring(key.Length - 1, 1)) - 1;
                                if (id < VoucherDetails.Count)
                                {
                                    string money = string.Empty;
                                    if (key.StartsWith("借方", false, null))
                                    {
                                        money = VoucherDetails[id + (i * 6)].借方.ToString();
                                    }
                                    else if (key.StartsWith("贷方", false, null))
                                    {
                                        money = VoucherDetails[id + (i * 6)].贷方.ToString();
                                    }
                                    if (money == "0")
                                    {
                                        xDetails++;
                                        continue;
                                    }
                                    List<string> ListMoney = TransMoney(money);
                                    for (int j = ListMoney.Count - 1; j >= 0; j--)
                                    {
                                        xlWorkSheet.Cells[yDetails + 1, xDetails + 10 - j] = ListMoney[j];
                                    }
                                }
                            }
                            else if (key.StartsWith("号", false, null))
                            {
                                xlWorkSheet.Cells[yDetails + 1, xDetails] = VoucherDetails[(i * 6)].凭证号;
                            }
                            //fill total money while the sheet is the last one
                            else if (key.StartsWith("合计借方金额", false, null) || key.StartsWith("合计贷方金额", false, null))
                            {
                                xlWorkSheet.Cells[yDetails + 1, xDetails] = "";
                                if(i==(SheetNum-1))
                                {
                                    List<string> ListMoney = (key.StartsWith("合计借方金额", false, null)) ? TransMoney(Voucher.合计借方金额.ToString()) : TransMoney(Voucher.合计贷方金额.ToString());
                                    for (int j = ListMoney.Count - 1; j >= 0; j--)
                                    {
                                        xlWorkSheet.Cells[yDetails + 1, xDetails + 10 - j] = ListMoney[j];
                                    }
                                }
                            }
                        }
                        xDetails++;
                    }
                    yDetails++;
                    xDetails = 1;
                }
            }
            xlApp.Visible = true;

            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
        }

        /// <summary>
        /// 将string类型的金额改成倒序List数组
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        private List<string> TransMoney(string parm)
        {
            List<string> result = new List<string>();
            string m1 = string.Empty;
            if(parm.IndexOf('.') > 0)
            {
                m1 = parm.Split('.')[0];
                string m2 = parm.Split('.')[1];
                result.Add((m2.Length == 2) ? m2.Substring(1, 1) : "0");
                result.Add(m2.Substring(0, 1));
            }
            else
            {
                m1 = parm;
                result.Add("0");
                result.Add("0");
            }
            for (int i = m1.Length-1; i >= 0; i--)
            {
                result.Add(m1.Substring(i, 1));
            }
            return result;
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
