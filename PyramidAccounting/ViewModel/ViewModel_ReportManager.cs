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
        public List<Model_报表类> GetIncomeAndExpenses(int index)
        {
            List<Model_报表类> list = new List<Model_报表类>();
            string sql = "SELECT a.SUBJECT_ID,a.fee,b.fee FROM (SELECT SUBJECT_ID,fee FROM " +
                DBTablesName.T_FEE + " WHERE PERIOD = " + index + ") a LEFT JOIN (SELECT SUBJECT_ID,total(fee) AS fee FROM "
                + DBTablesName.T_FEE + " WHERE PERIOD = 0 GROUP BY	SUBJECT_ID	) b ON a.SUBJECT_ID = b.SUBJECT_ID "
                + "WHERE a.SUBJECT_ID IN ('401','404','407','501','502','505','303') ";
            DataTable dt = db.Query(sql).Tables[0];
            foreach (DataRow d in dt.Rows)
            {
                Model_报表类 m = new Model_报表类();
                m.年初数 = d[2].ToString();
                m.期末数 = d[1].ToString();
                list.Add(m);
            }
            return list;
        }
        public List<Model_报表类> GetAdministrativeExpenseDetail(int index)
        {
            List<string> sqlList = new List<string>();
            List<Model_报表类> list = new List<Model_报表类>();
            string _sql1 = "create temporary table sbtemp(detail text,period int,subject_id text,fee decimal)";
            sqlList.Add(_sql1);
            string _sql2 = "insert into sbtemp SELECT a.DETAIL,b.PERIOD,total(a.DEBIT - a.CREDIT) AS fee FROM "
                    + DBTablesName.T_VOUCHER_DETAIL + " a LEFT JOIN "
                    + DBTablesName.T_VOUCHER + " b ON a.PARENTID = b.ID GROUP BY a.DETAIL,b.PERIOD where a.detail like '501%'";
            sqlList.Add(_sql2);
            bool flag = db.BatchOperate(sqlList);
            if ( flag )
            {
                string _sql3 = "select a.subject_id,a.fee,b.fee from(select subject_id,fee from sbtemp where period="
                + index + ") a,(select subject_id,sum(fee) as fee from sbtemp where period<=" + 
                index + " group by subject_id) b where a.subject_id=b.subject_id "
                + "and a.subject_id not in (select subject_id from " + DBTablesName.T_SUBJECT 
                + " where parent_id='501') order by a.subject_id ";
                DataTable dt = db.Query(_sql3).Tables[0];
                foreach (DataRow d in dt.Rows)
                {
                    Model_报表类 m = new Model_报表类();
                    m.年初数 = d[2].ToString();
                    m.期末数 = d[1].ToString();
                    list.Add(m);
                }
            }
            return list;
       
        }
    }
}
