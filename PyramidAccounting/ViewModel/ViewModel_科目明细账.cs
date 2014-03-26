using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.Model.DataGrid;
using PA.Helper.DataBase;
using PA.Helper.DataDefind;

namespace PA.ViewModel
{
    class ViewModel_科目明细账
    {
        public List<Model_科目明细账> GetData(string subject_name,string detail)
        {
            List<Model_科目明细账> list = new List<Model_科目明细账>();
            string sql = "select b.op_time,a.voucher_no,a.abstract,a.subject_id,a.detail,a.detail,a.debit,a.credit from " 
                + DBTablesName.T_VOUCHER_DETAIL
                + " a left join " 
                + DBTablesName.T_VOUCHER 
                + " b on a.parentid=b.id where a.subject_id='"
                + subject_name 
                + "'" + " and  a.detail='"
                + detail + "'";
            return list;
        }
    }
}
