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
        private object misValue = System.Reflection.Missing.Value;

        #region 1.凭证
        /// <summary>
        /// 记账凭证
        /// </summary>
        /// <param name="guid"></param>
        public void ExportVouchers(Guid guid)
        {
            xls.Application xlApp = null;
            xls.Workbook xlWorkBook;
            xls.Worksheet xlWorkSheet;

            int SheetNum = new PA.ViewModel.ViewModel_凭证管理().GetPageNum(guid);
            Model_凭证单 Voucher = new PA.ViewModel.ViewModel_凭证管理().GetVoucher(guid);
            List<Model_凭证明细> VoucherDetails = new PA.ViewModel.ViewModel_凭证管理().GetVoucherDetails(guid);

            string SourceXls = Path + @"Data\打印\记账凭证模板.xls";
            string ExportXls = Path + @"Data\打印\记账凭证export.xls";
            File.Copy(SourceXls, ExportXls, true);
            try
            {
                xlApp = new xls.Application();
            }
            catch (Exception)
            {
                Console.WriteLine("找不到EXCEL");
            }
            xlWorkBook = xlApp.Workbooks.Open(ExportXls);
            xlWorkSheet = (xls.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            #region fill Voucher
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
            #endregion
            #region copy sheet while SheetNum>1 after fill Voucher Data
            for (int i = 1; i < SheetNum; i++)
            {
                xlWorkSheet.Copy(Type.Missing, xlWorkBook.Sheets[i]);
                xlWorkBook.Worksheets.get_Item(i + 1).Name = "Sheet" + (i + 1);
            }
            #endregion
            #region fill VoucherDetails
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
            #endregion
            xlApp.Visible = true;

            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
        }
        #endregion

        #region 2.账簿
        /// <summary>
        /// 总账
        /// </summary>
        /// <param name="Parm"></param>
        /// <returns></returns>
        public bool ExportLedger(string DetailsData)
        {
            xls.Application xlApp = null;
            xls.Workbook xlWorkBook;
            xls.Worksheet xlWorkSheet;

            List<Model_总账> LedgerData = new PA.ViewModel.ViewModel_账薄管理().GetTotalFee(DetailsData);
            if (LedgerData.Count <= 1)
            {
                return false;
            }
            const int PageLine = 47;
            int TotalPageNum = LedgerData.Count / PageLine + 1;
            string SourceXls = Path + @"Data\打印\总账模板.xls";
            string ExportXls = Path + @"Data\打印\总账export.xls";
            File.Copy(SourceXls, ExportXls, true);
            try
            {
                xlApp = new xls.Application();
            }
            catch (Exception)
            {
                Console.WriteLine("找不到EXCEL");
            }
            xlWorkBook = xlApp.Workbooks.Open(ExportXls);
            xlWorkSheet = (xls.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            //fill head data
            xlWorkSheet.Cells[1,6] = "总账";
            xlWorkSheet.Cells[3, 30] = DetailsData.Split('\t')[1];
            xlWorkSheet.Cells[6, 1] = LedgerData[1].年 + "年";
            //copy sheet
            for (int i = 1; i < TotalPageNum; i++)
            {
                xlWorkSheet.Copy(Type.Missing, xlWorkBook.Sheets[1]);
                xlWorkBook.Worksheets.get_Item(i + 1).Name = "Sheet" + (i + 1);
            }
            //fill detail data
            for (int PageNum = 0; PageNum < TotalPageNum; PageNum++)
            {
                xlWorkSheet = (xls.Worksheet)xlWorkBook.Worksheets.get_Item(PageNum+1);
                xlWorkSheet.Cells[3, 2] = (PageNum+1)+ "/" + TotalPageNum;
                for (int i = 0; i < PageLine; i++)
                {
                    if (i + (PageNum * PageLine) >= LedgerData.Count)
                    {
                        break;
                    }
                    #region 填充详细内容
                    xlWorkSheet.Cells[9 + i, 1 ] = LedgerData[i + (PageNum * PageLine)].月;
                    xlWorkSheet.Cells[9 + i, 2 ] = LedgerData[i + (PageNum * PageLine)].日;
                    xlWorkSheet.Cells[9 + i, 3 ] = LedgerData[i + (PageNum * PageLine)].号数;
                    xlWorkSheet.Cells[9 + i, 5 ] = LedgerData[i + (PageNum * PageLine)].摘要;
                    xlWorkSheet.Cells[9 + i, 7 ] = LedgerData[i + (PageNum * PageLine)].借方金额2;
                    xlWorkSheet.Cells[9 + i, 8 ] = LedgerData[i + (PageNum * PageLine)].借方金额3;
                    xlWorkSheet.Cells[9 + i, 9 ] = LedgerData[i + (PageNum * PageLine)].借方金额4;
                    xlWorkSheet.Cells[9 + i, 10] = LedgerData[i + (PageNum * PageLine)].借方金额5;
                    xlWorkSheet.Cells[9 + i, 11] = LedgerData[i + (PageNum * PageLine)].借方金额6;
                    xlWorkSheet.Cells[9 + i, 12] = LedgerData[i + (PageNum * PageLine)].借方金额7;
                    xlWorkSheet.Cells[9 + i, 13] = LedgerData[i + (PageNum * PageLine)].借方金额8;
                    xlWorkSheet.Cells[9 + i, 14] = LedgerData[i + (PageNum * PageLine)].借方金额9;
                    xlWorkSheet.Cells[9 + i, 15] = LedgerData[i + (PageNum * PageLine)].借方金额10;
                    xlWorkSheet.Cells[9 + i, 16] = LedgerData[i + (PageNum * PageLine)].借方金额11;
                    xlWorkSheet.Cells[9 + i, 17] = LedgerData[i + (PageNum * PageLine)].借方金额12;
                    xlWorkSheet.Cells[9 + i, 18] = LedgerData[i + (PageNum * PageLine)].贷方金额2;
                    xlWorkSheet.Cells[9 + i, 19] = LedgerData[i + (PageNum * PageLine)].贷方金额3;
                    xlWorkSheet.Cells[9 + i, 20] = LedgerData[i + (PageNum * PageLine)].贷方金额4;
                    xlWorkSheet.Cells[9 + i, 21] = LedgerData[i + (PageNum * PageLine)].贷方金额5;
                    xlWorkSheet.Cells[9 + i, 22] = LedgerData[i + (PageNum * PageLine)].贷方金额6;
                    xlWorkSheet.Cells[9 + i, 23] = LedgerData[i + (PageNum * PageLine)].贷方金额7;
                    xlWorkSheet.Cells[9 + i, 24] = LedgerData[i + (PageNum * PageLine)].贷方金额8;
                    xlWorkSheet.Cells[9 + i, 25] = LedgerData[i + (PageNum * PageLine)].贷方金额9;
                    xlWorkSheet.Cells[9 + i, 26] = LedgerData[i + (PageNum * PageLine)].贷方金额10;
                    xlWorkSheet.Cells[9 + i, 27] = LedgerData[i + (PageNum * PageLine)].贷方金额11;
                    xlWorkSheet.Cells[9 + i, 28] = LedgerData[i + (PageNum * PageLine)].贷方金额12;
                    xlWorkSheet.Cells[9 + i, 29] = LedgerData[i + (PageNum * PageLine)].借或贷;
                    xlWorkSheet.Cells[9 + i, 30] = LedgerData[i + (PageNum * PageLine)].余额2;
                    xlWorkSheet.Cells[9 + i, 31] = LedgerData[i + (PageNum * PageLine)].余额3;
                    xlWorkSheet.Cells[9 + i, 32] = LedgerData[i + (PageNum * PageLine)].余额4;
                    xlWorkSheet.Cells[9 + i, 33] = LedgerData[i + (PageNum * PageLine)].余额5;
                    xlWorkSheet.Cells[9 + i, 34] = LedgerData[i + (PageNum * PageLine)].余额6;
                    xlWorkSheet.Cells[9 + i, 35] = LedgerData[i + (PageNum * PageLine)].余额7;
                    xlWorkSheet.Cells[9 + i, 36] = LedgerData[i + (PageNum * PageLine)].余额8;
                    xlWorkSheet.Cells[9 + i, 37] = LedgerData[i + (PageNum * PageLine)].余额9;
                    xlWorkSheet.Cells[9 + i, 38] = LedgerData[i + (PageNum * PageLine)].余额10;
                    xlWorkSheet.Cells[9 + i, 39] = LedgerData[i + (PageNum * PageLine)].余额11;
                    xlWorkSheet.Cells[9 + i, 40] = LedgerData[i + (PageNum * PageLine)].余额12;
                    #endregion
                }
            }
            xlApp.Visible = true;

            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
            return true;
        }
        /// <summary>
        /// 经费支出明细账
        /// </summary>
        public bool ExportExpenditureDetails(string Parm)
        {
            xls.Application xlApp = null;
            xls.Workbook xlWorkBook;
            xls.Worksheet xlWorkSheet;

            List<Model_费用明细> data = new PA.ViewModel.ViewModel_账薄管理().GetFeeDetail(Parm);
            string year;
            try
            {
                year = data[0].年;
            }
            catch (Exception)
            {
                return false;
            }
            const int PageLine = 23;
            int TotalPageNum = data.Count / PageLine + 1;
            string SourceXls = Path + @"Data\打印\管理费用模板.xls";
            string ExportXls = Path + @"Data\打印\管理费用export.xls";
            File.Copy(SourceXls, ExportXls, true);
            try
            {
                xlApp = new xls.Application();
            }
            catch (Exception)
            {
                Console.WriteLine("找不到EXCEL");
            }
            xlWorkBook = xlApp.Workbooks.Open(ExportXls);
            xlWorkSheet = (xls.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            //fill head data
            xlWorkSheet.Cells[1, 27] = "管    理    费    用";
            xlWorkSheet.Cells[2, 1 ] = "项（或目）科目名称：" + Parm.Split('\t')[1];
            xlWorkSheet.Cells[6, 1] = year + "年";
            try
            {
                xlWorkSheet.Cells[7, "AI"] = data[0].列名[0].Split('\t')[1];
                xlWorkSheet.Cells[7, "AS"] = data[0].列名[1].Split('\t')[1];
                xlWorkSheet.Cells[7, "BD"] = data[0].列名[2].Split('\t')[1];
                xlWorkSheet.Cells[7, "BN"] = data[0].列名[3].Split('\t')[1];
                xlWorkSheet.Cells[7, "BX"] = data[0].列名[4].Split('\t')[1];
                xlWorkSheet.Cells[7, "CH"] = data[0].列名[5].Split('\t')[1];
                xlWorkSheet.Cells[7, "CR"] = data[0].列名[6].Split('\t')[1];
                xlWorkSheet.Cells[7, "DB"] = data[0].列名[7].Split('\t')[1];
                xlWorkSheet.Cells[7, "DL"] = data[0].列名[8].Split('\t')[1];
                xlWorkSheet.Cells[7, "DV"] = data[0].列名[9].Split('\t')[1];
                xlWorkSheet.Cells[7, "EF"] = data[0].列名[10].Split('\t')[1];
            }
            catch (ArgumentOutOfRangeException) { }
            //copy sheet
            for (int i = 1; i < TotalPageNum; i++)
            {
                xlWorkSheet.Copy(Type.Missing, xlWorkBook.Sheets[1]);
                xlWorkBook.Worksheets.get_Item(i + 1).Name = "Sheet" + (i + 1);
            }
            //fill detail data
            for (int PageNum = 0; PageNum < TotalPageNum; PageNum++)
            {
                xlWorkSheet = (xls.Worksheet)xlWorkBook.Worksheets.get_Item(PageNum + 1);
                xlWorkSheet.Cells[3, "EE"] = (PageNum + 1) + "/" + TotalPageNum;
                for (int i = 0; i < PageLine; i++)
                {
                    if (i + (PageNum * PageLine) >= data.Count)
                    {
                        break;
                    }
                    #region 填充详细内容
                    xlWorkSheet.Cells[9 + i, 1 ] = data[i + (PageNum * PageLine)].月;
                    xlWorkSheet.Cells[9 + i, 2 ] = data[i + (PageNum * PageLine)].日;
                    xlWorkSheet.Cells[9 + i, 3 ] = data[i + (PageNum * PageLine)].号数;
                    xlWorkSheet.Cells[9 + i, 4 ] = data[i + (PageNum * PageLine)].摘要;
                    xlWorkSheet.Cells[9 + i, 5 ] = data[i + (PageNum * PageLine)].借方金额1;
                    xlWorkSheet.Cells[9 + i, 6 ] = data[i + (PageNum * PageLine)].借方金额2;
                    xlWorkSheet.Cells[9 + i, 7 ] = data[i + (PageNum * PageLine)].借方金额3;
                    xlWorkSheet.Cells[9 + i, 8 ] = data[i + (PageNum * PageLine)].借方金额4;
                    xlWorkSheet.Cells[9 + i, 9 ] = data[i + (PageNum * PageLine)].借方金额5;
                    xlWorkSheet.Cells[9 + i, 10] = data[i + (PageNum * PageLine)].借方金额6;
                    xlWorkSheet.Cells[9 + i, 11] = data[i + (PageNum * PageLine)].借方金额7;
                    xlWorkSheet.Cells[9 + i, 12] = data[i + (PageNum * PageLine)].借方金额8;
                    xlWorkSheet.Cells[9 + i, 13] = data[i + (PageNum * PageLine)].借方金额9;
                    xlWorkSheet.Cells[9 + i, 14] = data[i + (PageNum * PageLine)].借方金额10;
                    xlWorkSheet.Cells[9 + i, 15] = data[i + (PageNum * PageLine)].贷方金额1;
                    xlWorkSheet.Cells[9 + i, 16] = data[i + (PageNum * PageLine)].贷方金额2;
                    xlWorkSheet.Cells[9 + i, 17] = data[i + (PageNum * PageLine)].贷方金额3;
                    xlWorkSheet.Cells[9 + i, 18] = data[i + (PageNum * PageLine)].贷方金额4;
                    xlWorkSheet.Cells[9 + i, 19] = data[i + (PageNum * PageLine)].贷方金额5;
                    xlWorkSheet.Cells[9 + i, 20] = data[i + (PageNum * PageLine)].贷方金额6;
                    xlWorkSheet.Cells[9 + i, 21] = data[i + (PageNum * PageLine)].贷方金额7;
                    xlWorkSheet.Cells[9 + i, 22] = data[i + (PageNum * PageLine)].贷方金额8;
                    xlWorkSheet.Cells[9 + i, 23] = data[i + (PageNum * PageLine)].贷方金额9;
                    xlWorkSheet.Cells[9 + i, 24] = data[i + (PageNum * PageLine)].贷方金额10;
                    xlWorkSheet.Cells[9 + i, 25] = data[i + (PageNum * PageLine)].余额1;
                    xlWorkSheet.Cells[9 + i, 26] = data[i + (PageNum * PageLine)].余额2;
                    xlWorkSheet.Cells[9 + i, 27] = data[i + (PageNum * PageLine)].余额3;
                    xlWorkSheet.Cells[9 + i, 28] = data[i + (PageNum * PageLine)].余额4;
                    xlWorkSheet.Cells[9 + i, 29] = data[i + (PageNum * PageLine)].余额5;
                    xlWorkSheet.Cells[9 + i, 30] = data[i + (PageNum * PageLine)].余额6;
                    xlWorkSheet.Cells[9 + i, 31] = data[i + (PageNum * PageLine)].余额7;
                    xlWorkSheet.Cells[9 + i, 32] = data[i + (PageNum * PageLine)].余额8;
                    xlWorkSheet.Cells[9 + i, 33] = data[i + (PageNum * PageLine)].余额9;
                    xlWorkSheet.Cells[9 + i, 34] = data[i + (PageNum * PageLine)].余额10;
                    xlWorkSheet.Cells[9 + i, 35] = data[i + (PageNum * PageLine)].金额31;
                    xlWorkSheet.Cells[9 + i, 36] = data[i + (PageNum * PageLine)].金额32;
                    xlWorkSheet.Cells[9 + i, 37] = data[i + (PageNum * PageLine)].金额33;
                    xlWorkSheet.Cells[9 + i, 38] = data[i + (PageNum * PageLine)].金额34;
                    xlWorkSheet.Cells[9 + i, 39] = data[i + (PageNum * PageLine)].金额35;
                    xlWorkSheet.Cells[9 + i, 40] = data[i + (PageNum * PageLine)].金额36;
                    xlWorkSheet.Cells[9 + i, 41] = data[i + (PageNum * PageLine)].金额37;
                    xlWorkSheet.Cells[9 + i, 42] = data[i + (PageNum * PageLine)].金额38;
                    xlWorkSheet.Cells[9 + i, 43] = data[i + (PageNum * PageLine)].金额39;
                    xlWorkSheet.Cells[9 + i, 44] = data[i + (PageNum * PageLine)].金额40;
                    xlWorkSheet.Cells[9 + i, 45] = data[i + (PageNum * PageLine)].金额41;
                    xlWorkSheet.Cells[9 + i, 46] = data[i + (PageNum * PageLine)].金额42;
                    xlWorkSheet.Cells[9 + i, 47] = data[i + (PageNum * PageLine)].金额43;
                    xlWorkSheet.Cells[9 + i, 48] = data[i + (PageNum * PageLine)].金额44;
                    xlWorkSheet.Cells[9 + i, 49] = data[i + (PageNum * PageLine)].金额45;
                    xlWorkSheet.Cells[9 + i, 50] = data[i + (PageNum * PageLine)].金额46;
                    xlWorkSheet.Cells[9 + i, 51] = data[i + (PageNum * PageLine)].金额47;
                    xlWorkSheet.Cells[9 + i, 52] = data[i + (PageNum * PageLine)].金额48;
                    xlWorkSheet.Cells[9 + i, 53] = data[i + (PageNum * PageLine)].金额49;
                    xlWorkSheet.Cells[9 + i, 54] = data[i + (PageNum * PageLine)].金额50;
                    //第二页
                    xlWorkSheet.Cells[9 + i, 56] = data[i + (PageNum * PageLine)].金额51;
                    xlWorkSheet.Cells[9 + i, 57] = data[i + (PageNum * PageLine)].金额52;
                    xlWorkSheet.Cells[9 + i, 58] = data[i + (PageNum * PageLine)].金额53;
                    xlWorkSheet.Cells[9 + i, 59] = data[i + (PageNum * PageLine)].金额54;
                    xlWorkSheet.Cells[9 + i, 60] = data[i + (PageNum * PageLine)].金额55;
                    xlWorkSheet.Cells[9 + i, 61] = data[i + (PageNum * PageLine)].金额56;
                    xlWorkSheet.Cells[9 + i, 62] = data[i + (PageNum * PageLine)].金额57;
                    xlWorkSheet.Cells[9 + i, 63] = data[i + (PageNum * PageLine)].金额58;
                    xlWorkSheet.Cells[9 + i, 64] = data[i + (PageNum * PageLine)].金额59;
                    xlWorkSheet.Cells[9 + i, 65] = data[i + (PageNum * PageLine)].金额60;
                    xlWorkSheet.Cells[9 + i, 66] = data[i + (PageNum * PageLine)].金额61;
                    xlWorkSheet.Cells[9 + i, 67] = data[i + (PageNum * PageLine)].金额62;
                    xlWorkSheet.Cells[9 + i, 68] = data[i + (PageNum * PageLine)].金额63;
                    xlWorkSheet.Cells[9 + i, 69] = data[i + (PageNum * PageLine)].金额64;
                    xlWorkSheet.Cells[9 + i, 70] = data[i + (PageNum * PageLine)].金额65;
                    xlWorkSheet.Cells[9 + i, 71] = data[i + (PageNum * PageLine)].金额66;
                    xlWorkSheet.Cells[9 + i, 72] = data[i + (PageNum * PageLine)].金额67;
                    xlWorkSheet.Cells[9 + i, 73] = data[i + (PageNum * PageLine)].金额68;
                    xlWorkSheet.Cells[9 + i, 74] = data[i + (PageNum * PageLine)].金额69;
                    xlWorkSheet.Cells[9 + i, 75] = data[i + (PageNum * PageLine)].金额70;
                    xlWorkSheet.Cells[9 + i, 76] = data[i + (PageNum * PageLine)].金额71;
                    xlWorkSheet.Cells[9 + i, 77] = data[i + (PageNum * PageLine)].金额72;
                    xlWorkSheet.Cells[9 + i, 78] = data[i + (PageNum * PageLine)].金额73;
                    xlWorkSheet.Cells[9 + i, 79] = data[i + (PageNum * PageLine)].金额74;
                    xlWorkSheet.Cells[9 + i, 80] = data[i + (PageNum * PageLine)].金额75;
                    xlWorkSheet.Cells[9 + i, 81] = data[i + (PageNum * PageLine)].金额76;
                    xlWorkSheet.Cells[9 + i, 82] = data[i + (PageNum * PageLine)].金额77;
                    xlWorkSheet.Cells[9 + i, 83] = data[i + (PageNum * PageLine)].金额78;
                    xlWorkSheet.Cells[9 + i, 84] = data[i + (PageNum * PageLine)].金额79;
                    xlWorkSheet.Cells[9 + i, 85] = data[i + (PageNum * PageLine)].金额80;
                    xlWorkSheet.Cells[9 + i, 86] = data[i + (PageNum * PageLine)].金额81;
                    xlWorkSheet.Cells[9 + i, 87] = data[i + (PageNum * PageLine)].金额82;
                    xlWorkSheet.Cells[9 + i, 88] = data[i + (PageNum * PageLine)].金额83;
                    xlWorkSheet.Cells[9 + i, 89] = data[i + (PageNum * PageLine)].金额84;
                    xlWorkSheet.Cells[9 + i, 90] = data[i + (PageNum * PageLine)].金额85;
                    xlWorkSheet.Cells[9 + i, 91] = data[i + (PageNum * PageLine)].金额86;
                    xlWorkSheet.Cells[9 + i, 92] = data[i + (PageNum * PageLine)].金额87;
                    xlWorkSheet.Cells[9 + i, 93] = data[i + (PageNum * PageLine)].金额88;
                    xlWorkSheet.Cells[9 + i, 94] = data[i + (PageNum * PageLine)].金额89;
                    xlWorkSheet.Cells[9 + i, 95] = data[i + (PageNum * PageLine)].金额90;
                    xlWorkSheet.Cells[9 + i, 96] = data[i + (PageNum * PageLine)].金额91;
                    xlWorkSheet.Cells[9 + i, 97] = data[i + (PageNum * PageLine)].金额92;
                    xlWorkSheet.Cells[9 + i, 98] = data[i + (PageNum * PageLine)].金额93;
                    xlWorkSheet.Cells[9 + i, 99] = data[i + (PageNum * PageLine)].金额94;
                    xlWorkSheet.Cells[9 + i, 100] = data[i + (PageNum * PageLine)].金额95;
                    xlWorkSheet.Cells[9 + i, 101] = data[i + (PageNum * PageLine)].金额96;
                    xlWorkSheet.Cells[9 + i, 102] = data[i + (PageNum * PageLine)].金额97;
                    xlWorkSheet.Cells[9 + i, 103] = data[i + (PageNum * PageLine)].金额98;
                    xlWorkSheet.Cells[9 + i, 104] = data[i + (PageNum * PageLine)].金额99;
                    xlWorkSheet.Cells[9 + i, 105] = data[i + (PageNum * PageLine)].金额100;
                    xlWorkSheet.Cells[9 + i, 106] = data[i + (PageNum * PageLine)].金额101;
                    xlWorkSheet.Cells[9 + i, 107] = data[i + (PageNum * PageLine)].金额102;
                    xlWorkSheet.Cells[9 + i, 108] = data[i + (PageNum * PageLine)].金额103;
                    xlWorkSheet.Cells[9 + i, 109] = data[i + (PageNum * PageLine)].金额104;
                    xlWorkSheet.Cells[9 + i, 110] = data[i + (PageNum * PageLine)].金额105;
                    xlWorkSheet.Cells[9 + i, 111] = data[i + (PageNum * PageLine)].金额106;
                    xlWorkSheet.Cells[9 + i, 112] = data[i + (PageNum * PageLine)].金额107;
                    xlWorkSheet.Cells[9 + i, 113] = data[i + (PageNum * PageLine)].金额108;
                    xlWorkSheet.Cells[9 + i, 114] = data[i + (PageNum * PageLine)].金额109;
                    xlWorkSheet.Cells[9 + i, 115] = data[i + (PageNum * PageLine)].金额110;
                    xlWorkSheet.Cells[9 + i, 116] = data[i + (PageNum * PageLine)].金额111;
                    xlWorkSheet.Cells[9 + i, 117] = data[i + (PageNum * PageLine)].金额112;
                    xlWorkSheet.Cells[9 + i, 118] = data[i + (PageNum * PageLine)].金额113;
                    xlWorkSheet.Cells[9 + i, 119] = data[i + (PageNum * PageLine)].金额114;
                    xlWorkSheet.Cells[9 + i, 120] = data[i + (PageNum * PageLine)].金额115;
                    xlWorkSheet.Cells[9 + i, 121] = data[i + (PageNum * PageLine)].金额116;
                    xlWorkSheet.Cells[9 + i, 122] = data[i + (PageNum * PageLine)].金额117;
                    xlWorkSheet.Cells[9 + i, 123] = data[i + (PageNum * PageLine)].金额118;
                    xlWorkSheet.Cells[9 + i, 124] = data[i + (PageNum * PageLine)].金额119;
                    xlWorkSheet.Cells[9 + i, 125] = data[i + (PageNum * PageLine)].金额120;
                    xlWorkSheet.Cells[9 + i, 126] = data[i + (PageNum * PageLine)].金额121;
                    xlWorkSheet.Cells[9 + i, 127] = data[i + (PageNum * PageLine)].金额122;
                    xlWorkSheet.Cells[9 + i, 128] = data[i + (PageNum * PageLine)].金额123;
                    xlWorkSheet.Cells[9 + i, 129] = data[i + (PageNum * PageLine)].金额124;
                    xlWorkSheet.Cells[9 + i, 130] = data[i + (PageNum * PageLine)].金额125;
                    xlWorkSheet.Cells[9 + i, 131] = data[i + (PageNum * PageLine)].金额126;
                    xlWorkSheet.Cells[9 + i, 132] = data[i + (PageNum * PageLine)].金额127;
                    xlWorkSheet.Cells[9 + i, 133] = data[i + (PageNum * PageLine)].金额128;
                    xlWorkSheet.Cells[9 + i, 134] = data[i + (PageNum * PageLine)].金额129;
                    xlWorkSheet.Cells[9 + i, 135] = data[i + (PageNum * PageLine)].金额130;
                    xlWorkSheet.Cells[9 + i, 136] = data[i + (PageNum * PageLine)].金额131;
                    xlWorkSheet.Cells[9 + i, 137] = data[i + (PageNum * PageLine)].金额132;
                    xlWorkSheet.Cells[9 + i, 138] = data[i + (PageNum * PageLine)].金额133;
                    xlWorkSheet.Cells[9 + i, 139] = data[i + (PageNum * PageLine)].金额134;
                    xlWorkSheet.Cells[9 + i, 140] = data[i + (PageNum * PageLine)].金额135;
                    xlWorkSheet.Cells[9 + i, 141] = data[i + (PageNum * PageLine)].金额136;
                    xlWorkSheet.Cells[9 + i, 142] = data[i + (PageNum * PageLine)].金额137;
                    xlWorkSheet.Cells[9 + i, 143] = data[i + (PageNum * PageLine)].金额138;
                    xlWorkSheet.Cells[9 + i, 144] = data[i + (PageNum * PageLine)].金额139;
                    xlWorkSheet.Cells[9 + i, 145] = data[i + (PageNum * PageLine)].金额140;
                    #endregion
                }
            }

            xlApp.Visible = true;
            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
            return true;
        }
        /// <summary>
        /// 科目明细账
        /// </summary>
        public bool ExportSubjectDetails(string Parm1, string Parm2)
        {
            xls.Application xlApp = null;
            xls.Workbook xlWorkBook;
            xls.Worksheet xlWorkSheet;

            string SourceXls = Path + @"Data\打印\三栏明细账模板.xls";
            string ExportXls = Path + @"Data\打印\三栏明细账export.xls";
            File.Copy(SourceXls, ExportXls, true);
            try
            {
                xlApp = new xls.Application();
            }
            catch (Exception)
            {
                Console.WriteLine("找不到EXCEL");
            }
            xlWorkBook = xlApp.Workbooks.Open(ExportXls);
            xlWorkSheet = (xls.Worksheet)xlWorkBook.Worksheets.get_Item(1);


            xlApp.Visible = true;
            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
            return true;
        }
        #endregion

        #region 3.报表

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
