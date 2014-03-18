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

        public List<Model_凭证管理> GetData()
        {
            List<Model_凭证管理> datas = new List<Model_凭证管理>();
            string sql = "SELECT "
                            + "voucher.VOUCHER_NO,"
                            + "voucher.OP_TIME,"
                            + "detail.ABSTRACT,"
                            + "detail.SUBJECT_ID,"
                            + "'科目名称',"
                            + "detail.DEBIT,"
                            + "detail.CREDIT,"
                            + "'当前期数',"
                            + "voucher.REVIEW_MARK"
                        + " FROM "
                            + DBTablesName.T_VOUCHER + ","
                            + DBTablesName.T_VOUCHER_DETAIL
                        + " WHERE "
                            + "voucher.VOUCHER_NO = detail.PARENTID"
                        + " ORDER BY "
                            + "detail.id";
            DataSet ds = new PA.Helper.DataBase.DataBase().Query(sql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (LastVOUCHER_NO != dr[0].ToString()) //新凭证
                {
                    if (LastVOUCHER_NO != "")
                    {
                        datas.Add(LastData);
                    }
                    LastVOUCHER_NO = dr[0].ToString();
                    Model_凭证管理 data = new Model_凭证管理();
                    LastData = data;
                    LastData.凭证号 = dr[0].ToString();
                    LastData.制表时间 = Convert.ToDateTime(dr[1]);
                    LastData.摘要 = dr[2].ToString();
                    LastData.科目编号 = dr[3].ToString();
                    LastData.科目名称 = dr[4].ToString();
                    LastData.借方金额 = dr[5].ToString();
                    LastData.贷方金额 = dr[6].ToString();
                    LastData.当前期数 = dr[7].ToString();
                    LastData.审核状态 = dr[8].ToString();
                }
                else //旧凭证
                {
                    LastData.摘要 += "\n" + dr[2].ToString();
                    LastData.科目编号 += "\n" + dr[3].ToString();
                    LastData.科目名称 += "\n" + dr[4].ToString();
                    LastData.借方金额 += "\n" + dr[5].ToString();
                    LastData.贷方金额 += "\n" + dr[6].ToString();
                }
            }
            datas.Add(LastData);
            return datas;
        }
    }
}
