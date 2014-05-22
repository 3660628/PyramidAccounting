using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.Model.DataGrid;
using System.Data;
using PA.Helper.DataDefind;
using PA.Helper.DataBase;

namespace PA.ViewModel
{
    class ViewModel_ReportManager
    {
        private DataBase db = new DataBase();
        /// <summary>
        /// 获取资产负债表数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<Model_报表类> GetBalanceSheet(int index)
        {
            List<Model_报表类> list = new List<Model_报表类>();
            string sql = "SELECT a.SUBJECT_ID,a.fee,b.fee FROM (SELECT SUBJECT_ID,fee FROM " +
                DBTablesName.T_FEE + " WHERE PERIOD = " + index + ") a LEFT JOIN (SELECT SUBJECT_ID,total(fee) AS fee FROM "
                + DBTablesName.T_FEE + " WHERE PERIOD = 0 GROUP BY	SUBJECT_ID	) b ON a.SUBJECT_ID = b.SUBJECT_ID "
                + "WHERE a.SUBJECT_ID IN (SELECT subject_id FROM " + DBTablesName.T_SUBJECT + " WHERE parent_id = '0') ";
            DataTable dt = db.Query(sql).Tables[0];
            foreach (DataRow d in dt.Rows)
            {
                Model_报表类 m = new Model_报表类();
                m.编号 = d[0].ToString();
                m.年初数 = d[2].ToString();
                m.期末数 = d[1].ToString();
                if (m.年初数.Equals("0"))
                {
                    m.年初数 = "";
                }
                if (m.期末数.Equals("0"))
                {
                    m.期末数 = "";
                }
                list.Add(m);
            }
            return list;
        }
        /// <summary>
        /// 获取收入支出报表数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<Model_报表类> GetIncomeAndExpenses(int index, List<string> subList)
        {
            string temp = string.Empty;
            foreach (string s in subList)
            {
                temp += ",'" + s + "'";
            }
            List<Model_报表类> list = new List<Model_报表类>();
            string sql = "SELECT SUBJECT_ID,debit,credit,fee from " + DBTablesName.T_FEE
                + " WHERE period=" + index + " and SUBJECT_ID IN (" + temp.Substring(1, temp.Length - 1) + ") ";
            DataTable dt = db.Query(sql).Tables[0];
            foreach (DataRow d in dt.Rows)
            {
                Model_报表类 m = new Model_报表类();
                m.编号 = d[0].ToString();
                if (d[3].ToString().Equals("0"))
                {
                    m.累计数 = "";
                }
                else
                {
                    m.累计数 = d[3].ToString();
                }
                decimal d1 = 0m;
                decimal d2 = 0m;
                decimal.TryParse(d[1].ToString(), out d1);
                decimal.TryParse(d[2].ToString(), out d2);
                m.本期数 = (d1 - d2).ToString().Replace("-", "");
                if (m.本期数.Equals("0"))
                {
                    m.本期数 = "";
                }
                list.Add(m);
            }
            return list;
        }

        /// <summary>
        /// 获取收入支出报表数据 - 二级科目数据,20140512 改成支持4级
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<Model_报表类> GetIncomeAndExpensesForTwoSubject(int index,List<string> subList)
        {
            string temp = string.Empty;
            foreach(string s in subList)
            {
                temp += ",'" + s + "'";
            }
            List<Model_报表类> list = new List<Model_报表类>();
            string sql = "SELECT a.DETAIL,a.fee as fee1,b.fee as fee2 FROM (SELECT substr(a.DETAIL,1,5) as detail,total(a.CREDIT) - total(a.DEBIT) AS fee FROM "
                + DBTablesName.T_VOUCHER_DETAIL + " a LEFT JOIN "
                + DBTablesName.T_VOUCHER + " b ON a.PARENTID = b.ID WHERE b.REVIEW_MARK = 1 AND b.PERIOD = "
                + index + " GROUP BY substr(a.DETAIL, 1, 5)) a LEFT JOIN (SELECT substr(a.DETAIL,1,5) as detail,total(a.CREDIT) - total(a.DEBIT) AS fee FROM " 
                + DBTablesName.T_VOUCHER_DETAIL + " a LEFT JOIN "
                + DBTablesName.T_VOUCHER + " b ON a.PARENTID = b.ID WHERE b.REVIEW_MARK = 1 AND b.PERIOD <= "
                + index + " GROUP BY substr(a.DETAIL,1,5)) b ON a.DETAIL = b.DETAIL WHERE substr(a.DETAIL,1,5) IN (" 
                + temp.Substring(1, temp.Length - 1) + ")";
            DataTable dt = db.Query(sql).Tables[0];
            foreach (DataRow d in dt.Rows)
            {
                Model_报表类 m = new Model_报表类();
                m.编号 = d[0].ToString();
                if (d[2].ToString().Equals("0"))
                {
                    m.累计数 = "";
                }
                else
                {
                    m.累计数 = d[2].ToString().Replace("-","");
                }
                if (d[1].ToString().Equals("0"))
                {
                    m.本期数 = "";
                }
                else
                {
                    m.本期数 = d[1].ToString().Replace("-", "");
                }
                list.Add(m);
            }
            return list;
        }
        /// <summary>
        /// 行政费用支出明细表 事业及事业支出明细表    2014/4/20     
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<Model_报表类> GetAdministrativeExpenseDetail(int index,int parentID)
        {
            List<string> sqlList = new List<string>();
            List<Model_报表类> list = new List<Model_报表类>();
            string dropSql = "drop table IF EXISTS sbtemp";
            sqlList.Add(dropSql);
            string _sql1 = "create table sbtemp(subject_name text,period int,fee decimal)";
            sqlList.Add(_sql1);
            string _sql2 = "insert into sbtemp select b.subject_name,a.PERIOD,total(a.fee) from (SELECT a.DETAIL,b.PERIOD,total(a.DEBIT - a.CREDIT) AS fee FROM "
                    + DBTablesName.T_VOUCHER_DETAIL + " a LEFT JOIN "
                    + DBTablesName.T_VOUCHER + " b ON a.PARENTID = b.ID where b.REVIEW_MARK=1 and a.detail like '" + parentID + "%' GROUP BY a.DETAIL,b.PERIOD ) a LEFT JOIN "
                    + DBTablesName.T_SUBJECT 
                    + " b ON a.DETAIL = b.subject_id group by a.PERIOD,b.SUBJECT_NAME";
            sqlList.Add(_sql2);
            bool flag = db.BatchOperate(sqlList);
            if ( flag )
            {
                string _sql3 = "select b.subject_name,a.fee,b.fee from (select subject_name,sum(fee) as fee from sbtemp where period<=" +
                index + " group by subject_name) b left join  (select subject_name,fee from sbtemp where period="
                + index + ") a on a.subject_name=b.subject_name";
                DataTable dt = db.Query(_sql3).Tables[0];
                foreach (DataRow d in dt.Rows)
                {
                    Model_报表类 m = new Model_报表类();
                    m.编号 = d[0].ToString();
                    m.本期数 = d[1].ToString();
                    m.累计数 = d[2].ToString();
                    list.Add(m);
                }
            }
            return list;
        }

    }
}
