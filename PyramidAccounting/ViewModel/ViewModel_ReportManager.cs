﻿using System;
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
                + DBTablesName.T_FEE + " WHERE PERIOD <= " + index + " GROUP BY SUBJECT_ID) b ON a.SUBJECT_ID = b.SUBJECT_ID "
                + "WHERE a.SUBJECT_ID IN ('401','404','407','501','502','505') ";
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
        /// 获取收入支出报表数据 - 二级科目数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<Model_报表类> GetIncomeAndExpensesForTwoSubject(int index)
        {
            List<Model_报表类> list = new List<Model_报表类>();
            string sql = "SELECT c.parent_id,total(w.fee1),total(w.fee2) FROM (SELECT a.DETAIL,a.fee as fee1,b.fee as fee2 FROM (SELECT a.DETAIL,a.DEBIT - a.CREDIT AS fee FROM "
                + DBTablesName.T_VOUCHER_DETAIL + " a LEFT JOIN "
                + DBTablesName.T_VOUCHER + " b ON a.PARENTID = b.ID WHERE b.REVIEW_MARK = 1 AND b.PERIOD = " 
                + index + ") a LEFT JOIN (SELECT a.DETAIL,total(a.DEBIT) - total(a.CREDIT) AS fee FROM " 
                + DBTablesName.T_VOUCHER_DETAIL + " a LEFT JOIN T_VOUCHER_20140420164636 b ON a.PARENTID = b.ID WHERE b.REVIEW_MARK = 1 AND b.PERIOD <= "
                + index + " GROUP BY a.DETAIL) b ON a.DETAIL = b.DETAIL WHERE a.DETAIL IN (SELECT t.SUBJECT_ID FROM " 
                + DBTablesName.T_SUBJECT + " t WHERE t.PARENT_ID IN ('40101','40102','40401','40402'))) w LEFT JOIN T_SUBJECT_0 c ON w.detail = c.subject_id GROUP BY c.PARENT_ID";
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
        /// 行政费用支出明细表    2014/4/20      a.DEBIT - a.CREDIT  改为 a.CREDIT - a.DEBIT
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<Model_报表类> GetAdministrativeExpenseDetail(int index)
        {
            List<string> sqlList = new List<string>();
            List<Model_报表类> list = new List<Model_报表类>();
            string dropSql = "drop table IF EXISTS sbtemp";
            sqlList.Add(dropSql);
            string _sql1 = "create table sbtemp(detail text,period int,fee decimal)";
            sqlList.Add(_sql1);
            string _sql2 = "insert into sbtemp SELECT a.DETAIL,b.PERIOD,total(a.CREDIT - a.DEBIT) AS fee FROM "
                    + DBTablesName.T_VOUCHER_DETAIL + " a LEFT JOIN "
                    + DBTablesName.T_VOUCHER + " b ON a.PARENTID = b.ID where b.REVIEW_MARK=1 and a.detail like '501%' GROUP BY a.DETAIL,b.PERIOD";
            sqlList.Add(_sql2);
            bool flag = db.BatchOperate(sqlList);
            if ( flag )
            {
                string _sql3 = "select a.detail,a.fee,b.fee from (select detail,fee from sbtemp where period="
                + index + ") a left join (select detail,sum(fee) as fee from sbtemp where period<=" +
                index + " group by detail) b on a.detail=b.detail "
                + "where a.detail not in (select subject_id from " + DBTablesName.T_SUBJECT
                + " where parent_id='501') order by a.detail ";
                DataTable dt = db.Query(_sql3).Tables[0];
                foreach (DataRow d in dt.Rows)
                {
                    Model_报表类 m = new Model_报表类();
                    m.编号 = d[0].ToString();
                    m.本期数 = d[2].ToString();
                    m.累计数 = d[1].ToString();
                    list.Add(m);
                }
            }
            return list;
       
        }
    }
}
