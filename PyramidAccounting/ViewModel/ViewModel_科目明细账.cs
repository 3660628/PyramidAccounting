using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.Model.DataGrid;
using PA.Helper.DataBase;
using PA.Helper.DataDefind;
using System.Data;

namespace PA.ViewModel
{
    class ViewModel_科目明细账
    {
        private DataBase db = new DataBase();
        public List<Model_科目明细账> GetData(string subject_name,string detail)
        {
            List<Model_科目明细账> list = new List<Model_科目明细账>();
            string sql = "select b.op_time,a.voucher_no,a.abstract,a.debit,a.credit from " 
                + DBTablesName.T_VOUCHER_DETAIL
                + " a left join " 
                + DBTablesName.T_VOUCHER 
                + " b on a.parentid=b.id where a.subject_id='"
                + subject_name 
                + "'" + " and  a.detail='"
                + detail + "' order by b.op_time";
            DataSet ds = new DataSet();
            ds = db.Query(sql);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                foreach (DataRow d in dt.Rows)
                {
                    Model_科目明细账 m = new Model_科目明细账();
                    string date = d[0].ToString().Split(' ')[0];
                    m.年 = date.Split('/')[0];
                    m.月 = date.Split('/')[1];
                    m.日 = date.Split('/')[2];
                    m.号数 = d[1].ToString();
                    m.摘要 = d[2].ToString();
                    m.借方金额 = d[3].ToString();
                    m.贷方金额 = d[4].ToString();
                    if (string.IsNullOrEmpty(m.借方金额))
                    {
                        m.借或贷 = "贷";
                    }
                    else
                    {
                        m.借或贷 = "借";
                    }
                    list.Add(m);
                }
            }
            return list;
        }
        private void Turn(string value)
        {
            List<string> list = new List<string>();
            if (value.IndexOf(".") > 0)
            {
                value = value.Replace(".", "");
                int length = value.Length;
                for(int j = 0 ; j < 12; j ++)
                {
                    string s = string.Empty;
                    if (j > 12 - length)
                    {
                    }

                    list.Add(s);

                }
            }
        }
    }
}
