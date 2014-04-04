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
            string id = subject_id.Split('\t')[0];
            string name = subject_id.Split('\t')[1];
            string sql = "select strftime(op_time),VOUCHER_NUMS,COMMENTS,DEBIT,CREDIT,FEE,mark from " 
                + DBTablesName.T_FEE + " where delete_mark=0 and subject_id='" + id + "' order by op_time";

            DataSet ds = new DataSet();
            decimal debit = 0;
            decimal credit = 0;
            ds = db.Query(sql);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                int count = 0;
                foreach (DataRow d in dt.Rows)
                {
                    Model_总账 m = new Model_总账();
                    if (!string.IsNullOrEmpty(d[0].ToString()))
                    {
                        string date = d[0].ToString().Split(' ')[0];
                        m.年 = date.Split('-')[0];
                        m.月 = date.Split('-')[1];
                        m.日 = date.Split('-')[2];
                    }
                    m.号数 = d[1].ToString();
                    m.摘要 = d[2].ToString();
                    m.借方金额 = d[3].ToString();
                    m.贷方金额 = d[4].ToString();
                    m.余额 = d[5].ToString();
                    if (count == 0)
                    {
                        m.借或贷 = d[6].ToString().Equals("1") ? "借" : "贷";
                    }
                    else
                    {
                        if (credit == debit)
                        {
                            m.借或贷 = "平";
                        }
                        else if (credit > debit)
                        {
                            m.借或贷 = "贷";
                        }
                        else
                        {
                            m.借或贷 = "借";
                        }
                    }
                    string temp = string.Empty;
                    List<string> _list = new List<string>();

                    _list = Turn(m.贷方金额, 12);
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

                    _list = Turn(m.借方金额, 12);
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

                    decimal.TryParse(m.贷方金额, out credit);
                    decimal.TryParse(m.借方金额, out debit);
                    
                    
                    _list.Clear();
                    _list = Turn(m.余额, 12);
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
                    count++;
                }
            }
            return list;
        }
        public List<Model_费用明细> GetFeeDetail(string subject_id)
        {
            ComboBox_科目 cb = new ComboBox_科目();
            List<string> lst = new List<string>();
            lst = cb.GetChildSubjectList("",subject_id.Split('\t')[0]);
            string _tempstr = string.Empty;
            foreach(string i in lst)
            {
                _tempstr += ",sum(case when t.detail='" + i.Split('\t')[1] 
                    + "' then (t.fee1+t.fee2) else '0' end) as " 
                    + i.Split('\t')[1];
            }
            for (int i = lst.Count; i < 11; i++)
            {
                _tempstr += ",0";
            }

            List<Model_费用明细> list = new List<Model_费用明细>();
            string sql = "select strftime(time),number,comments,sum(fee1),sum(fee2)" + _tempstr + " from " 
                + "(select b.op_time as time ,a.voucher_no as number,a.abstract as comments,a.debit as fee1,a.credit as fee2,a.detail as detail from "
                + DBTablesName.T_VOUCHER_DETAIL
                + " a left join "
                + DBTablesName.T_VOUCHER
                + " b on a.parentid=b.id where a.subject_id='"
                + subject_id.Split('\t')[1]
                + "'" + " and b.delete_mark=0 order by b.op_time)t group by t.number,t.time ";

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
                    #region 赋值
                    m.年 = date.Split('-')[0];
                    m.月 = date.Split('-')[1];
                    m.日 = date.Split('-')[2];
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
                        _list = Turn(d[3].ToString(), 10);
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
                    _list = Turn(yearfee, 10);
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

                    
                    _list = Turn(d[5].ToString(), 10);
                    m.金额31 = _list[0];
                    m.金额32 = _list[1];
                    m.金额33 = _list[2];
                    m.金额34 = _list[3];
                    m.金额35 = _list[4];
                    m.金额36 = _list[5];
                    m.金额37 = _list[6];
                    m.金额38 = _list[7];
                    m.金额39 = _list[8];
                    m.金额40 = _list[9];

                    _list.Clear();
                    _list = Turn(d[6].ToString(), 10);
                    m.金额41 = _list[0];
                    m.金额42 = _list[1];
                    m.金额43 = _list[2];
                    m.金额44 = _list[3];
                    m.金额45 = _list[4];
                    m.金额46 = _list[5];
                    m.金额47 = _list[6];
                    m.金额48 = _list[7];
                    m.金额49 = _list[8];
                    m.金额50 = _list[9];

                    _list.Clear();
                    _list = Turn(d[7].ToString(), 10);
                    m.金额51 = _list[0];
                    m.金额52 = _list[1];
                    m.金额53 = _list[2];
                    m.金额54 = _list[3];
                    m.金额55 = _list[4];
                    m.金额56 = _list[5];
                    m.金额57 = _list[6];
                    m.金额58 = _list[7];
                    m.金额59 = _list[8];
                    m.金额60 = _list[9];

                    _list.Clear();
                    _list = Turn(d[8].ToString(), 10);
                    m.金额61 = _list[0];
                    m.金额62 = _list[1];
                    m.金额63 = _list[2];
                    m.金额64 = _list[3];
                    m.金额65 = _list[4];
                    m.金额66 = _list[5];
                    m.金额67 = _list[6];
                    m.金额68 = _list[7];
                    m.金额69 = _list[8];
                    m.金额70 = _list[9];

                    _list.Clear();
                    _list = Turn(d[9].ToString(), 10);
                    m.金额71 = _list[0];
                    m.金额72 = _list[1];
                    m.金额73 = _list[2];
                    m.金额74 = _list[3];
                    m.金额75 = _list[4];
                    m.金额76 = _list[5];
                    m.金额77 = _list[6];
                    m.金额78 = _list[7];
                    m.金额79 = _list[8];
                    m.金额80 = _list[9];

                    _list.Clear();
                    _list = Turn(d[10].ToString(), 10);
                    m.金额81 = _list[0];
                    m.金额82 = _list[1];
                    m.金额83 = _list[2];
                    m.金额84 = _list[3];
                    m.金额85 = _list[4];
                    m.金额86 = _list[5];
                    m.金额87 = _list[6];
                    m.金额88 = _list[7];
                    m.金额89 = _list[8];
                    m.金额90 = _list[9];

                    _list.Clear();
                    _list = Turn(d[11].ToString(), 10);
                    m.金额91 = _list[0];
                    m.金额92 = _list[1];
                    m.金额93 = _list[2];
                    m.金额94 = _list[3];
                    m.金额95 = _list[4];
                    m.金额96 = _list[5];
                    m.金额97 = _list[6];
                    m.金额98 = _list[7];
                    m.金额99 = _list[8];
                    m.金额100 = _list[9];

                    _list.Clear();
                    _list = Turn(d[12].ToString(), 10);
                    m.金额101 = _list[0];
                    m.金额102 = _list[1];
                    m.金额103 = _list[2];
                    m.金额104 = _list[3];
                    m.金额105 = _list[4];
                    m.金额106 = _list[5];
                    m.金额107 = _list[6];
                    m.金额108 = _list[7];
                    m.金额109 = _list[8];
                    m.金额110 = _list[9];

                    _list.Clear();
                    _list = Turn(d[13].ToString(), 10);
                    m.金额111 = _list[0];
                    m.金额112 = _list[1];
                    m.金额113 = _list[2];
                    m.金额114 = _list[3];
                    m.金额115 = _list[4];
                    m.金额116 = _list[5];
                    m.金额117 = _list[6];
                    m.金额118 = _list[7];
                    m.金额119 = _list[8];
                    m.金额120 = _list[9];

                    _list.Clear();
                    _list = Turn(d[14].ToString(), 10);
                    m.金额121 = _list[0];
                    m.金额122 = _list[1];
                    m.金额123 = _list[2];
                    m.金额124 = _list[3];
                    m.金额125 = _list[4];
                    m.金额126 = _list[5];
                    m.金额127 = _list[6];
                    m.金额128 = _list[7];
                    m.金额129 = _list[8];
                    m.金额130 = _list[9];

                    _list.Clear();
                    _list = Turn(d[15].ToString(), 10);
                    m.金额131 = _list[0];
                    m.金额132 = _list[1];
                    m.金额133 = _list[2];
                    m.金额134 = _list[3];
                    m.金额135 = _list[4];
                    m.金额136 = _list[5];
                    m.金额137 = _list[6];
                    m.金额138 = _list[7];
                    m.金额139 = _list[8];
                    m.金额140 = _list[9];

                    m.列名 = lst;
                    #endregion
                    list.Add(m);
                }
            }
            return list;
        }
        /// <summary>
        /// 科目明细账查询方法
        /// </summary>
        /// <param name="subject_id">一级科目</param>
        /// <param name="detail">二级科目</param>
        /// <param name="peroid">查询期</param>
        /// <returns></returns>
        public List<Model_科目明细账> GetData(string subject_id,string detail,int peroid)
        {
            List<Model_科目明细账> list = new List<Model_科目明细账>();
            string sql = "select strftime(b.op_time),a.voucher_no,a.abstract,a.debit,a.credit,a.credit-a.debit,case when a.debit>a.credit then '借' when a.debit<a.credit then '贷' else '平' end  from " 
                + DBTablesName.T_VOUCHER_DETAIL
                + " a left join " 
                + DBTablesName.T_VOUCHER 
                + " b on a.parentid=b.id where a.subject_id='"
                + subject_id
                + "'" + " and b.delete_mark=0 and  a.detail='"
                + detail.Split('\t')[1] + "' order by b.op_time";

            //判断第一期查年初数
            //以后差每一期期末数
            string sql2 = "select abs(a.fee*b.borrow_mark) from "+ DBTablesName.T_YEAR_FEE + " a left join " + DBTablesName.T_SUBJECT 
                +" b on a.subject_id=b.subject_id where a.subject_id='" + detail.Split('\t')[0] 
                + "' and a.bookid='" + CommonInfo.账薄号 + "'";
            string yearfee = db.GetAllData(sql2).Split('\t')[0].Split(',')[0];
            DataSet ds = new DataSet();
            ds = db.Query(sql);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                foreach (DataRow d in dt.Rows)
                {
                    List<string> _list = new List<string>();
                    Model_科目明细账 m = new Model_科目明细账();
                    string date = d[0].ToString().Split(' ')[0];
                    m.年 = date.Split('-')[0];
                    m.月 = date.Split('-')[1];
                    m.日 = date.Split('-')[2];
                    m.号数 = d[1].ToString();
                    m.摘要 = d[2].ToString();
                    m.借方金额 = d[3].ToString();
                    m.贷方金额 = d[4].ToString();
                    string temp = string.Empty;
                    temp = d[5].ToString();  //差额
                    m.借或贷 = d[6].ToString();

                    _list = Turn(m.贷方金额, 12);
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

                    _list = Turn(m.借方金额, 12);
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
            if (value.Equals("0"))
            {
                for (int i = 0; i < size;i++ )
                {
                    list.Add(string.Empty);
                }
                return list;
            }
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
                    list.Add(s);
                }
                list.Add("0");
                list.Add("0");
            }
            return list;
        }
        /// <summary>
        /// 结账
        /// </summary>
        public bool CheckOut()
        {
            //string date = DateTime.Now.ToString("yyyy-MM-dd");
            string sql = "INSERT INTO " + DBTablesName.T_FEE
                          + "(OP_TIME,PERIOD,SUBJECT_ID,VOUCHER_NUMS,COMMENTS,DEBIT,CREDIT,MARK,DELETE_MARK,FEE)"
                          + " select t.*,b.mark,0,abs(b.mark*b.fee-(t.credit-t.debit)) as fee from ("
                          + "SELECT datetime('now', 'localtime') as op_time," + CommonInfo.当前期
                          + ",b.subject_id as subject_id,"
                          + "a.voucher_nums,"
                          + "a.comments,"
                          + "a.debit as debit,"
                          + "a.CREDIT as credit "
                          + "FROM (SELECT min(VOUCHER_NO) || '-' || max(VOUCHER_NO) AS voucher_nums,"
                          + "SUBJECT_ID,    "
                          + "SUBJECT_ID || '汇总' AS comments,"
                          + "sum(DEBIT) AS DEBIT,"
                          + "sum(CREDIT) AS CREDIT"
                          + " FROM "
                          + DBTablesName.T_VOUCHER_DETAIL
                          + " WHERE PARENTID IN (  "
                          + "SELECT ID  FROM   "
                          + DBTablesName.T_VOUCHER
                          + " WHERE period = " + CommonInfo.当前期
                          + " and review_mark=1) GROUP BY "
                          + "SUBJECT_ID  ORDER BY SUBJECT_ID) a "
                          + "LEFT JOIN " + DBTablesName.T_SUBJECT 
                          +" b ON a.SUBJECT_ID = b.SUBJECT_NAME where b.parent_id='0') t," + DBTablesName.T_FEE 
                          + " b where t.subject_id=b.subject_id and b.period=" + (CommonInfo.当前期-1);
            bool flag = db.Excute(sql);
            if (flag && CommonInfo.当前期 != 12)
            {
                CommonInfo.当前期++;
            }
            return flag;
        }
    }
}
