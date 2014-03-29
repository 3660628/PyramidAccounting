using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.Model.DataGrid;
using PA.Helper.DataBase;
using PA.Helper.DataDefind;
using System.Data;
using PA.Model.ComboBox;

namespace PA.ViewModel
{
    class ViewModel_账薄管理
    {
        private DataBase db = new DataBase();
        public List<Model_总账> GetData(string subject_id)
        {
            List<Model_总账> list = new List<Model_总账>();
            string sql = "select time,number,comments,sum(fee1),sum(fee2) from "
                + "(select b.op_time as time ,a.voucher_no as number,a.abstract as comments,a.debit as fee1,a.credit as fee2 from " 
                + DBTablesName.T_VOUCHER_DETAIL
                + " a left join " 
                + DBTablesName.T_VOUCHER 
                + " b on a.parentid=b.id where a.subject_id='"
                + subject_id.Split('\t')[1]
                + "'" + " and b.delete_mark=0 order by b.op_time)t group by t.number ";

            //判断第一期查年初数
            //以后差每一期期末数
            string sql2 = "select fee from t_yearfee where subject_id='" + subject_id.Split('\t')[0] + "' and bookid='" + CommonInfo.账薄号 + "'";
            string yearfee = db.GetAllData(sql2).Split('\t')[0].Split(',')[0];
            DataSet ds = new DataSet();
            ds = db.Query(sql);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                foreach (DataRow d in dt.Rows)
                {
                    Model_总账 m = new Model_总账();
                    string date = d[0].ToString().Split(' ')[0];
                    m.年 = date.Split('/')[0];
                    m.月 = date.Split('/')[1];
                    m.日 = date.Split('/')[2];
                    m.号数 = d[1].ToString();
                    m.摘要 = d[2].ToString();
                    m.借方金额 = d[3].ToString();
                    m.贷方金额 = d[4].ToString();
                    string temp = string.Empty;
                    List<string> _list = new List<string>();
                    if (string.IsNullOrEmpty(m.借方金额))
                    {
                        temp = d[4].ToString();
                        _list = Turn(d[4].ToString(), 12);
                        m.贷方金额1 = _list[0];
                        m.贷方金额2 = _list[1];
                        m.贷方金额3 = _list[2];
                        m.贷方金额4 = _list[3];
                        m.贷方金额5 = _list[4];
                        m.贷方金额6 = _list[5];
                        m.贷方金额7 = _list[6];
                        m.贷方金额8 = _list[7];
                        m.贷方金额9 = _list[8];
                        m.贷方金额10 = _list[9];
                        m.贷方金额11 = _list[10];
                        m.贷方金额12 = _list[11];
                        m.借或贷 = "贷";

                    }
                    else
                    {
                        temp = d[3].ToString();
                        _list = Turn(d[3].ToString(), 12);
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
                        m.借或贷 = "借";
                    }

                    yearfee = (Convert.ToDecimal(yearfee) - Convert.ToDecimal(temp)).ToString();
                    _list.Clear();
                    _list = Turn(yearfee, 12);
                    m.余额1 = _list[0];
                    m.余额2 = _list[1];
                    m.余额3 = _list[2];
                    m.余额4 = _list[3];
                    m.余额5 = _list[4];
                    m.余额6 = _list[5];
                    m.余额7 = _list[6];
                    m.余额8 = _list[7];
                    m.余额9 = _list[8];
                    m.余额10 = _list[9];
                    m.余额11 = _list[10];
                    m.余额12 = _list[11];
                    _list.Clear();
                    list.Add(m);
                }
            }
            return list;
        }
        public List<Model_费用明细> GetFeeDetail(string subject_id)
        {
            ComboBox_科目 cb = new ComboBox_科目();
            List<string> lst = new List<string>();
            lst = cb.GetChildSubjectList("",subject_id);
            string _tempstr = string.Empty;
            foreach(string i in lst)
            {
                _tempstr += ",sum(case when detail='" + i + "' then (debit+credit) else '0' end) as " + i;
            }

            List<Model_费用明细> list = new List<Model_费用明细>();
            string sql = "select b.op_time,a.voucher_no,a.abstract,a.debit,a.credit from "
                + DBTablesName.T_VOUCHER_DETAIL
                + " a left join "
                + DBTablesName.T_VOUCHER
                + " b on a.parentid=b.id where a.subject_id='"
                + subject_id.Split('\t')[1]
                + "'" + " and b.delete_mark=0 order by b.op_time) ";

            //判断第一期查年初数
            //以后差每一期期末数
            string sql2 = "select fee from t_yearfee where subject_id='" + subject_id.Split('\t')[0] + "' and bookid='" + CommonInfo.账薄号 + "'";
            string yearfee = db.GetAllData(sql2).Split('\t')[0].Split(',')[0];
            DataSet ds = new DataSet();
            ds = db.Query(sql);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                foreach (DataRow d in dt.Rows)
                {
                    Model_费用明细 m = new Model_费用明细();
                    string date = d[0].ToString().Split(' ')[0];
                    m.年 = date.Split('/')[0];
                    m.月 = date.Split('/')[1];
                    m.日 = date.Split('/')[2];
                    m.号数 = d[1].ToString();
                    m.摘要 = d[2].ToString();
                    m.借方金额 = d[3].ToString();
                    m.贷方金额 = d[4].ToString();
                    string temp = string.Empty;
                    List<string> _list = new List<string>();
                    if (string.IsNullOrEmpty(m.借方金额))
                    {
                        temp = d[4].ToString();
                        _list = Turn(d[4].ToString(), 10);
                        m.贷方金额1 = _list[0];
                        m.贷方金额2 = _list[1];
                        m.贷方金额3 = _list[2];
                        m.贷方金额4 = _list[3];
                        m.贷方金额5 = _list[4];
                        m.贷方金额6 = _list[5];
                        m.贷方金额7 = _list[6];
                        m.贷方金额8 = _list[7];
                        m.贷方金额9 = _list[8];
                        m.贷方金额10 = _list[9];
                    }
                    else
                    {
                        temp = d[3].ToString();
                        _list = Turn(d[3].ToString(), 12);
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
                    }

                    yearfee = (Convert.ToDecimal(yearfee) - Convert.ToDecimal(temp)).ToString();
                    _list.Clear();
                    _list = Turn(yearfee, 12);
                    m.余额1 = _list[0];
                    m.余额2 = _list[1];
                    m.余额3 = _list[2];
                    m.余额4 = _list[3];
                    m.余额5 = _list[4];
                    m.余额6 = _list[5];
                    m.余额7 = _list[6];
                    m.余额8 = _list[7];
                    m.余额9 = _list[8];
                    m.余额10 = _list[9];
                    _list.Clear();
                    list.Add(m);
                }
            }
            return list;
        }
        /// <summary>
        /// 科目明细账查询方法
        /// </summary>
        /// <param name="subject_id"></param>
        /// <param name="detail"></param>
        /// <param name="childSubjectId"></param>
        /// <returns></returns>
        public List<Model_科目明细账> GetData(string subject_id,string detail)
        {
            List<Model_科目明细账> list = new List<Model_科目明细账>();
            string sql = "select b.op_time,a.voucher_no,a.abstract,a.debit,a.credit from " 
                + DBTablesName.T_VOUCHER_DETAIL
                + " a left join " 
                + DBTablesName.T_VOUCHER 
                + " b on a.parentid=b.id where a.subject_id='"
                + subject_id
                + "'" + " and b.delete_mark=0 and  a.detail='"
                + detail.Split('\t')[1] + "' order by b.op_time";

            //判断第一期查年初数
            //以后差每一期期末数
            string sql2 = "select fee from t_yearfee where subject_id='" + detail.Split('\t')[0] + "' and bookid='" + CommonInfo.账薄号 + "'";
            string yearfee = db.GetAllData(sql2).Split('\t')[0].Split(',')[0];
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
                    string temp = string.Empty;
                    List<string> _list = new List<string>();
                    if (string.IsNullOrEmpty(m.借方金额))
                    {
                        temp = d[4].ToString();
                        _list = Turn(d[4].ToString(),12);
                        m.贷方金额1 = _list[0];
                        m.贷方金额2 = _list[1];
                        m.贷方金额3 = _list[2];
                        m.贷方金额4 = _list[3];
                        m.贷方金额5 = _list[4];
                        m.贷方金额6 = _list[5];
                        m.贷方金额7 = _list[6];
                        m.贷方金额8 = _list[7];
                        m.贷方金额9 = _list[8];
                        m.贷方金额10 = _list[9];
                        m.贷方金额11 = _list[10];
                        m.贷方金额12 = _list[11];
                        m.借或贷 = "贷";
                       
                    }
                    else
                    {
                        temp = d[3].ToString();
                        _list = Turn(d[3].ToString(),12);
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
                        m.借或贷 = "借";
                    }

                    yearfee = (Convert.ToDecimal(yearfee) - Convert.ToDecimal(temp)).ToString();
                    _list.Clear();
                    _list = Turn(yearfee,12);
                    m.余额1 = _list[0];
                    m.余额2 = _list[1];
                    m.余额3 = _list[2];
                    m.余额4 = _list[3];
                    m.余额5 = _list[4];
                    m.余额6 = _list[5];
                    m.余额7 = _list[6];
                    m.余额8 = _list[7];
                    m.余额9 = _list[8];
                    m.余额10 = _list[9];
                    m.余额11 = _list[10];
                    m.余额12 = _list[11];
                    _list.Clear();

                    list.Add(m);
                }
            }
            return list;
        }

        /// <summary>
        /// 金额转换算法
        /// </summary>
        /// <param name="value">转换数</param>
        /// <param name="size">位数</param>
        /// <returns>List</returns>
        private List<string> Turn(string value,int size)
        {
            List<string> list = new List<string>();
            int length = value.Length;
            value = value.Replace(".", "");
            string s = string.Empty;
            char[] cc = value.ToCharArray();

            if (value.IndexOf(".") > 0)
            {
                for (int j = 0; j < size - length; j++)
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
                for (int j = 0; j < size - 2 - length; j++)
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
