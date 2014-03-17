using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.Model.DataGrid;
using PA.Helper.DataDefind;

namespace PA.ViewModel
{
    class ViewModel_记账凭证
    {
        public void GetData()
        {

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

        public void UpdateData()
        {
            List<PA.Model.Database.UpdateParm> lists = new List<Model.Database.UpdateParm>();
            PA.Model.Database.UpdateParm parm = new Model.Database.UpdateParm();
            parm.TableName = DBTablesName.T_VOUCHER_DETAIL;
            parm.Key = "PARENTID";
            parm.Value = "'asdasd'";
            parm.WhereParm = "vid=1";
            lists.Add(parm);
            new PA.Helper.DataBase.DataBase().UpdatePackage(lists);
        }
    }
}
