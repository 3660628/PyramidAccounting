using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PA.Model.DataGrid;
using PA.Helper.DataDefind;

namespace PA.ViewModel
{
    class ViewModel_凭证管理
    {
        private string DateFormat = "yyyy-MM-dd";
        private Guid  LastID = Guid.Empty;
        private Model_凭证管理 LastData = new Model_凭证管理();

        /// <summary>
        /// 凭证管理
        /// </summary>
        /// <param name="whereParm"></param>
        /// <returns></returns>
        public List<Model_凭证管理> GetData(string whereParm)
        {
            List<Model_凭证管理> datas = new List<Model_凭证管理>();
            string sql = "SELECT "
                            + "voucher.ID,"
                            + "detail.VOUCHER_NO,"
                            + "voucher.OP_TIME,"
                            + "detail.ABSTRACT,"
                            + "detail.SUBJECT_ID,"
                            + "detail.DETAIL,"
                            + "detail.DEBIT,"
                            + "detail.CREDIT,"
                            + "'当前期数',"
                            + "voucher.REVIEW_MARK"
                        + " FROM "
                            + DBTablesName.T_VOUCHER + " voucher,"
                            + DBTablesName.T_VOUCHER_DETAIL + " detail"
                        + " WHERE "
                            + "voucher.ID = detail.PARENTID and DELETE_MARK=0 " + whereParm
                        + " ORDER BY "
                            + "voucher.OP_TIME";
            DataSet ds = new PA.Helper.DataBase.DataBase().Query(sql);
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
                    LastData.审核状态 = dr[9].ToString();
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

        public void InsertData(Model_凭证单 Voucher, List<Model_凭证明细> VoucherDetails)
        {
            List<Model_凭证单> Vouchers = new List<Model_凭证单>();
            Vouchers.Add(Voucher);
            new PA.Helper.DataBase.DataBase().InsertPackage(DBTablesName.T_VOUCHER, Vouchers.OfType<object>().ToList());
            List<Model_凭证明细> NewVoucherDetails = new List<Model_凭证明细>();
            foreach (Model_凭证明细 VoucherDetail in VoucherDetails)
            {
                if (VoucherDetail.摘要 != "" && VoucherDetail.摘要 != null && VoucherDetail.科目编号 != null)
                {
                    NewVoucherDetails.Add(VoucherDetail);
                }
            }
            new PA.Helper.DataBase.DataBase().InsertPackage(DBTablesName.T_VOUCHER_DETAIL, NewVoucherDetails.OfType<object>().ToList());
        }

        public void Review(Guid id)
        {
            string sql = "update "+DBTablesName.T_VOUCHER+" set review_mark=1 where id='" + id + "'";
            List<string> lists = new List<string>();
            lists.Add(sql);
            new PA.Helper.DataBase.DataBase().BatchOperate(lists);
        }

        public void Delete(Guid id)
        {
            string sql = "update " + DBTablesName.T_VOUCHER + " set DELETE_MARK=-1 where id='" + id + "'";
            List<string> lists = new List<string>();
            lists.Add(sql);
            new PA.Helper.DataBase.DataBase().BatchOperate(lists);
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
            bool StartAdd = false;
            List<Model_凭证明细> VoucherDetails = new List<Model_凭证明细>();
            Model_凭证明细 detail;
            string sql = "select * from " + DBTablesName.T_VOUCHER_DETAIL + " where PARENTID='" + guid + "'";
            DataSet ds = new PA.Helper.DataBase.DataBase().Query(sql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (LastVoucherNum == dr[4].ToString() || !StartAdd)
                {
                    detail = new Model_凭证明细();
                    detail.ID = int.Parse(dr[0].ToString());
                    detail.序号 = int.Parse(dr[1].ToString());
                    detail.父节点ID = dr[2].ToString();
                    detail.凭证字 = dr[3].ToString();
                    detail.凭证号 = dr[4].ToString();
                    detail.摘要 = dr[5].ToString();
                    detail.科目编号 = dr[6].ToString();
                    detail.子细目 = dr[7].ToString();
                    detail.记账 = int.Parse(dr[8].ToString());
                    detail.借方 = decimal.Parse(dr[9].ToString());
                    detail.贷方 = decimal.Parse(dr[10].ToString());
                    VoucherDetails.Add(detail);
                    LastVoucherNum = dr[4].ToString();
                    CountNum++;
                    StartAdd = true;
                }
                else
                {
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
                    detail.子细目 = dr[7].ToString();
                    detail.记账 = int.Parse(dr[8].ToString());
                    detail.借方 = decimal.Parse(dr[9].ToString());
                    detail.贷方 = decimal.Parse(dr[10].ToString());
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
