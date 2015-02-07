using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.Model.DataGrid;
using PA.Helper.DataBase;
using PA.Helper.DataDefind;
using System.Data;
using PA.Model.ComboBox;
using PA.Helper.Tools;

namespace PA.ViewModel
{
    class ViewModel_账薄管理
    {
        private DataBase db = new DataBase();
        private Util ut = new Util();
        public List<Model_总账> GetTotalFee(string subject_id)
        {
            return GetTotalFee(subject_id, false);
        }
        public List<Model_总账> GetTotalFee(string subject_id, bool WithoutBalance)
        {
            string WhereParm = "";
            if (WithoutBalance)
            {
                WhereParm = " COMMENTS <> '承上年结余' AND ";
            }
            List<Model_总账> list = new List<Model_总账>();
            string id = subject_id.Split('\t')[0];
            string name = subject_id.Split('\t')[1];
            string sql = "select strftime(op_time),VOUCHER_NUMS,COMMENTS,DEBIT,CREDIT,total(FEE*mark) from "
                + DBTablesName.T_FEE + " where " + WhereParm + " delete_mark=0 and subject_id='" + id + "' group by period order by op_time";

            DataSet ds = new DataSet();
            decimal fee = 0;
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
                    decimal.TryParse(d[5].ToString(), out fee);
                    m.余额 = fee.ToString();
                    m.借或贷 = GetMark(fee);
                    string temp = string.Empty;
                    List<string> _list = new List<string>();

                    _list = ut.Turn(m.贷方金额, 12);
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

                    _list = ut.Turn(m.借方金额, 12);
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
                    
                    _list.Clear();
                    _list = ut.Turn(m.余额, 12);
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
        /// <summary>
        /// 支持1.2.3级科目查询子科目
        /// </summary>
        /// <param name="subject_id"></param>
        /// <param name="detail_id"></param>
        /// <returns></returns>
        public List<Model_费用明细> GetFeeDetail(string subject_id,string detail_id)
        {
            bool flag = true;
            ComboBox_科目 cb = new ComboBox_科目();
            List<string> lst = new List<string>();
            lst = cb.GetChildSubjectList("", detail_id, true);
            string _tempstr = string.Empty;
            foreach(string i in lst)
            {
                _tempstr += ",total(case when t.detail='"
                    + i.Split('\t')[0] 
                    + "' then (t.fee1+t.fee2) else '0' end) as '" 
                    + i.Split('\t')[1]
                    + "'";
            }
            for (int i = lst.Count; i < 18; i++)
            {
                _tempstr += ",0";
            }

            List<Model_费用明细> list = new List<Model_费用明细>();
            string sql = "select strftime(time),number,comments,total(fee1),total(fee2)"
                + _tempstr
                + " from " 
                + "(select b.op_time as time ,a.voucher_no as number,a.abstract as comments,a.debit as fee1,a.credit as fee2,a.detail as detail from "
                + DBTablesName.T_VOUCHER_DETAIL
                + " a left join "
                + DBTablesName.T_VOUCHER
                + " b on a.parentid=b.id where a.subject_id='"
                + subject_id
                + "' and a.detail in (select subject_id from "
                + DBTablesName.T_SUBJECT
                + " where parent_id='"
                + detail_id
                + "') and b.delete_mark=0 and b.REVIEW_MARK=1 order by b.op_time)t group by t.time,t.number order by t.time ";

            //查年初数
            string sql2 = "select a.fee*b.borrow_mark from t_yearfee a left join "
                + DBTablesName.T_SUBJECT
                + " b on a.subject_id = b.subject_id where a.subject_id='"
                + subject_id
                + "' and a.bookid='"
                + CommonInfo.账薄号
                + "'";

            string yearfee = db.GetAllData(sql2).Split('\t')[0].Split(',')[0];

            string YearStartFee = yearfee;  //年初值
            String MonthLastValue = "01";

                //月合计
            List<decimal> MonthList = new List<decimal>(20);
            List<decimal> YearList = new List<decimal>(20);
            for (int i = 0; i < 20;i++ )
            {
                decimal childvalue = 0;
                MonthList.Add(childvalue);
                YearList.Add(childvalue);
            }
            //月累计
            

            DataTable dt = db.Query(sql).Tables[0];
            bool isHasData = false;
            if (dt.Rows.Count > 0)
            {

                foreach (DataRow d in dt.Rows)
                {
                    decimal debit = 0;
                    decimal credit = 0;
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

                    if (m.贷方金额.Contains("-") || m.借方金额.Contains("-"))
                    {
                        m.红字标记 = 1;
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                    }

                    decimal.TryParse(m.借方金额, out debit);
                    decimal.TryParse(m.贷方金额, out credit);
                    yearfee = (Convert.ToDecimal(yearfee) - credit + debit).ToString();

                    #region 赋值
                    List<string> _list = new List<string>();

                    _list = ut.Turn(d[3].ToString(), 10, flag);
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

                    _list = ut.Turn(d[4].ToString(), 10, flag);
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

                    _list.Clear();
                    _list = ut.Turn(yearfee, 10, flag);
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

                    _list = ut.Turn(d[5].ToString(), 10, flag);
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
                    _list = ut.Turn(d[6].ToString(), 10, flag);
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
                    _list = ut.Turn(d[7].ToString(), 10, flag);
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
                    _list = ut.Turn(d[8].ToString(), 10, flag);
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
                    _list = ut.Turn(d[9].ToString(), 10, flag);
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
                    _list = ut.Turn(d[10].ToString(), 10, flag);
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
                    _list = ut.Turn(d[11].ToString(), 10, flag);
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
                    _list = ut.Turn(d[12].ToString(), 10, flag);
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
                    _list = ut.Turn(d[13].ToString(), 10, flag);
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
                    _list = ut.Turn(d[14].ToString(), 10, flag);
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
                    _list = ut.Turn(d[15].ToString(), 10, flag);
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

                    _list.Clear();
                    _list = ut.Turn(d[16].ToString(), 10, flag);
                    m.金额141 = _list[0];
                    m.金额142 = _list[1];
                    m.金额143 = _list[2];
                    m.金额144 = _list[3];
                    m.金额145 = _list[4];
                    m.金额146 = _list[5];
                    m.金额147 = _list[6];
                    m.金额148 = _list[7];
                    m.金额149 = _list[8];
                    m.金额150 = _list[9];

                    _list.Clear();
                    _list = ut.Turn(d[17].ToString(), 10, flag);
                    m.金额151 = _list[0];
                    m.金额152 = _list[1];
                    m.金额153 = _list[2];
                    m.金额154 = _list[3];
                    m.金额155 = _list[4];
                    m.金额156 = _list[5];
                    m.金额157 = _list[6];
                    m.金额158 = _list[7];
                    m.金额159 = _list[8];
                    m.金额160 = _list[9];

                    _list.Clear();
                    _list = ut.Turn(d[18].ToString(), 10, flag);
                    m.金额161 = _list[0];
                    m.金额162 = _list[1];
                    m.金额163 = _list[2];
                    m.金额164 = _list[3];
                    m.金额165 = _list[4];
                    m.金额166 = _list[5];
                    m.金额167 = _list[6];
                    m.金额168 = _list[7];
                    m.金额169 = _list[8];
                    m.金额170 = _list[9];

                    _list.Clear();
                    _list = ut.Turn(d[19].ToString(), 10, flag);
                    m.金额171 = _list[0];
                    m.金额172 = _list[1];
                    m.金额173 = _list[2];
                    m.金额174 = _list[3];
                    m.金额175 = _list[4];
                    m.金额176 = _list[5];
                    m.金额177 = _list[6];
                    m.金额178 = _list[7];
                    m.金额179 = _list[8];
                    m.金额180 = _list[9];

                    _list.Clear();
                    _list = ut.Turn(d[20].ToString(), 10, flag);
                    m.金额181 = _list[0];
                    m.金额182 = _list[1];
                    m.金额183 = _list[2];
                    m.金额184 = _list[3];
                    m.金额185 = _list[4];
                    m.金额186 = _list[5];
                    m.金额187 = _list[6];
                    m.金额188 = _list[7];
                    m.金额189 = _list[8];
                    m.金额190 = _list[9];

                    _list.Clear();
                    _list = ut.Turn(d[21].ToString(), 10, flag);
                    m.金额191 = _list[0];
                    m.金额192 = _list[1];
                    m.金额193 = _list[2];
                    m.金额194 = _list[3];
                    m.金额195 = _list[4];
                    m.金额196 = _list[5];
                    m.金额197 = _list[6];
                    m.金额198 = _list[7];
                    m.金额199 = _list[8];
                    m.金额200 = _list[9];

                    _list.Clear();
                    _list = ut.Turn(d[22].ToString(), 10, flag);
                    m.金额201 = _list[0];
                    m.金额202 = _list[1];
                    m.金额203 = _list[2];
                    m.金额204 = _list[3];
                    m.金额205 = _list[4];
                    m.金额206 = _list[5];
                    m.金额207 = _list[6];
                    m.金额208 = _list[7];
                    m.金额209 = _list[8];
                    m.金额210 = _list[9];

                    m.列名 = lst;

                    decimal dValue = 0;
                    if (MonthLastValue.Equals(m.月))
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            decimal.TryParse(d[i + 3].ToString(), out dValue);
                            MonthList[i] += dValue;
                            YearList[i] += dValue;
                        }
                        isHasData = true;  //f1001
                    }
                    else
                    {
                        if (isHasData)
                        {
                            Model_费用明细 mm = new Model_费用明细();
                            mm = GetFeeDetail(MonthList, MonthList[0] - MonthList[1] + decimal.Parse(YearStartFee),flag);
                            mm.摘要 = "本月合计";
                            list.Add(mm);

                            if (!MonthLastValue.Equals("01"))
                            {
                                Model_费用明细 mmm = new Model_费用明细();
                                mmm = GetFeeDetail(YearList, YearList[0] - YearList[1] + decimal.Parse(YearStartFee), flag);
                                mmm.摘要 = "本月累计";
                                list.Add(mmm);
                            }
                        }
                        MonthList = new List<decimal>(20);
                        for (int i = 0; i < 20; i++)
                        {
                            decimal childvalue = 0;
                            MonthList.Add(childvalue);
                        }
                        for (int i = 0; i < 20; i++)
                        {
                            decimal.TryParse(d[i + 3].ToString(), out dValue);
                            MonthList[i] += dValue;
                            YearList[i] += dValue;
                        }
                    }
                    MonthLastValue = m.月;
                    #endregion


                    #endregion
                    list.Add(m);
                }
                Model_费用明细 mlast = new Model_费用明细();
                mlast = GetFeeDetail(MonthList, decimal.Parse(yearfee),flag);
                mlast.摘要 = "本月合计";
                list.Add(mlast);
                if (!MonthLastValue.Equals("01"))
                {
                    Model_费用明细 mmm = new Model_费用明细();
                    mmm = GetFeeDetail(YearList, decimal.Parse(yearfee),flag);
                    if (MonthLastValue.Equals("12"))
                    {
                        mmm.摘要 = "本年结账";
                    }
                    else
                    {
                        mmm.摘要 = "本月累计";
                    }
                    list.Add(mmm);
                }
            }
            return list;
        }

        private Model_费用明细 GetFeeDetail(List<decimal> list,decimal SumFee,bool flag)
        {
            Model_费用明细 m = new Model_费用明细();
            List<string> _list = new List<string>();

            _list = ut.Turn(list[0].ToString(), 10, flag);
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

            _list = ut.Turn(list[1].ToString(), 10, flag);
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

            _list.Clear();
            _list = ut.Turn(SumFee.ToString(), 10, flag);
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

            _list = ut.Turn(list[2].ToString(), 10, flag);
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
            _list = ut.Turn(list[3].ToString(), 10, flag);
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
            _list = ut.Turn(list[4].ToString(), 10, flag);
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
            _list = ut.Turn(list[5].ToString(), 10, flag);
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
            _list = ut.Turn(list[6].ToString(), 10, flag);
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
            _list = ut.Turn(list[7].ToString(), 10, flag);
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
            _list = ut.Turn(list[8].ToString(), 10, flag);
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
            _list = ut.Turn(list[9].ToString(), 10, flag);
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
            _list = ut.Turn(list[10].ToString(), 10, flag);
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
            _list = ut.Turn(list[11].ToString(), 10, flag);
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
            _list = ut.Turn(list[12].ToString(), 10, flag);
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

            _list.Clear();
            _list = ut.Turn(list[13].ToString(), 10, flag);
            m.金额141 = _list[0];
            m.金额142 = _list[1];
            m.金额143 = _list[2];
            m.金额144 = _list[3];
            m.金额145 = _list[4];
            m.金额146 = _list[5];
            m.金额147 = _list[6];
            m.金额148 = _list[7];
            m.金额149 = _list[8];
            m.金额150 = _list[9];

            _list.Clear();
            _list = ut.Turn(list[14].ToString(), 10, flag);
            m.金额151 = _list[0];
            m.金额152 = _list[1];
            m.金额153 = _list[2];
            m.金额154 = _list[3];
            m.金额155 = _list[4];
            m.金额156 = _list[5];
            m.金额157 = _list[6];
            m.金额158 = _list[7];
            m.金额159 = _list[8];
            m.金额160 = _list[9];

            _list.Clear();
            _list = ut.Turn(list[15].ToString(), 10, flag);
            m.金额161 = _list[0];
            m.金额162 = _list[1];
            m.金额163 = _list[2];
            m.金额164 = _list[3];
            m.金额165 = _list[4];
            m.金额166 = _list[5];
            m.金额167 = _list[6];
            m.金额168 = _list[7];
            m.金额169 = _list[8];
            m.金额170 = _list[9];

            _list.Clear();
            _list = ut.Turn(list[16].ToString(), 10, flag);
            m.金额171 = _list[0];
            m.金额172 = _list[1];
            m.金额173 = _list[2];
            m.金额174 = _list[3];
            m.金额175 = _list[4];
            m.金额176 = _list[5];
            m.金额177 = _list[6];
            m.金额178 = _list[7];
            m.金额179 = _list[8];
            m.金额180 = _list[9];

            _list.Clear();
            _list = ut.Turn(list[17].ToString(), 10, flag);
            m.金额181 = _list[0];
            m.金额182 = _list[1];
            m.金额183 = _list[2];
            m.金额184 = _list[3];
            m.金额185 = _list[4];
            m.金额186 = _list[5];
            m.金额187 = _list[6];
            m.金额188 = _list[7];
            m.金额189 = _list[8];
            m.金额190 = _list[9];

            _list.Clear();
            _list = ut.Turn(list[18].ToString(), 10, flag);
            m.金额191 = _list[0];
            m.金额192 = _list[1];
            m.金额193 = _list[2];
            m.金额194 = _list[3];
            m.金额195 = _list[4];
            m.金额196 = _list[5];
            m.金额197 = _list[6];
            m.金额198 = _list[7];
            m.金额199 = _list[8];
            m.金额200 = _list[9];

            _list.Clear();
            _list = ut.Turn(list[19].ToString(), 10, flag);
            m.金额201 = _list[0];
            m.金额202 = _list[1];
            m.金额203 = _list[2];
            m.金额204 = _list[3];
            m.金额205 = _list[4];
            m.金额206 = _list[5];
            m.金额207 = _list[6];
            m.金额208 = _list[7];
            m.金额209 = _list[8];
            m.金额210 = _list[9];
            return m;
        }
        /// <summary>
        /// 科目明细账查询方法
        /// </summary>
        /// <param name="subject_id">一级科目</param>
        /// <param name="detail">二级科目</param>
        /// <reut.Turns></reut.Turns>
        public List<Model_科目明细账> GetSubjectDetail(string subject_id,string detail)
        {
            List<Model_科目明细账> list = new List<Model_科目明细账>();
            List<string> _list = new List<string>();
            bool flag = true;

            subject_id = subject_id.Split('\t')[0];
            detail = detail.Split('\t')[0];
            string sql = "select strftime(b.op_time),a.voucher_no,a.abstract,a.debit,a.credit from " 
                + DBTablesName.T_VOUCHER_DETAIL
                + " a left join " 
                + DBTablesName.T_VOUCHER 
                + " b on a.parentid=b.id where a.subject_id='"
                + subject_id
                + "'" + " and b.delete_mark=0 and b.REVIEW_MARK=1 and  a.detail='"
                + detail + "' order by b.op_time";

            //查年初数
            string sql2 = "select case when b.borrow_mark=1 then '借' else '贷' end,a.fee*b.borrow_mark from " 
                + DBTablesName.T_YEAR_FEE + " a left join " + DBTablesName.T_SUBJECT
                + " b on a.subject_id=b.subject_id where "
                + " a.subject_id='" + detail + "' and a.bookid='" + CommonInfo.账薄号 + "'";
            DataRow dr = db.Query(sql2).Tables[0].Rows[0];

            Model_科目明细账 firstRow = new Model_科目明细账();
            firstRow.摘要 = "承上年结余";
            firstRow.借或贷 = dr[0].ToString();
            firstRow.余额 = dr[1].ToString();
            _list = ut.Turn(firstRow.余额, 12);
            firstRow.余额1 = _list[0];
            firstRow.余额2 = _list[1];
            firstRow.余额3 = _list[2];
            firstRow.余额4 = _list[3];
            firstRow.余额5 = _list[4];
            firstRow.余额6 = _list[5];
            firstRow.余额7 = _list[6];
            firstRow.余额8 = _list[7];
            firstRow.余额9 = _list[8];
            firstRow.余额10 = _list[9];
            firstRow.余额11 = _list[10];
            firstRow.余额12 = _list[11];
            list.Add(firstRow);

            decimal yearfee = 0;
            decimal.TryParse(firstRow.余额,out yearfee);

            DataTable dt = db.Query(sql).Tables[0];

            String MonthLastValue = "01";

            //月合计
            decimal MonthDebit = 0;
            decimal MonthCredit = 0;

            //月累计
            decimal YearDebit = 0;
            decimal YearCredit = 0;

            if (dt.Rows.Count > 0)
            {
                bool isHasData = false;//这个月是否有数据，有才打印合计累计
                foreach (DataRow d in dt.Rows)
                {
                    Model_科目明细账 m = new Model_科目明细账();
                    string date = d[0].ToString().Split(' ')[0];
                    m.年 = date.Split('-')[0];
                    m.月 = date.Split('-')[1];
                    m.日 = date.Split('-')[2];
                    m.号数 = d[1].ToString();
                    m.摘要 = d[2].ToString();
                    m.借方金额 = d[3].ToString();
                    m.贷方金额 = d[4].ToString();

                    if (m.借方金额.Contains("-") || m.贷方金额.Contains("-"))
                    {
                        m.红字标记 = 1;
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                    }
                    yearfee -= Convert.ToDecimal(m.贷方金额) - Convert.ToDecimal(m.借方金额);
                    string tempvalue = yearfee.ToString();
                    _list.Clear();
                    _list = ut.Turn(m.贷方金额, 12);
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

                    _list.Clear();
                    _list = ut.Turn(m.借方金额, 12, flag);
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
                    
                    _list.Clear();
                    _list = ut.Turn(tempvalue, 12);
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

                    
                    decimal dValue = 0;
                    if (MonthLastValue.Equals(m.月))
                    {
                        decimal.TryParse(m.借方金额, out dValue);
                        MonthDebit += dValue;
                        YearDebit += dValue;

                        decimal.TryParse(m.贷方金额, out dValue);
                        MonthCredit += dValue;
                        YearCredit += dValue;
                        isHasData = true;
                    }
                    else
                    {
                        if (isHasData)
                        {
                            Model_科目明细账 mm = new Model_科目明细账();
                            mm = GetModel_Subject(MonthDebit, MonthCredit, MonthDebit - MonthCredit + decimal.Parse(firstRow.余额),flag);
                            mm.摘要 = "本月合计";
                            mm.借或贷 = GetMark(yearfee);
                            list.Add(mm);

                            if (!MonthLastValue.Equals("01"))
                            {
                                Model_科目明细账 mmm = new Model_科目明细账();
                                mmm = GetModel_Subject(YearDebit, YearCredit, MonthDebit - MonthCredit + decimal.Parse(firstRow.余额),flag);
                                mmm.摘要 = "本月累计";
                                mmm.借或贷 = GetMark(yearfee);
                                list.Add(mmm);
                            }
                        }
                        MonthDebit = 0;
                        MonthCredit = 0;

                        decimal.TryParse(m.借方金额, out dValue);
                        MonthDebit += dValue;
                        YearDebit += dValue;

                        decimal.TryParse(m.贷方金额, out dValue);
                        MonthCredit += dValue;
                        YearCredit += dValue;
                    }
                    MonthLastValue = m.月;
                    list.Add(m);
                }
                Model_科目明细账 mlast = new Model_科目明细账();
                mlast = GetModel_Subject(MonthDebit, MonthCredit, yearfee,flag);
                mlast.摘要 = "本月合计";
                mlast.借或贷 = GetMark(yearfee);
                list.Add(mlast);
                if (!MonthLastValue.Equals("01"))
                {
                    Model_科目明细账 mmm = new Model_科目明细账();
                    mmm = GetModel_Subject(YearDebit, YearCredit, yearfee,flag);
                    if (MonthLastValue.Equals("12"))
                    {
                        mmm.摘要 = "本年结账";
                    }
                    else
                    {
                        mmm.摘要 = "本月累计";
                    }
                    mmm.借或贷 = GetMark(yearfee);
                    list.Add(mmm);
                }
            }
            return list;
        }

        private string GetMark(decimal yearfee)
        {
            string mark = string.Empty;
            if (yearfee < 0)
            {
                mark = "贷";
            }
            else if (yearfee == 0)
            {
                mark = "平";
            }
            else
            {
                mark = "借";
            }
            return mark;
        }
        private Model_科目明细账 GetModel_Subject(decimal a,decimal b,decimal c,bool flag)
        {
            Model_科目明细账 m = new Model_科目明细账();
            List<string> _list = new List<string>();
            _list = ut.Turn(a.ToString(), 12, flag);
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
            _list.Clear();
            _list = ut.Turn(b.ToString(), 12, flag);
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

            _list.Clear();
            _list = ut.Turn(c.ToString(), 12);
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

            return m;
        }
       
        /// <summary>
        /// 结账
        /// </summary>
        public bool CheckOut(int peroid)
        {
            string sql = "INSERT INTO " + DBTablesName.T_FEE
                          + "(OP_TIME,PERIOD,SUBJECT_ID,VOUCHER_NUMS,COMMENTS,DEBIT,CREDIT,MARK,DELETE_MARK,FEE) ";
            sql += "select op_time,period,subject_id,voucher_nums,comments,case when debit is null then 0 else debit end, case when credit is null then 0 else credit end,mark,0,"
                + "case when fee is null then 0 else fee end from (select '" + GetLastDay(peroid) + "' as op_time," + peroid
                + " as period,b.subject_id as subject_id,t.voucher_nums as voucher_nums,b.SUBJECT_ID || '汇总' AS comments,t.DEBIT as debit,t.CREDIT as credit,b.mark as mark," 
                + "round(abs(b.mark*b.fee-total(t.credit -t.debit)),2) as fee from "
                + DBTablesName.T_FEE + " b left join ("
                + "SELECT min(VOUCHER_NO) || '-' || max(VOUCHER_NO) AS voucher_nums,subject_id,"
                + "total(DEBIT) AS DEBIT,"
                + "total(CREDIT) AS CREDIT"
                + " FROM "
                + DBTablesName.T_VOUCHER_DETAIL
                + " WHERE PARENTID IN (  "
                + "SELECT ID  FROM   "
                + DBTablesName.T_VOUCHER
                + " WHERE period = " + peroid
                + " and review_mark=1 and delete_mark=0 ) GROUP BY "
                + "SUBJECT_ID  ORDER BY SUBJECT_ID ) t on t.subject_id=b.subject_id where b.period = "
                + (peroid - 1) + " group by b.subject_id) a";          
            bool flag = db.Excute(sql);
            if (flag)
            {
                CommonInfo.当前期++;
            }
            return flag;
        }

        private string GetLastDay(int peroid)
        {
            string date = new ViewModel_Books().GetYear() + "/" + peroid + "/1";
            DateTime dt;
            DateTime.TryParse(date, out dt);
            return dt.AddMonths(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
