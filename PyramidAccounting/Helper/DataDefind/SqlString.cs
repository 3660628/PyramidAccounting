using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Helper.DataDefind
{
    class SqlString
    {
        private static readonly string update_Sql = "Update @tableName set @key = @value where @whereParm";
        private static readonly string insert_T_VOUCHER = "Insert into T_VOUCHER() values()";

        

        private static readonly string insert_T_VOUCHER_DETAIL = "Insert into T_VOUCHER_DETAIL(VID, PARENTID, ABSTRACT, SUBJECT_ID, DETAIL, BOOKKEEP_MARK, DEBIT, CREDIT, BOOK_ID) "
            + "values(@VID, @PARENTID, @ABSTRACT, @SUBJECT_ID, @DETAIL, @BOOKKEEP_MARK, @DEBIT, @CREDIT, @BOOK_ID)";
        

        public static string Update_Sql
        {
            get { return SqlString.update_Sql; }
        } 

        public static string Insert_T_VOUCHER_DETAIL
        {
            get { return SqlString.insert_T_VOUCHER_DETAIL; }
        }

        public static string Insert_T_VOUCHER
        {
            get { return SqlString.insert_T_VOUCHER; }
        } 


    }
}
