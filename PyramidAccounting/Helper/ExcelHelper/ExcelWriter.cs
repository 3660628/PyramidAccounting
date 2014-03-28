using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Model_凭证单 Voucher = new PA.ViewModel.ViewModel_凭证管理().GetVoucher(guid);
            List<Model_凭证明细> VoucherDetails = new PA.ViewModel.ViewModel_凭证管理().GetVoucherDetails(guid);

            string SourceXls = Path + @"Data\打印\记账凭证模板.xls";
            string ExportXls = Path + @"Data\打印\记账凭证export.xls";
            File.Copy(SourceXls, ExportXls, true);
            xlWorkBook = xlApp.Workbooks.Open(ExportXls);
            xlWorkSheet = (xls.Worksheet)xlWorkBook.Worksheets.get_Item(1);
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
                                    xlWorkSheet.Cells[y + 1, x] = "";
                                    string money = item.GetValue(Voucher, null).ToString();
                                    List<string> ListMoney = TransMoney(money);
                                    for (int i = ListMoney.Count - 1; i >= 0; i--)
                                    {
                                        xlWorkSheet.Cells[y + 1, x + 10 - i] = ListMoney[i];
                                    }
                                }
                                else
                                {
                                    xlWorkSheet.Cells[y + 1, x] = item.GetValue(Voucher, null);
                                }
                            }
                        }

                        if (key.StartsWith("摘要", false, null) || key.StartsWith("科目", false, null) || key.StartsWith("子细目", false, null) || key.StartsWith("记账", false, null))
                        {
                            int id = int.Parse(key.Substring(key.Length - 1, 1))-1;
                            if (id < VoucherDetails.Count)
                            {
                                if (key.StartsWith("摘要", false, null))
                                {
                                    xlWorkSheet.Cells[y + 1, x] = VoucherDetails[id].摘要;
                                }
                                else if (key.StartsWith("科目", false, null))
                                {
                                    xlWorkSheet.Cells[y + 1, x] = VoucherDetails[id].科目编号;
                                }
                                else if (key.StartsWith("子细目", false, null))
                                {
                                    xlWorkSheet.Cells[y + 1, x] = VoucherDetails[id].子细目;
                                }
                                else if (key.StartsWith("记账", false, null))
                                {
                                    xlWorkSheet.Cells[y + 1, x] = (VoucherDetails[id].记账 == 1) ? "√" : "";
                                }
                            }
                        }
                        else if (key.StartsWith("借方", false, null) || key.StartsWith("贷方", false, null))
                        {
                            xlWorkSheet.Cells[y + 1, x] = "";
                            int id = int.Parse(key.Substring(key.Length - 1, 1)) - 1;
                            if (id < VoucherDetails.Count)
                            {
                                string money = "0";
                                if (key.StartsWith("借方", false, null))
                                {
                                    money = VoucherDetails[id].借方.ToString();
                                }
                                else if (key.StartsWith("贷方", false, null))
                                {
                                    money = VoucherDetails[id].贷方.ToString();
                                }
                                if(money == "0")
                                {
                                    x++;
                                    continue;
                                }
                                List<string> ListMoney = TransMoney(money);
                                for (int i = ListMoney.Count - 1; i >= 0; i--)
                                {
                                    xlWorkSheet.Cells[y + 1, x +10-i] = ListMoney[i];
                                }
                            }
                        }
                    }
                    x++;
                }
                y++;
                x = 1;
            }
            xlApp.Visible = true;

            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
        }
        private List<string> TransMoney(string parm)
        {
            List<string> result = new List<string>();
            string m1 = string.Empty;
            if(parm.IndexOf('.') > 0)
            {
                m1 = parm.Split('.')[0];
                string m2 = parm.Split('.')[1];
                result.Add(m2.Substring(1, 1));
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
