using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Helper.DataDefind
{
    class SqlString
    {
        private static readonly string t_VOUCHER = "Insert into T_VOUCHER() values()";
        private static readonly string t_VOUCHER_DETAIL = "Insert into T_VOUCHER_DETAIL(VID, PARENTID, ABSTRACT, SUBJECT_ID, DETAIL, BOOKKEEP_MARK, DEBIT, CREDIT, BOOK_ID) "
            + "values(@VID, @PARENTID, @ABSTRACT, @SUBJECT_ID, @DETAIL, @BOOKKEEP_MARK, @DEBIT, @CREDIT, @BOOK_ID)";

        public static string T_VOUCHER
        {
            get { return SqlString.t_VOUCHER; }
        } 
        public static string T_VOUCHER_DETAIL
        {
            get { return SqlString.t_VOUCHER_DETAIL; }
        }


    }
}
