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
        public string ExportVouchers(Guid guid)
        {
            string result = "";
            xls.Application xlApp = null;
            xls.Workbook xlWorkBook;
            xls.Worksheet xlWorkSheet;

            int SheetNum = new PA.ViewModel.ViewModel_凭证管理().GetPageNum(guid);
            Model_凭证单 Voucher = new PA.ViewModel.ViewModel_凭证管理().GetVoucher(guid);
            List<Model_凭证明细> VoucherDetails = new PA.ViewModel.ViewModel_凭证管理().GetVoucherDetails(guid);

            string SourceXls = Path + @"Data\打印\记账凭证模板.xls";
            string ExportXls = Path + @"Data\打印\记账凭证export.xls";
            try
            {
                File.Copy(SourceXls, ExportXls, true);
            }
            catch (FileNotFoundException)
            {
                return "FileNotFound";
            }
            catch (IOException)
            {
                return "FileLocking";
            }
            try
            {
                xlApp = new xls.Application();
            }
            catch (Exception)
            {
                return "找不到EXCEL";
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
            return result;
        }
    #endregion

    #region 2.账簿
        /// <summary>
        /// 总账
        /// </summary>
        /// <param name="Parm"></param>
        /// <returns></returns>
        public string ExportLedger(string DetailsData)
        {
            string result = "";
            xls.Application xlApp = null;
            xls.Workbook xlWorkBook;
            xls.Worksheet xlWorkSheet;

            List<Model_总账> LedgerData = new PA.ViewModel.ViewModel_账薄管理().GetTotalFee(DetailsData);
            if (LedgerData.Count <= 1)
            {
                return "NoData";
            }
            const int PageLine = 47;
            int TotalPageNum = LedgerData.Count / PageLine + 1;
            string SourceXls = Path + @"Data\打印\总账模板.xls";
            string ExportXls = Path + @"Data\打印\总账export.xls";
            try
            {
                File.Copy(SourceXls, ExportXls, true);
            }
            catch (FileNotFoundException)
            {
                return "FileNotFound";
            }
            catch (IOException)
            {
                return "FileLocking";
            }
            try
            {
                xlApp = new xls.Application();
            }
            catch (Exception)
            {
                return "找不到EXCEL";
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
            return result;
        }
        /// <summary>
        /// 经费支出明细账
        /// </summary>
        public string ExportExpenditureDetails(string Parm)
        {
            string result = "";
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
                return "NoData";
            }
            const int PageLine = 23;
            int TotalPageNum = data.Count / PageLine + 1;
            string SourceXls = Path + @"Data\打印\管理费用模板.xls";
            string ExportXls = Path + @"Data\打印\管理费用export.xls";
            try
            {
                File.Copy(SourceXls, ExportXls, true);
            }
            catch (FileNotFoundException)
            {
                return "FileNotFound";
            }
            catch (IOException)
            {
                return "FileLocking";
            }
            try
            {
                xlApp = new xls.Application();
            }
            catch (Exception)
            {
                return "找不到EXCEL";
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
            return result;
        }
        /// <summary>
        /// 科目明细账
        /// </summary>
        public string ExportSubjectDetails(string ParmSubjectId, string ParmDetailId, int ParmPeroid)
        {
            string result = "";
            List<Model_科目明细账> data = new PA.ViewModel.ViewModel_账薄管理().GetSubjectDetail(ParmSubjectId, ParmDetailId, ParmPeroid);
            string year = "    ";
            try
            {
                year = data[0].年;
            }
            catch (Exception)
            {
                return "NoData";
            }
            const int PageLine = 21;
            int TotalPageNum = data.Count / PageLine + 1;

            xls.Application xlApp = null;
            xls.Workbook xlWorkBook;
            xls.Worksheet xlWorkSheet;
            string SourceXls = Path + @"Data\打印\三栏明细账模板.xls";
            string ExportXls = Path + @"Data\打印\三栏明细账export.xls";
            try
            {
                File.Copy(SourceXls, ExportXls, true);
            }
            catch (FileNotFoundException)
            {
                return "FileNotFound";
            }
            catch (IOException)
            {
                return "FileLocking";
            }
            try
            {
                xlApp = new xls.Application();
            }
            catch (Exception)
            {
                return "找不到EXCEL";
            }
            xlWorkBook = xlApp.Workbooks.Open(ExportXls);
            xlWorkSheet = (xls.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            #region fill head data
            xlWorkSheet.Cells[2, "AI"] = ParmSubjectId;
            xlWorkSheet.Cells[3, "AI"] = ParmDetailId;
            xlWorkSheet.Cells[5, "B"] = year + "年";
            #endregion
            #region copy sheet
            for (int i = 1; i < TotalPageNum; i++)
            {
                xlWorkSheet.Copy(Type.Missing, xlWorkBook.Sheets[1]);
                xlWorkBook.Worksheets.get_Item(i + 1).Name = "Sheet" + (i + 1);
            }
            #endregion
            #region fill detail data
            for (int PageNum = 0; PageNum < TotalPageNum; PageNum++)
            {
                xlWorkSheet = (xls.Worksheet)xlWorkBook.Worksheets.get_Item(PageNum + 1);
                //xlWorkSheet.Cells[3, "EE"] = (PageNum + 1) + "/" + TotalPageNum;
                for (int i = 0; i < PageLine; i++)
                {
                    if (i + (PageNum * PageLine) >= data.Count)
                    {
                        break;
                    }
                    #region fill data
                    xlWorkSheet.Cells[8 + i, 2 ] = data[i + (PageNum * PageLine)].月;
                    xlWorkSheet.Cells[8 + i, 3 ] = data[i + (PageNum * PageLine)].日;
                    xlWorkSheet.Cells[8 + i, 4 ] = data[i + (PageNum * PageLine)].号数;
                    xlWorkSheet.Cells[8 + i, 5 ] = data[i + (PageNum * PageLine)].摘要;
                    xlWorkSheet.Cells[8 + i, 7 ] = data[i + (PageNum * PageLine)].日页;
                    xlWorkSheet.Cells[8 + i, 8 ] = data[i + (PageNum * PageLine)].借方金额1;
                    xlWorkSheet.Cells[8 + i, 9 ] = data[i + (PageNum * PageLine)].借方金额2;
                    xlWorkSheet.Cells[8 + i, 10] = data[i + (PageNum * PageLine)].借方金额3;
                    xlWorkSheet.Cells[8 + i, 11] = data[i + (PageNum * PageLine)].借方金额4;
                    xlWorkSheet.Cells[8 + i, 12] = data[i + (PageNum * PageLine)].借方金额5;
                    xlWorkSheet.Cells[8 + i, 13] = data[i + (PageNum * PageLine)].借方金额6;
                    xlWorkSheet.Cells[8 + i, 14] = data[i + (PageNum * PageLine)].借方金额7;
                    xlWorkSheet.Cells[8 + i, 15] = data[i + (PageNum * PageLine)].借方金额8;
                    xlWorkSheet.Cells[8 + i, 16] = data[i + (PageNum * PageLine)].借方金额9;
                    xlWorkSheet.Cells[8 + i, 17] = data[i + (PageNum * PageLine)].借方金额10;
                    xlWorkSheet.Cells[8 + i, 18] = data[i + (PageNum * PageLine)].借方金额11;
                    xlWorkSheet.Cells[8 + i, 19] = data[i + (PageNum * PageLine)].借方金额12;
                    xlWorkSheet.Cells[8 + i, 20] = "";
                    xlWorkSheet.Cells[8 + i, 21] = data[i + (PageNum * PageLine)].贷方金额1;
                    xlWorkSheet.Cells[8 + i, 22] = data[i + (PageNum * PageLine)].贷方金额2;
                    xlWorkSheet.Cells[8 + i, 23] = data[i + (PageNum * PageLine)].贷方金额3;
                    xlWorkSheet.Cells[8 + i, 24] = data[i + (PageNum * PageLine)].贷方金额4;
                    xlWorkSheet.Cells[8 + i, 25] = data[i + (PageNum * PageLine)].贷方金额5;
                    xlWorkSheet.Cells[8 + i, 26] = data[i + (PageNum * PageLine)].贷方金额6;
                    xlWorkSheet.Cells[8 + i, 27] = data[i + (PageNum * PageLine)].贷方金额7;
                    xlWorkSheet.Cells[8 + i, 28] = data[i + (PageNum * PageLine)].贷方金额8;
                    xlWorkSheet.Cells[8 + i, 29] = data[i + (PageNum * PageLine)].贷方金额9;
                    xlWorkSheet.Cells[8 + i, 30] = data[i + (PageNum * PageLine)].贷方金额10;
                    xlWorkSheet.Cells[8 + i, 31] = data[i + (PageNum * PageLine)].贷方金额11;
                    xlWorkSheet.Cells[8 + i, 32] = data[i + (PageNum * PageLine)].贷方金额12;
                    xlWorkSheet.Cells[8 + i, 33] = "";
                    xlWorkSheet.Cells[8 + i, 34] = data[i + (PageNum * PageLine)].借或贷;
                    xlWorkSheet.Cells[8 + i, 35] = data[i + (PageNum * PageLine)].余额1;
                    xlWorkSheet.Cells[8 + i, 36] = data[i + (PageNum * PageLine)].余额2;
                    xlWorkSheet.Cells[8 + i, 37] = data[i + (PageNum * PageLine)].余额3;
                    xlWorkSheet.Cells[8 + i, 38] = data[i + (PageNum * PageLine)].余额4;
                    xlWorkSheet.Cells[8 + i, 39] = data[i + (PageNum * PageLine)].余额5;
                    xlWorkSheet.Cells[8 + i, 40] = data[i + (PageNum * PageLine)].余额6;
                    xlWorkSheet.Cells[8 + i, 41] = data[i + (PageNum * PageLine)].余额7;
                    xlWorkSheet.Cells[8 + i, 42] = data[i + (PageNum * PageLine)].余额8;
                    xlWorkSheet.Cells[8 + i, 43] = data[i + (PageNum * PageLine)].余额9;
                    xlWorkSheet.Cells[8 + i, 44] = data[i + (PageNum * PageLine)].余额10;
                    xlWorkSheet.Cells[8 + i, 45] = data[i + (PageNum * PageLine)].余额11;
                    xlWorkSheet.Cells[8 + i, 46] = data[i + (PageNum * PageLine)].余额12;
                    xlWorkSheet.Cells[8 + i, 47] = "";
                    #endregion
                }
            }
            #endregion

            xlApp.Visible = true;
            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
            return result;
        }
    #endregion

    #region 3.报表
        /// <summary>
        /// 资产负债表
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
            string SourceXls = Path + @"Data\打印\资产负债表模板.xls";
            string ExportXls = Path + @"Data\打印\资产负债表export.xls";
            try
            {
                File.Copy(SourceXls, ExportXls, true);
            }
            catch (FileNotFoundException)
            {
                return "FileNotFound";
            }
            catch (IOException)
            {
                return "FileLocking";
            }
            try
            {
                xlApp = new xls.Application();
            }
            catch (Exception)
            {
                return "找不到EXCEL";
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
            if(data.Count < 1)
            {
                return "NoData";
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
        /// 收入支出总表
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
            string SourceXls = Path + @"Data\打印\收入支出总表模板.xls";
            string ExportXls = Path + @"Data\打印\收入支出总表export.xls";
            try
            {
                File.Copy(SourceXls, ExportXls, true);
            }
            catch (FileNotFoundException)
            {
                return "FileNotFound";
            }
            catch (IOException)
            {
                return "FileLocking";
            }
            try
            {
                xlApp = new xls.Application();
            }
            catch (Exception)
            {
                return "找不到EXCEL";
            }
            xlWorkBook = xlApp.Workbooks.Open(ExportXls);
            xlWorkSheet = (xls.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            #endregion

            #region fill data
            List<Model_报表类> data = new PA.ViewModel.ViewModel_ReportManager().GetIncomeAndExpenses(ParmPeroid);
            if (data.Count <= 0)
            {
                return "NoData";
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

            xlWorkSheet.Cells[7, "H"] = (insumy1 - insumy2);
            xlWorkSheet.Cells[8, "H"] = b3;

            xlWorkSheet.Cells[6, "H"] = ((insumy1 - insumy2) + b3);

            xlWorkSheet.Cells[16, "H"] = ((insumy1 - insumy2) + b3);


            #endregion

            xlApp.Visible = true;
            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
            return result;
        }
        /// <summary>
        /// 行政费用支出明细表
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
            string SourceXls = Path + @"Data\打印\行政费用支出明细表模板2.xls";
            string ExportXls = Path + @"Data\打印\行政费用支出明细表2export.xls";
            try
            {
                File.Copy(SourceXls, ExportXls, true);
            }
            catch (FileNotFoundException)
            {
                return "FileNotFound";
            }
            catch (IOException)
            {
                return "FileLocking";
            }
            try
            {
                xlApp = new xls.Application();
            }
            catch (Exception)
            {
                return "找不到EXCEL";
            }
            xlWorkBook = xlApp.Workbooks.Open(ExportXls);
            xlWorkSheet = (xls.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            #endregion

            #region fill data
            List<Model_报表类> data = new PA.ViewModel.ViewModel_ReportManager().GetAdministrativeExpenseDetail(ParmPeroid);
            Console.WriteLine(data.Count);
            if(data.Count <= 0)
            {
                return "NoData";
            }
            decimal dy = 0;
            decimal dn = 0;
            //二级科目
            decimal b101 = 0;
            decimal b102 = 0;
            decimal b201 = 0;
            decimal b202 = 0;
            decimal b301 = 0;
            decimal b302 = 0;
            decimal b401 = 0;
            decimal b402 = 0;

            for (int i = 0; i < data.Count; i++)
            {
                if (i < 7)
                {
                    xlWorkSheet.Cells[9 + i, "B"] = data[i].本期数;
                    xlWorkSheet.Cells[9 + i, "C"] = data[i].累计数;
                    decimal.TryParse(data[i].本期数, out dn);
                    decimal.TryParse(data[i].累计数, out dy);
                    b101 += dn;
                    b102 += dy;
                }
                else if (i >= 7 && i < 27)
                {
                    xlWorkSheet.Cells[10 + i, "B"] = data[i].本期数;
                    xlWorkSheet.Cells[10 + i, "C"] = data[i].累计数;
                    decimal.TryParse(data[i].本期数, out dn);
                    decimal.TryParse(data[i].累计数, out dy);
                    b201 += dn;
                    b202 += dy;
                }
                else if (i == 27)
                {
                    xlWorkSheet.Cells[7, "I"] = data[i].本期数;
                    xlWorkSheet.Cells[7, "J"] = data[i].累计数;
                    decimal.TryParse(data[i].本期数, out dn);
                    decimal.TryParse(data[i].累计数, out dy);
                    b201 += dn;
                    b202 += dy;
                }
                else if (i >= 28 && i < 42)
                {
                    xlWorkSheet.Cells[i-28+9, "I"] = data[i].本期数;
                    xlWorkSheet.Cells[i-28+9, "J"] = data[i].累计数;
                    decimal.TryParse(data[i].本期数, out dn);
                    decimal.TryParse(data[i].累计数, out dy);
                    b301 += dn;
                    b302 += dy;
                }
                else
                {
                    xlWorkSheet.Cells[i-42+24, "B"] = data[i].本期数;
                    xlWorkSheet.Cells[i-42+24, "C"] = data[i].累计数;
                    decimal.TryParse(data[i].本期数, out dn);
                    decimal.TryParse(data[i].累计数, out dy);
                    b401 += dn;
                    b402 += dy;
                }

                //2级科目设置
                xlWorkSheet.Cells[8, "B"] = b101;
                xlWorkSheet.Cells[8, "C"] = b102;
                xlWorkSheet.Cells[16, "B"] = b201;
                xlWorkSheet.Cells[16, "C"] = b202;
                xlWorkSheet.Cells[8, "I"] = b301;
                xlWorkSheet.Cells[8, "J"] = b302;
                xlWorkSheet.Cells[23, "I"] = b401;
                xlWorkSheet.Cells[23, "J"] = b402;

                xlWorkSheet.Cells[7, "B"] = (b101 + b201 + b301 + b401);
                xlWorkSheet.Cells[7, "C"] = (b102 + b202 + b302 + b402);
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
