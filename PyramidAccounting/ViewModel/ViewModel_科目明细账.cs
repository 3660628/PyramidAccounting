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
                    List<string> _list = new List<string>();
                    _list = Turn(d[3].ToString());
                    m.借方金额1 = _list[0];
                    m.借方金额2 = _list[1];
                    m.借方金额3 = _list[2];
                    m.借方金额4 = _list[3];
                    m.借方金额5 = _list[4];
                    m.借方金额6 = _list[5];
                    m.借方金额7 = _list[6];
                    m.借方金额8 = _list[7];
                    m.借方金额9 = _list[8];
                    m.借方金额10 = _list[9];
                    m.借方金额11 = _list[10];
                    m.借方金额12 = _list[11];
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

        /// <summary>
        /// 金额转换算法
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private List<string> Turn(string value)
        {
            List<string> list = new List<string>(12);
            int length = value.Length;
            value = value.Replace(".", "");
            string s = string.Empty;
            char[] cc = value.ToCharArray();

            if (value.IndexOf(".") > 0)
            {
                for(int j = 0 ; j < 12-length; j ++)
                {
                    list.Add(s);
                }
                for (int i = 0; i < length; i++)
                {
                    s = cc[i].ToString();
                    list.Add(s);
                }
            }
            else
            {
                for (int j = 0; j < 10 - length; j++)
                {
                    list.Add(s);
                }
                for (int i = 0; i < length; i++)
                {
                    s = cc[i].ToString();
                    Console.WriteLine(s);
                    list.Add(s);
                }
                list.Add("0");
                list.Add("0");
            }
            return list;
        }

    }
}
