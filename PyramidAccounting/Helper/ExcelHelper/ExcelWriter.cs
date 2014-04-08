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
                                        xlWorkSheet.Cells[yDetails + 1, xDetails] = VoucherDetails[id + (i * 6)].主科目名;
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
                                    List<string> ListMoney = new ExcelHelper().TransMoney(money);
                                    for (int j = ListMoney.Count - 1; j >= 0; j--)
                                    {
                                        xlWorkSheet.Cells[yDetails + 1, xDetails + 10 - j] = ListMoney[j];
                                    }
                                }
                            }
                            else if (key.StartsWith("号", false, null))
                            {
                                xlWorkSheet.Cells[yDetails + 1, xDetails] = VoucherDetails[(i * 6)].凭证号 + "号";
                            }
                            //fill total money while the sheet is the last one
                            else if (key.StartsWith("合计借方金额", false, null) || key.StartsWith("合计贷方金额", false, null))
                            {
                                xlWorkSheet.Cells[yDetails + 1, xDetails] = "";
                                if(i==(SheetNum-1))
                                {
                                    List<string> ListMoney = (key.StartsWith("合计借方金额", false, null)) ? new ExcelHelper().TransMoney(Voucher.合计借方金额.ToString()) : new ExcelHelper().TransMoney(Voucher.合计贷方金额.ToString());
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
        /// 总账
        /// </summary>
        /// <param name="Parm"></param>
        /// <returns></returns>
        public bool ExportLedger(string DetailsData)
        {
            Console.WriteLine("Print Ledger");
            const int PageLine = 47;

            List<Model_总账> LedgerData = new PA.ViewModel.ViewModel_账薄管理().GetTotalFee(DetailsData);
            string SourceXls = Path + @"Data\打印\总账模板.xls";
            string ExportXls = Path + @"Data\打印\总账export.xls";
            File.Copy(SourceXls, ExportXls, true);
            xlWorkBook = xlApp.Workbooks.Open(ExportXls);
            xlWorkSheet = (xls.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            //fill data
            xlWorkSheet.Cells[1,6] = "总账";
            xlWorkSheet.Cells[3, 2] = "1/" + (LedgerData.Count / PageLine + 1);
            xlWorkSheet.Cells[3, 30] = DetailsData.Split('\t')[1];
            for (int i = 0; i < LedgerData.Count; i++)
            {
                xlWorkSheet.Cells[9 + i, 1] = LedgerData[i].月;
                xlWorkSheet.Cells[9 + i, 2] = LedgerData[i].日;
                xlWorkSheet.Cells[9 + i, 3] = LedgerData[i].号数;
                xlWorkSheet.Cells[9 + i, 5] = LedgerData[i].摘要;
                xlWorkSheet.Cells[9 + i, 7] = LedgerData[i].借方金额2;
                xlWorkSheet.Cells[9 + i, 8] = LedgerData[i].借方金额3;
                xlWorkSheet.Cells[9 + i, 9] = LedgerData[i].借方金额4;
                xlWorkSheet.Cells[9 + i, 10] = LedgerData[i].借方金额5;
                xlWorkSheet.Cells[9 + i, 11] = LedgerData[i].借方金额6;
                xlWorkSheet.Cells[9 + i, 12] = LedgerData[i].借方金额7;
                xlWorkSheet.Cells[9 + i, 13] = LedgerData[i].借方金额8;
                xlWorkSheet.Cells[9 + i, 14] = LedgerData[i].借方金额9;
                xlWorkSheet.Cells[9 + i, 15] = LedgerData[i].借方金额10;
                xlWorkSheet.Cells[9 + i, 16] = LedgerData[i].借方金额11;
                xlWorkSheet.Cells[9 + i, 17] = LedgerData[i].借方金额12;
                xlWorkSheet.Cells[9 + i, 18] = LedgerData[i].贷方金额2;
                xlWorkSheet.Cells[9 + i, 19] = LedgerData[i].贷方金额3;
                xlWorkSheet.Cells[9 + i, 20] = LedgerData[i].贷方金额4;
                xlWorkSheet.Cells[9 + i, 21] = LedgerData[i].贷方金额5;
                xlWorkSheet.Cells[9 + i, 22] = LedgerData[i].贷方金额6;
                xlWorkSheet.Cells[9 + i, 23] = LedgerData[i].贷方金额7;
                xlWorkSheet.Cells[9 + i, 24] = LedgerData[i].贷方金额8;
                xlWorkSheet.Cells[9 + i, 25] = LedgerData[i].贷方金额9;
                xlWorkSheet.Cells[9 + i, 26] = LedgerData[i].贷方金额10;
                xlWorkSheet.Cells[9 + i, 27] = LedgerData[i].贷方金额11;
                xlWorkSheet.Cells[9 + i, 28] = LedgerData[i].贷方金额12;
                xlWorkSheet.Cells[9 + i, 29] = LedgerData[i].借或贷;
                xlWorkSheet.Cells[9 + i, 30] = LedgerData[i].余额2;
                xlWorkSheet.Cells[9 + i, 31] = LedgerData[i].余额3;
                xlWorkSheet.Cells[9 + i, 32] = LedgerData[i].余额4;
                xlWorkSheet.Cells[9 + i, 33] = LedgerData[i].余额5;
                xlWorkSheet.Cells[9 + i, 34] = LedgerData[i].余额6;
                xlWorkSheet.Cells[9 + i, 35] = LedgerData[i].余额7;
                xlWorkSheet.Cells[9 + i, 36] = LedgerData[i].余额8;
                xlWorkSheet.Cells[9 + i, 37] = LedgerData[i].余额9;
                xlWorkSheet.Cells[9 + i, 38] = LedgerData[i].余额10;
                xlWorkSheet.Cells[9 + i, 39] = LedgerData[i].余额11;
                xlWorkSheet.Cells[9 + i, 40] = LedgerData[i].余额12;
            }


            xlApp.Visible = true;

            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
            return false;
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
