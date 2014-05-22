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
using PA.Helper.DataBase;
using PA.Helper.DataDefind;

namespace PA.Helper.ExcelHelper
{
    /// <summary>
    /// By:Lugia
    /// </summary>
    public class ExcelWriter
    {
        private string Path = AppDomain.CurrentDomain.BaseDirectory;
        private object misValue = System.Reflection.Missing.Value;
        private string DateNow = "";

        public ExcelWriter()
        {
            DateNow = DateTime.Now.ToString("_yyyyMMddHHmmss");
        }

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
            string ExportXls = Path + @"Excel\打印\记账凭证" + DateNow + ".xls";
            try
            {
                File.Copy(SourceXls, ExportXls, true);
            }
            catch (FileNotFoundException fe)
            {
                Log.Write(fe.Message);
                return "模板文件未找到";
            }
            catch (IOException ioe)
            {
                Log.Write(ioe.Message);
                return "文件锁定，请关闭Excel再试";
            }
            try
            {
                xlApp = new xls.Application();
            }
            catch (Exception ee)
            {
                Log.Write(ee.Message);
                return "找不到EXCEL软件";
            }
            xlWorkBook = xlApp.Workbooks.Open(ExportXls);
            xlWorkSheet = (xls.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            #region fill Voucher
            int x = 1, y = 1;
            DataSet ds;
            if(!new PA.Helper.ExcelHelper.ExcelReader().ExcelDataSource(SourceXls, "Sheet1", out ds))
            {
                return "出错了，请联系管理员。";
            }
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
                //DataSet ds = ds;// new PA.Helper.ExcelHelper.ExcelReader().ExcelDataSource(SourceXls, "Sheet" + (i + 1));
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    foreach (DataColumn dc in ds.Tables[0].Columns)
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
        public string ExportLedger(string DetailName)
        {
            string result = "";
            xls.Application xlApp = null;
            xls.Workbook xlWorkBook;
            xls.Worksheet xlWorkSheet;

            List<Model_总账> LedgerData;
            int FirstDataLine = 0;//第一行数据的行数，0-
            if (DetailName.Substring(0, 1) == "4" || DetailName.Substring(0, 1) == "5")
            {
                LedgerData = new PA.ViewModel.ViewModel_账薄管理().GetTotalFee(DetailName, true);
                FirstDataLine = 0;
                if (LedgerData.Count == FirstDataLine)
                {
                    return "没有数据";
                }
            }
            else
            {
                LedgerData = new PA.ViewModel.ViewModel_账薄管理().GetTotalFee(DetailName);
                FirstDataLine = 1;
                if (LedgerData.Count <= FirstDataLine)
                {
                    return "没有数据";
                }
            }
            const int PageLine = 47;
            int TotalPageNum = LedgerData.Count / PageLine + 1;
            string SourceXls = Path + @"Data\打印\总账模板.xls";
            string ExportXls = Path + @"Excel\打印\总账" + DateNow + ".xls";
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

            //fill head data
            xlWorkSheet.Cells[1,6] = "总账";
            xlWorkSheet.Cells[3, 30] = DetailName.Split('\t')[1];
            xlWorkSheet.Cells[6, 1] = LedgerData[FirstDataLine].年 + "年";
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
        /// 多栏明细账打印
        /// </summary>
        /// <param name="subjectid">一级科目编号及名称</param>
        /// <param name="detailid">二级或三级科目编号</param>
        /// <returns></returns>
        public string ExportExpenditureDetails(string subjectid,string detailid)
        {
            string result = "";
            xls.Application xlApp = null;
            xls.Workbook xlWorkBook;
            xls.Worksheet xlWorkSheet;

            List<Model_费用明细> data = new PA.ViewModel.ViewModel_账薄管理().GetFeeDetail(subjectid.Split('\t')[0], detailid);
            string year;
            try
            {
                year = data[0].年;
            }
            catch (Exception)
            {
                return "没有数据";
            }
            const int PageLine = 23;
            int TotalPageNum = data.Count / PageLine + 1;
            string SourceXls = Path + @"Data\打印\管理费用模板18栏.xls";
            string ExportXls = Path + @"Excel\打印\多栏明细账" + DateNow + ".xls";
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

            //fill head data
            xlWorkSheet.Cells[1, 27] = "管    理    费    用";
            xlWorkSheet.Cells[2, 1] = "项（或目）科目名称：" + subjectid.Split('\t')[1];
            xlWorkSheet.Cells[6, 1] = year + "年";
            try
            {
                xlWorkSheet.Cells[7, "AI"] = data[0].列名[0].Split('\t')[1];
                xlWorkSheet.Cells[7, "AS"] = data[0].列名[1].Split('\t')[1];
                //第二页
                xlWorkSheet.Cells[7, "BD"] = data[0].列名[2].Split('\t')[1];
                xlWorkSheet.Cells[7, "BN"] = data[0].列名[3].Split('\t')[1];
                xlWorkSheet.Cells[7, "BX"] = data[0].列名[4].Split('\t')[1];
                xlWorkSheet.Cells[7, "CH"] = data[0].列名[5].Split('\t')[1];
                xlWorkSheet.Cells[7, "CR"] = data[0].列名[6].Split('\t')[1];
                xlWorkSheet.Cells[7, "DB"] = data[0].列名[7].Split('\t')[1];
                xlWorkSheet.Cells[7, "DL"] = data[0].列名[8].Split('\t')[1];
                xlWorkSheet.Cells[7, "DV"] = data[0].列名[9].Split('\t')[1];
                xlWorkSheet.Cells[7, "EF"] = data[0].列名[10].Split('\t')[1];
                //第三页
                xlWorkSheet.Cells[7, "EQ"] = data[0].列名[11].Split('\t')[1];
                xlWorkSheet.Cells[7, "FA"] = data[0].列名[12].Split('\t')[1];
                xlWorkSheet.Cells[7, "FK"] = data[0].列名[13].Split('\t')[1];
                xlWorkSheet.Cells[7, "FU"] = data[0].列名[14].Split('\t')[1];
                xlWorkSheet.Cells[7, "GE"] = data[0].列名[15].Split('\t')[1];
                xlWorkSheet.Cells[7, "GO"] = data[0].列名[16].Split('\t')[1];
                xlWorkSheet.Cells[7, "GY"] = data[0].列名[17].Split('\t')[1];
                xlWorkSheet.Cells[7, "HI"] = data[0].列名[18].Split('\t')[1];
                xlWorkSheet.Cells[7, "HS"] = data[0].列名[19].Split('\t')[1];
            }
            catch (ArgumentOutOfRangeException) 
            {
                Console.WriteLine("ArgumentOutOfRangeException 溢出是正常的");
            }
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
                    //第三页
                    xlWorkSheet.Cells[9 + i, 147] = data[i + (PageNum * PageLine)].金额141;
                    xlWorkSheet.Cells[9 + i, 148] = data[i + (PageNum * PageLine)].金额142;
                    xlWorkSheet.Cells[9 + i, 149] = data[i + (PageNum * PageLine)].金额143;
                    xlWorkSheet.Cells[9 + i, 150] = data[i + (PageNum * PageLine)].金额144;
                    xlWorkSheet.Cells[9 + i, 151] = data[i + (PageNum * PageLine)].金额145;
                    xlWorkSheet.Cells[9 + i, 152] = data[i + (PageNum * PageLine)].金额146;
                    xlWorkSheet.Cells[9 + i, 153] = data[i + (PageNum * PageLine)].金额147;
                    xlWorkSheet.Cells[9 + i, 154] = data[i + (PageNum * PageLine)].金额148;
                    xlWorkSheet.Cells[9 + i, 155] = data[i + (PageNum * PageLine)].金额149;
                    xlWorkSheet.Cells[9 + i, 156] = data[i + (PageNum * PageLine)].金额150;
                    xlWorkSheet.Cells[9 + i, 157] = data[i + (PageNum * PageLine)].金额151;
                    xlWorkSheet.Cells[9 + i, 158] = data[i + (PageNum * PageLine)].金额152;
                    xlWorkSheet.Cells[9 + i, 159] = data[i + (PageNum * PageLine)].金额153;
                    xlWorkSheet.Cells[9 + i, 160] = data[i + (PageNum * PageLine)].金额154;
                    xlWorkSheet.Cells[9 + i, 161] = data[i + (PageNum * PageLine)].金额155;
                    xlWorkSheet.Cells[9 + i, 162] = data[i + (PageNum * PageLine)].金额156;
                    xlWorkSheet.Cells[9 + i, 163] = data[i + (PageNum * PageLine)].金额157;
                    xlWorkSheet.Cells[9 + i, 164] = data[i + (PageNum * PageLine)].金额158;
                    xlWorkSheet.Cells[9 + i, 165] = data[i + (PageNum * PageLine)].金额159;
                    xlWorkSheet.Cells[9 + i, 166] = data[i + (PageNum * PageLine)].金额160;
                    xlWorkSheet.Cells[9 + i, 167] = data[i + (PageNum * PageLine)].金额161;
                    xlWorkSheet.Cells[9 + i, 168] = data[i + (PageNum * PageLine)].金额162;
                    xlWorkSheet.Cells[9 + i, 169] = data[i + (PageNum * PageLine)].金额163;
                    xlWorkSheet.Cells[9 + i, 170] = data[i + (PageNum * PageLine)].金额164;
                    xlWorkSheet.Cells[9 + i, 171] = data[i + (PageNum * PageLine)].金额165;
                    xlWorkSheet.Cells[9 + i, 172] = data[i + (PageNum * PageLine)].金额166;
                    xlWorkSheet.Cells[9 + i, 173] = data[i + (PageNum * PageLine)].金额167;
                    xlWorkSheet.Cells[9 + i, 174] = data[i + (PageNum * PageLine)].金额168;
                    xlWorkSheet.Cells[9 + i, 175] = data[i + (PageNum * PageLine)].金额169;
                    xlWorkSheet.Cells[9 + i, 176] = data[i + (PageNum * PageLine)].金额170;
                    xlWorkSheet.Cells[9 + i, 177] = data[i + (PageNum * PageLine)].金额171;
                    xlWorkSheet.Cells[9 + i, 178] = data[i + (PageNum * PageLine)].金额172;
                    xlWorkSheet.Cells[9 + i, 179] = data[i + (PageNum * PageLine)].金额173;
                    xlWorkSheet.Cells[9 + i, 180] = data[i + (PageNum * PageLine)].金额174;
                    xlWorkSheet.Cells[9 + i, 181] = data[i + (PageNum * PageLine)].金额175;
                    xlWorkSheet.Cells[9 + i, 182] = data[i + (PageNum * PageLine)].金额176;
                    xlWorkSheet.Cells[9 + i, 183] = data[i + (PageNum * PageLine)].金额177;
                    xlWorkSheet.Cells[9 + i, 184] = data[i + (PageNum * PageLine)].金额178;
                    xlWorkSheet.Cells[9 + i, 185] = data[i + (PageNum * PageLine)].金额179;
                    xlWorkSheet.Cells[9 + i, 186] = data[i + (PageNum * PageLine)].金额180;
                    xlWorkSheet.Cells[9 + i, 187] = data[i + (PageNum * PageLine)].金额181;
                    xlWorkSheet.Cells[9 + i, 188] = data[i + (PageNum * PageLine)].金额182;
                    xlWorkSheet.Cells[9 + i, 189] = data[i + (PageNum * PageLine)].金额183;
                    xlWorkSheet.Cells[9 + i, 190] = data[i + (PageNum * PageLine)].金额184;
                    xlWorkSheet.Cells[9 + i, 191] = data[i + (PageNum * PageLine)].金额185;
                    xlWorkSheet.Cells[9 + i, 192] = data[i + (PageNum * PageLine)].金额186;
                    xlWorkSheet.Cells[9 + i, 193] = data[i + (PageNum * PageLine)].金额187;
                    xlWorkSheet.Cells[9 + i, 194] = data[i + (PageNum * PageLine)].金额188;
                    xlWorkSheet.Cells[9 + i, 195] = data[i + (PageNum * PageLine)].金额189;
                    xlWorkSheet.Cells[9 + i, 196] = data[i + (PageNum * PageLine)].金额190;
                    xlWorkSheet.Cells[9 + i, 197] = data[i + (PageNum * PageLine)].金额191;
                    xlWorkSheet.Cells[9 + i, 198] = data[i + (PageNum * PageLine)].金额192;
                    xlWorkSheet.Cells[9 + i, 199] = data[i + (PageNum * PageLine)].金额193;
                    xlWorkSheet.Cells[9 + i, 200] = data[i + (PageNum * PageLine)].金额194;
                    xlWorkSheet.Cells[9 + i, 201] = data[i + (PageNum * PageLine)].金额195;
                    xlWorkSheet.Cells[9 + i, 202] = data[i + (PageNum * PageLine)].金额196;
                    xlWorkSheet.Cells[9 + i, 203] = data[i + (PageNum * PageLine)].金额197;
                    xlWorkSheet.Cells[9 + i, 204] = data[i + (PageNum * PageLine)].金额198;
                    xlWorkSheet.Cells[9 + i, 205] = data[i + (PageNum * PageLine)].金额199;
                    xlWorkSheet.Cells[9 + i, 206] = data[i + (PageNum * PageLine)].金额200;
                    xlWorkSheet.Cells[9 + i, 207] = data[i + (PageNum * PageLine)].金额201;
                    xlWorkSheet.Cells[9 + i, 208] = data[i + (PageNum * PageLine)].金额202;
                    xlWorkSheet.Cells[9 + i, 209] = data[i + (PageNum * PageLine)].金额203;
                    xlWorkSheet.Cells[9 + i, 210] = data[i + (PageNum * PageLine)].金额204;
                    xlWorkSheet.Cells[9 + i, 211] = data[i + (PageNum * PageLine)].金额205;
                    xlWorkSheet.Cells[9 + i, 212] = data[i + (PageNum * PageLine)].金额206;
                    xlWorkSheet.Cells[9 + i, 213] = data[i + (PageNum * PageLine)].金额207;
                    xlWorkSheet.Cells[9 + i, 214] = data[i + (PageNum * PageLine)].金额208;
                    xlWorkSheet.Cells[9 + i, 215] = data[i + (PageNum * PageLine)].金额209;
                    xlWorkSheet.Cells[9 + i, 216] = data[i + (PageNum * PageLine)].金额210;
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
        public string ExportSubjectDetails(string ParmSubjectId, string ParmDetailId)
        {
            string result = "";
            List<Model_科目明细账> data = new PA.ViewModel.ViewModel_账薄管理().GetSubjectDetail(ParmSubjectId, ParmDetailId);
            string year = "    ";
            try
            {
                year = data[0].年;
            }
            catch (Exception)
            {
                return "没有数据";
            }
            const int PageLine = 21;
            int TotalPageNum = data.Count / PageLine + 1;

            xls.Application xlApp = null;
            xls.Workbook xlWorkBook;
            xls.Worksheet xlWorkSheet;
            string SourceXls = Path + @"Data\打印\三栏明细账模板.xls";
            string ExportXls = Path + @"Excel\打印\三栏明细账" + DateNow + ".xls";
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
        public string ExportBalanceSheet(int ParmPeroid, string People)
        {
            string result = "";
            #region init Excel
            xls.Application xlApp = null;
            xls.Workbook xlWorkBook;
            xls.Worksheet xlWorkSheet;
            string SourceXls = Path + @"Data\打印\资产负债表模板.xls";
            string ExportXls = Path + @"Excel\打印\资产负债表" + DateNow + ".xls";
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
            if(data.Count < 1)
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
            xlWorkSheet.Cells[1, "C"] = CommonInfo.年 + " 年 " + ParmPeroid + " 月 资 产 负 债 表（行 政）";
            xlWorkSheet.Cells[3, "A"] = "编制单位：" + CommonInfo.制表单位;

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
            xlWorkSheet.Cells[28, "C"] = "填表人：" + People;
            xlWorkSheet.Cells[28, "E"] = "填表日期：" + DateTime.Now.ToLongDateString();
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
            string ExportXls = Path + @"Excel\打印\收入支出总表" + DateNow + ".xls";
            try
            {
                File.Copy(SourceXls, ExportXls, true);
            }
            catch (FileNotFoundException fe)
            {
                Log.Write(fe.Message);
                return "模板文件未找到";
            }
            catch (IOException ioe)
            {
                Log.Write(ioe.Message);
                return "文件锁定，请关闭Excel再试";
            }
            try
            {
                xlApp = new xls.Application();
            }
            catch (Exception ee)
            {
                Log.Write(ee.Message);
                return "找不到EXCEL软件";
            }
            xlWorkBook = xlApp.Workbooks.Open(ExportXls);
            xlWorkSheet = (xls.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            #endregion

            #region fill data

            //第一次对一级科目赋值
            List<Model_报表类> data = new PA.ViewModel.ViewModel_ReportManager().GetIncomeAndExpenses(ParmPeroid, new ViewModel.ViewModel_科目管理().GetOneSubjectList());

            decimal dy = 0;
            decimal dn = 0;
            decimal insumm1 = 0;
            decimal insumy1 = 0;
            decimal insumm2 = 0;
            decimal insumy2 = 0;
            decimal insumy3 = 0;

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
                            if (!m.编号.StartsWith("3"))
                            {
                                xlWorkSheet.Cells[y + 1, x] = m.本期数;
                                xlWorkSheet.Cells[y + 1, x + 1] = m.累计数;
                            }
                            else
                            {
                                xlWorkSheet.Cells[y + 1, x] = m.累计数;
                            }
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
                                insumy3 = dy;
                            }
                        }
                    }
                    x++;
                }
                y++;
                x = 1;
            }
            xlWorkSheet.Cells[1, "D"] = CommonInfo.年 + " 年 " + ParmPeroid + " 月 收 入 支 出 总 表（行 政）";
            xlWorkSheet.Cells[16, "B"] = insumm1;
            xlWorkSheet.Cells[16, "C"] = insumy1;
            xlWorkSheet.Cells[16, "E"] = insumm2;
            xlWorkSheet.Cells[16, "F"] = insumy2;
            xlWorkSheet.Cells[16, "H"] = insumy3;
            xlWorkSheet.Cells[6, "H"] = insumy3;
            xlWorkSheet.Cells[3, "A"] = "编制单位：" + CommonInfo.制表单位;
            xlWorkSheet.Cells[3, "D"] = DateTime.Today.ToLongDateString();

            //第一次对二级科目赋值
            List<Model_报表类> data2 = new PA.ViewModel.ViewModel_ReportManager().GetIncomeAndExpensesForTwoSubject(ParmPeroid, new ViewModel.ViewModel_科目管理().GetIncomeAndOutSubjectList());
            x = 1;
            y = 1;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                foreach (DataColumn dc in ds.Tables[0].Columns)
                {
                    string key = dr[dc].ToString();
                    foreach (Model_报表类 m in data2)
                    {
                        if (key == "inM" + m.编号)
                        {
                            xlWorkSheet.Cells[y + 1, x] = m.本期数;
                            xlWorkSheet.Cells[y + 1, x + 1] = m.累计数;
                            decimal.TryParse(m.累计数, out dy);
                            decimal.TryParse(m.本期数, out dn);
                            if (m.编号.StartsWith("30601"))
                            {
                                xlWorkSheet.Cells[7, "H"] = m.累计数;
                            }
                            else if (m.编号.StartsWith("30602"))
                            {
                                xlWorkSheet.Cells[8, "H"] = m.累计数;
                            }
                        }
                    }
                    x++;
                }
                y++;
                x = 1;
            }

            xlWorkBook.Save();

            x = 1;
            y = 1;
            if (!new PA.Helper.ExcelHelper.ExcelReader().ExcelDataSource(ExportXls, "Sheet1", out ds))
            {
                return "出错了，请联系管理员。";
            }
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                foreach (DataColumn dc in ds.Tables[0].Columns)
                {
                    string key = dr[dc].ToString();
                    if (key.StartsWith("in") || key.StartsWith("B"))
                    {
                        xlWorkSheet.Cells[y + 1, x] = "";
                    }
                    x++;
                }
                y++;
                x = 1;
            }


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
            string SourceXls = Path + @"Data\打印\行政费用支出明细表模板3.xls";
            string ExportXls = Path + @"Excel\打印\行政费用支出明细表" + DateNow + ".xls";
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
            List<Model_报表类> data = new PA.ViewModel.ViewModel_ReportManager().GetAdministrativeExpenseDetail(ParmPeroid,501);
            if(data.Count <= 0)
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
            if(!new PA.Helper.ExcelHelper.ExcelReader().ExcelDataSource(SourceXls, "Sheet1", out ds))
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
                        foreach(Model_报表类 a in data)
                        {
                            if(key == a.编号)
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
            for (int i = 0; i < 7; i++ )
            {
                decimal temp101 = 0m, temp102 = 0m;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i + 9, "B"]).Text, out temp101);
                b101 += temp101;
                decimal.TryParse(((xls.Range)xlWorkSheet.Cells[i + 9, "C"]).Text, out temp102);
                b102 += temp102;
            }
            for (int i = 17; i < 34; i++ )
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

            xlWorkSheet.Cells[3, "A"] = "编制单位：" + CommonInfo.制表单位;
            xlWorkSheet.Cells[1, "B"] = CommonInfo.年 + "年" + ParmPeroid + "月行政经费支出明细表";
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
