using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.Model.DataGrid;

namespace PA.ViewModel
{
    class ViewModel_记账凭证
    {
        public void GetData()
        {

        }

        public void InsertData(Model_凭证单 Voucher, List<Model_凭证明细> VoucherDetails)
        {
            new PA.Helper.DataBase.DataBase().InsertPackage("T_VOUCHER_DETAIL", VoucherDetails.OfType<object>().ToList());
        }

        public void UpdateData()
        {
            List<PA.Model.Database.UpdateParm> lists = new List<Model.Database.UpdateParm>();
            PA.Model.Database.UpdateParm parm = new Model.Database.UpdateParm();
            parm.TableName = "T_VOUCHER_DETAIL";
            parm.Key = "PARENTID";
            parm.Value = "'asdasd'";
            parm.WhereParm = "vid=1";
            lists.Add(parm);
            new PA.Helper.DataBase.DataBase().UpdatePackage(lists);
        }
    }
}
