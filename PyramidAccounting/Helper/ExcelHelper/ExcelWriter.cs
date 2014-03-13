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
    class ExcelWriter
    {
        xls.Application xlApp;
        xls.Workbook xlWorkBook;
        xls.Worksheet xlWorkSheet;
        object misValue = System.Reflection.Missing.Value;


        /*

        public void ExportExcel(DataSet ds, Pyramid.src.Model.Model_加工单 加工单)
        {
            File.Copy(AppDomain.CurrentDomain.BaseDirectory + "Data\\正艺纸塑工艺厂加工单.xls", AppDomain.CurrentDomain.BaseDirectory + "正艺纸塑工艺厂加工单.xls", true);
            xlWorkBook = xlApp.Workbooks.Open(AppDomain.CurrentDomain.BaseDirectory + "正艺纸塑工艺厂加工单.xls");
            xlWorkSheet = (xls.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            int x = 1, y = 1;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                foreach (DataColumn dc in ds.Tables[0].Columns)
                {
                    if (dr[dc] != null && dr[dc].ToString() != "")
                    {
                        if (dr[dc].ToString().StartsWith("$", false, null))
                        {
                            string key = dr[dc].ToString().Replace("$", "");
                            System.Reflection.PropertyInfo[] properties = 加工单.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                            foreach (System.Reflection.PropertyInfo item in properties)
                            {
                                if (item.PropertyType.Name.StartsWith("String"))
                                {
                                    if (item.Name == key)
                                    {
                                        xlWorkSheet.Cells[y + 1, x] = item.GetValue(加工单, null);
                                    }
                                }
                                else if (item.PropertyType.Name.StartsWith("Boolean"))
                                {
                                    if (item.Name == key)
                                    {
                                        if (item.GetValue(加工单, null).ToString() == "True")
                                        {
                                            xlWorkSheet.Cells[y + 1, x] = "√";
                                        }
                                        else
                                        {
                                            xlWorkSheet.Cells[y + 1, x] = "";
                                        }
                                    }
                                }
                            }
                            //xlWorkSheet.Cells[y + 1, x] = "";
                        }
                    }
                    x++;
                }
                y++;
                x = 1;
            }
            xlApp.Visible = true;
        }

        */

    }
}
