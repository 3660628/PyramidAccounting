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
            string sql = "SELECT a.SUBJECT_ID,a.fee,b.fee FROM (SELECT SUBJECT_ID,fee FROM " +
                DBTablesName.T_FEE + " WHERE PERIOD = " + index + ") a LEFT JOIN (SELECT SUBJECT_ID,total(fee) AS fee FROM "
                + DBTablesName.T_FEE + " WHERE PERIOD <= " + index + " GROUP BY SUBJECT_ID) b ON a.SUBJECT_ID = b.SUBJECT_ID "
                + "WHERE a.SUBJECT_ID IN (" + temp.Substring(1, temp.Length - 1) + ") ";
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
                    m.累计数 = d[2].ToString();
                }
                if (d[1].ToString().Equals("0"))
                {
                    m.本期数 = "";
                }
                else
                {
                    m.本期数 = d[1].ToString();
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
            string sql = "SELECT substr(c.parent_id,1,5),total(w.fee1),total(w.fee2) FROM (SELECT a.DETAIL,a.fee as fee1,b.fee as fee2 FROM (SELECT a.DETAIL,a.CREDIT - a.DEBIT AS fee FROM "
                + DBTablesName.T_VOUCHER_DETAIL + " a LEFT JOIN "
                + DBTablesName.T_VOUCHER + " b ON a.PARENTID = b.ID WHERE b.REVIEW_MARK = 1 AND b.PERIOD = "
                + index + ") a LEFT JOIN (SELECT a.DETAIL,total(a.CREDIT) - total(a.DEBIT) AS fee FROM " 
                + DBTablesName.T_VOUCHER_DETAIL + " a LEFT JOIN "
                + DBTablesName.T_VOUCHER + " b ON a.PARENTID = b.ID WHERE b.REVIEW_MARK = 1 AND b.PERIOD <= "
                + index + " GROUP BY a.DETAIL) b ON a.DETAIL = b.DETAIL WHERE substr(a.DETAIL,1,5) IN (" + temp.Substring(1,temp.Length-1) + ")) w LEFT JOIN "
                + DBTablesName.T_SUBJECT + " c ON w.detail = c.subject_id GROUP BY substr(c.parent_id,1,5)";
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
        /// 行政费用支出明细表    2014/4/20      a.DEBIT - a.CREDIT  改为 a.CREDIT - a.DEBIT
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
                string _sql3 = "select a.subject_name,a.fee,b.fee from (select subject_name,fee from sbtemp where period="
                + index + ") a left join (select subject_name,sum(fee) as fee from sbtemp where period<=" +
                index + " group by subject_name) b on a.subject_name=b.subject_name";
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

        /// <summary>
        /// 事业费用支出明细表    2014/4/20      a.DEBIT - a.CREDIT  改为 a.CREDIT - a.DEBIT
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<Model_事业报表类> GetExpenditureDetail(int index, int parentID)
        {
            List<string> sqlList = new List<string>();
            List<Model_事业报表类> list = new List<Model_事业报表类>();
            string dropSql = "drop table IF EXISTS expenditure_temp";
            sqlList.Add(dropSql);
            string _sql1 = "create table expenditure_temp(subject_name text,period int,fee,fee1,fee2 decimal)";
            sqlList.Add(_sql1);
            string _sql2 = "insert into expenditure_temp select b.subject_name,a.PERIOD,total(a.fee),total(a.fee1),total(a.fee2) from (select a.PERIOD as PERIOD,a.fee as fee,b.fee as fee1,c.fee as fee2,a.DETAIL from (SELECT a.subject_id,a.DETAIL,b.PERIOD,total(a.DEBIT - a.CREDIT) AS fee FROM "
                    + DBTablesName.T_VOUCHER_DETAIL + " a LEFT JOIN "
                    + DBTablesName.T_VOUCHER + " b ON a.PARENTID = b.ID where b.REVIEW_MARK=1 and a.detail like '" + parentID + "%' GROUP BY a.DETAIL,b.PERIOD ) a left join " +
                    " (SELECT a.subject_id,a.DETAIL,b.PERIOD,total(a.DEBIT - a.CREDIT) AS fee FROM "
                    + DBTablesName.T_VOUCHER_DETAIL + " a LEFT JOIN "
                    + DBTablesName.T_VOUCHER + " b ON a.PARENTID = b.ID where b.REVIEW_MARK=1 and a.detail like '" + parentID + "01%' GROUP BY a.DETAIL,b.PERIOD ) b on a.subject_id = b.subject_id left join (SELECT a.subject_id,a.DETAIL,b.PERIOD,total(a.DEBIT - a.CREDIT) AS fee FROM "
                    + DBTablesName.T_VOUCHER_DETAIL + " a LEFT JOIN "
                    + DBTablesName.T_VOUCHER + " b ON a.PARENTID = b.ID where b.REVIEW_MARK=1 and a.detail like '" + parentID + "02%' GROUP BY a.DETAIL,b.PERIOD ) c on  a.subject_id=c.subject_id ) a LEFT JOIN "
                    + DBTablesName.T_SUBJECT
                    + " b ON a.DETAIL = b.subject_id group by a.PERIOD,b.SUBJECT_NAME";
            sqlList.Add(_sql2);
            bool flag = db.BatchOperate(sqlList);
            if (flag)
            {
                string _sql3 = "select a.subject_name,a.fee,b.fee,a.fee1,b.fee1,a.fee2,b.fee2 from (select subject_name,fee,fee1,fee2 from expenditure_temp where period="
                + index + ") a left join (select subject_name,sum(fee) as fee,sum(fee1) as fee1,sum(fee2) as fee2 from expenditure_temp where period<=" +
                index + " group by subject_name) b on a.subject_name=b.subject_name";
                DataTable dt = db.Query(_sql3).Tables[0];
                foreach (DataRow d in dt.Rows)
                {
                    Model_事业报表类 m = new Model_事业报表类();
                    m.编号 = d[0].ToString();
                    m.本期数 = d[1].ToString();
                    m.累计数 = d[2].ToString();
                    m.本期数1 = d[3].ToString();
                    m.累计数1 = d[4].ToString();
                    m.本期数2 = d[5].ToString();
                    m.累计数2 = d[6].ToString();
                    list.Add(m);
                }
            }
            return list;
        }
    }
}
