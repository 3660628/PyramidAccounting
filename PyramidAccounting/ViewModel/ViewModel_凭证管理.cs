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
        private string  LastVOUCHER_NO = "";
        private Model_凭证管理 LastData = new Model_凭证管理();

        public List<Model_凭证管理> GetData(string whereParm)
        {
            List<Model_凭证管理> datas = new List<Model_凭证管理>();
            string sql = "SELECT "
                            + "voucher.ID,"
                            + "voucher.VOUCHER_NO,"
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
                            + "voucher.VOUCHER_NO = detail.PARENTID " + whereParm
                        + " ORDER BY "
                            + "detail.id";
            DataSet ds = new PA.Helper.DataBase.DataBase().Query(sql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (LastVOUCHER_NO != dr[1].ToString()) //新凭证
                {
                    if (LastVOUCHER_NO != "")
                    {
                        datas.Add(LastData);
                    }
                    LastVOUCHER_NO = dr[1].ToString();
                    Model_凭证管理 data = new Model_凭证管理();
                    LastData = data;
                    LastData.ID = int.Parse(dr[0].ToString());
                    LastData.凭证号 = dr[1].ToString();
                    LastData.制表时间 = Convert.ToDateTime(dr[2]).ToString(DateFormat);
                    LastData.摘要 = dr[3].ToString();
                    LastData.科目编号 = dr[4].ToString() + "-" + dr[5].ToString();
                    LastData.科目名称 = dr[5].ToString();
                    LastData.借方金额 = (dr[6].ToString() == "0") ? "" : dr[5].ToString();
                    LastData.贷方金额 = (dr[7].ToString() == "0") ? "" : dr[6].ToString();
                    LastData.当前期数 = dr[8].ToString();
                    LastData.审核状态 = dr[9].ToString();
                }
                else //旧凭证
                {
                    LastData.摘要 += "\n" + dr[2].ToString();
                    LastData.科目编号 += "\n" + dr[3].ToString() + "-" + dr[4].ToString();
                    LastData.科目名称 += "\n" + dr[4].ToString();
                    LastData.借方金额 += "\n" + ((dr[5].ToString() == "0") ? "" : dr[5].ToString());
                    LastData.贷方金额 += "\n" + ((dr[6].ToString() == "0") ? "" : dr[6].ToString());
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

        public void Review(int id)
        {
            string sql = "update "+DBTablesName.T_VOUCHER+" set review_mark=1 where id="+id;
            List<string> asd = new List<string>();
            asd.Add(sql);
            new PA.Helper.DataBase.DataBase().BatchOperate(asd);
        }



    }
}
