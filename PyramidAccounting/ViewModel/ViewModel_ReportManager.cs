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
        public List<Model_资产负债表> GetData(int index)
        {
            string sql = "SELECT a.SUBJECT_ID,a.fee,b.fee FROM (SELECT	SUBJECT_ID,fee FROM " +
                DBTablesName.T_FEE + " WHERE PERIOD = " + index + ") a	 LEFT JOIN (SELECT SUBJECT_ID,total(fee) AS fee	FROM "
                + DBTablesName.T_FEE + " WHERE PERIOD <= " + index + " GROUP BY	SUBJECT_ID	) b ON a.SUBJECT_ID = b.SUBJECT_ID	"
                + "WHERE a.SUBJECT_ID IN (SELECT subject_id FROM " + DBTablesName.T_SUBJECT + " WHERE parent_id = '0') ";		
            return null;
        }
    }
}
