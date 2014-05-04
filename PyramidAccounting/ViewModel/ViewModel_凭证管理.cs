using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PA.Model.DataGrid;
using PA.Helper.DataDefind;
using PA.Helper.DataBase;

namespace PA.ViewModel
{
    class ViewModel_凭证管理
    {
        private string DateFormat = "yyyy-MM-dd";
        private DataBase db = new DataBase();
        /// <summary>
        /// 凭证管理
        /// </summary>
        /// <param name="whereParm"></param>
        /// <returns></returns>
        public List<Model_凭证管理> GetData(string whereParm)
        {
            List<Model_凭证管理> datas = new List<Model_凭证管理>();
            Guid LastID = Guid.Empty;
            Model_凭证管理 LastData = new Model_凭证管理();
            string sql = "SELECT "
                            + "voucher.ID,"
                            + "detail.VOUCHER_NO,"
                            + "voucher.OP_TIME,"
                            + "detail.ABSTRACT,"
                            + "detail.SUBJECT_ID,"//科目编号
                            //+ "detail.DETAIL,"//子细目编号
                            + "subject.subject_name,"//科目名
                            + "detail.DEBIT,"
                            + "detail.CREDIT,"
                            + "voucher.PERIOD,"
                            + "voucher.REVIEW_MARK"
                        + " FROM "
                            + DBTablesName.T_VOUCHER + " voucher,"
                            + DBTablesName.T_VOUCHER_DETAIL + " detail "
                            + " LEFT JOIN " + DBTablesName.T_SUBJECT + " subject ON detail.SUBJECT_ID = subject.subject_id"
                        + " WHERE "
                            + "voucher.ID = detail.PARENTID and DELETE_MARK=0 " + whereParm
                        + " ORDER BY "
                            + "voucher.PERIOD,voucher.OP_TIME,detail.VOUCHER_NO";
            DataSet ds = db.Query(sql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (LastID != Guid.Parse(dr[0].ToString())) //新凭证
                {
                    if (LastID != Guid.Empty)
                    {
                        datas.Add(LastData);
                    }
                    LastID = Guid.Parse(dr[0].ToString());
                    Model_凭证管理 data = new Model_凭证管理();
                    LastData = data;
                    LastData.ID = Guid.Parse( dr[0].ToString()) ;
                    LastData.凭证号 = dr[1].ToString();
                    LastData.制表时间 = Convert.ToDateTime(dr[2]).ToString(DateFormat);
                    LastData.摘要 = dr[3].ToString();
                    LastData.科目编号 = dr[4].ToString();
                    LastData.科目名称 = dr[5].ToString();
                    LastData.借方金额 = (dr[6].ToString() == "0") ? "" : dr[6].ToString();
                    LastData.贷方金额 = (dr[7].ToString() == "0") ? "" : dr[7].ToString();
                    LastData.当前期数 = dr[8].ToString();
                    LastData.审核状态 = (dr[9].ToString()=="0")?"未审核":"已审核";
                }
                else //旧凭证
                {
                    LastData.凭证号 += "\n" + dr[1].ToString();
                    LastData.摘要 += "\n" + dr[3].ToString();
                    LastData.科目编号 += "\n" + dr[4].ToString();
                    LastData.科目名称 += "\n" + dr[5].ToString();
                    LastData.借方金额 += "\n" + ((dr[6].ToString() == "0") ? "" : dr[6].ToString());
                    LastData.贷方金额 += "\n" + ((dr[7].ToString() == "0") ? "" : dr[7].ToString());
                }
            }
            if (ds.Tables[0].Rows.Count != 0)
            {
                datas.Add(LastData);
            }
            return datas;
        }

        public bool InsertData(Model_凭证单 Voucher, List<Model_凭证明细> VoucherDetails)
        {
            bool isEmpty = true;
            decimal CountBorrow = 0m;
            decimal CountLoan = 0m;
            List<Model_凭证明细> NewVoucherDetails = new List<Model_凭证明细>();
            foreach (Model_凭证明细 VoucherDetail in VoucherDetails)
            {
                if (VoucherDetail.科目编号 != null)
                {
                    NewVoucherDetails.Add(VoucherDetail);
                    CountBorrow += VoucherDetail.借方;
                    CountLoan += VoucherDetail.贷方;
                    isEmpty = false;
                }
            }
            if (!isEmpty)
            {
                if (CountBorrow != CountLoan)
                {
                    return false;
                }
                List<Model_凭证单> Vouchers = new List<Model_凭证单>();
                Vouchers.Add(Voucher);
                bool result = new PA.Helper.DataBase.DataBase().InsertVoucherAll(Vouchers.OfType<object>().ToList(), NewVoucherDetails.OfType<object>().ToList());
                if(!result)
                {
                    return false;
                }
            }
            return !isEmpty;
        }

        public void Review(Guid id)
        {
            string sql = "update " + DBTablesName.T_VOUCHER + " set review_mark=1,REVIEWER='" + CommonInfo.真实姓名 + "' where id='" + id + "'";
            List<string> lists = new List<string>();
            lists.Add(sql);
            db.BatchOperate(lists);
        }

        public void UnReview(Guid id)
        {
            string sql = "update " + DBTablesName.T_VOUCHER + " set review_mark=0,REVIEWER='" + CommonInfo.真实姓名 + "' where id='" + id + "'";
            List<string> lists = new List<string>();
            lists.Add(sql);
            db.BatchOperate(lists);
        }

        public void Delete(Guid id)
        {
            string sql = "update " + DBTablesName.T_VOUCHER + " set DELETE_MARK=-1 where id='" + id + "'";
            List<string> lists = new List<string>();
            lists.Add(sql);
            db.BatchOperate(lists);
        }
        public void DeleteAsModify(Guid id)
        {
            string sql2 = "Delete from " + DBTablesName.T_VOUCHER_DETAIL + " where parentid='" + id + "'";
            string sql1 = "Delete from " + DBTablesName.T_VOUCHER + " where id='" + id + "'";
            List<string> lists = new List<string>();
            lists.Add(sql2);
            lists.Add(sql1);
            db.BatchOperate(lists);
        }
        /// <summary>
        /// 用于结账前判断是否存在未审核的凭证单
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public bool IsReview(int period)
        {
            string sql = "select 1 from " + DBTablesName.T_VOUCHER 
                + " where REVIEW_MARK=0 and delete_mark=0 and period=" + period + "";
            DataTable dt = db.Query(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public Model_凭证单 GetVoucher(Guid guid)
        {
            Model_凭证单 Voucher = new Model_凭证单();
            string sql = "select * from " + DBTablesName.T_VOUCHER + " where id='" + guid + "'";
            DataSet ds = new PA.Helper.DataBase.DataBase().Query(sql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Voucher.ID = dr[0].ToString();
                Voucher.当前期 = int.Parse(dr[1].ToString());
                Voucher.制表时间 = Convert.ToDateTime(dr[2]);
                Voucher.附属单证数 = int.Parse(dr[3].ToString());
                Voucher.合计借方金额 = decimal.Parse(dr[4].ToString());
                Voucher.合计贷方金额 = decimal.Parse(dr[5].ToString());
                Voucher.会计主管 = dr[6].ToString();
                Voucher.制单人 = dr[7].ToString();
                Voucher.复核 = dr[8].ToString();
                Voucher.审核标志 = int.Parse(dr[9].ToString());
                Voucher.删除标志 = int.Parse(dr[10].ToString());
            }
            return Voucher;
        }
        public List<Model_凭证明细> GetVoucherDetails(Guid guid)
        {
            string LastVoucherNum = "";
            int CountNum = 0;
            bool isFirstLine = true;
            List<Model_凭证明细> VoucherDetails = new List<Model_凭证明细>();
            Model_凭证明细 detail;
            string sql = "select detail.*, subjectA.subject_name as MainSubjectName, subjectB.subject_name as TimesSubjectName"
                + " from "
                    + DBTablesName.T_VOUCHER_DETAIL + " detail"
                    + " LEFT JOIN " + DBTablesName.T_SUBJECT + " subjectA ON detail.subject_id=subjectA.subject_id"
                    + " LEFT JOIN " + DBTablesName.T_SUBJECT + " subjectB ON detail.detail=subjectB.subject_id"
                + " where PARENTID='" + guid + "'";
            DataSet ds = new PA.Helper.DataBase.DataBase().Query(sql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (LastVoucherNum == dr[4].ToString() || isFirstLine)
                {
                    detail = new Model_凭证明细();
                    detail.ID = int.Parse(dr[0].ToString());
                    detail.序号 = int.Parse(dr[1].ToString());
                    detail.父节点ID = dr[2].ToString();
                    detail.凭证字 = dr[3].ToString();
                    detail.凭证号 = dr[4].ToString();
                    detail.摘要 = dr[5].ToString();
                    detail.科目编号 = dr[6].ToString();
                    detail.子细目ID = dr[7].ToString();
                    detail.记账 = int.Parse(dr[8].ToString());
                    detail.借方 = decimal.Parse(dr[9].ToString());
                    detail.贷方 = decimal.Parse(dr[10].ToString());
                    detail.主科目名 = dr[6].ToString()+ " " +dr["MainSubjectName"].ToString();
                    detail.子细目 = dr[7].ToString() + " " + dr["TimesSubjectName"].ToString();
                    VoucherDetails.Add(detail);
                    LastVoucherNum = dr[4].ToString();
                    CountNum++;
                    isFirstLine = false;
                }
                else
                {
                    //补全上一页剩余空白行
                    for (int i = CountNum; i < 6; i++)
                    {
                        detail = new Model_凭证明细();
                        detail.序号 = i;
                        VoucherDetails.Add(detail);
                    }
                    CountNum = 0;
                    detail = new Model_凭证明细();
                    detail.ID = int.Parse(dr[0].ToString());
                    detail.序号 = int.Parse(dr[1].ToString());
                    detail.父节点ID = dr[2].ToString();
                    detail.凭证字 = dr[3].ToString();
                    detail.凭证号 = dr[4].ToString();
                    detail.摘要 = dr[5].ToString();
                    detail.科目编号 = dr[6].ToString();
                    detail.子细目ID = dr[7].ToString();
                    detail.记账 = int.Parse(dr[8].ToString());
                    detail.借方 = decimal.Parse(dr[9].ToString());
                    detail.贷方 = decimal.Parse(dr[10].ToString());
                    detail.主科目名 = dr[6].ToString()+ " " +dr["MainSubjectName"].ToString();
                    detail.子细目 = dr[7].ToString() + " " + dr["TimesSubjectName"].ToString();
                    VoucherDetails.Add(detail);
                    LastVoucherNum = dr[4].ToString();
                    CountNum++;
                }
            }
            //补全最后的空白行
            for (int i = CountNum; i < 6; i++)
            {
                detail = new Model_凭证明细();
                detail.序号 = i;
                VoucherDetails.Add(detail);
            }
            return VoucherDetails;
        }
        public int GetPageNum(Guid guid)
        {
            int result = 1;
            string sql = "select count(*) from (SELECT count(*) FROM " + DBTablesName.T_VOUCHER_DETAIL + " WHERE PARENTID = '"+ guid +"' GROUP BY VOUCHER_NO)";
            DataSet ds = new PA.Helper.DataBase.DataBase().Query(sql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                result = int.Parse(dr[0].ToString());
            }
            return result;
        }
    }
}
