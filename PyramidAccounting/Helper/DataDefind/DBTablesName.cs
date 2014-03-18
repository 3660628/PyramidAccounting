using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Helper.DataDefind
{
    class DBTablesName
    {
        //private static string _bookid = CommonInfo.账薄号;
        private static string _bookid = Properties.Settings.Default.BookID;
        private static  string t_BOOKS = "T_BOOKS";
        private static  string t_VOUCHER = "T_VOUCHER_" + _bookid;
        private static  string t_VOUCHER_DETAIL = "T_VOUCHER_DETAILR_" + _bookid;
        private static  string t_SUBJECT = "T_SUBJECTR_" + _bookid;
        private static  string t_SUBJECT_TYPE = "T_SUBJECT_TYPER_" + _bookid;
        private static  string t_USER = "T_USER";
        private static  string t_RECORD = "T_RECORDR_" + _bookid;
        
        #region GETSET
        public static string T_BOOKS
        {
            get { return DBTablesName.t_BOOKS; }
        } 

        public static string T_VOUCHER
        {
            get { return DBTablesName.t_VOUCHER; }
        }

        public static string T_VOUCHER_DETAIL
        {
            get { return DBTablesName.t_VOUCHER_DETAIL; }
        }

        public static string T_SUBJECT
        {
            get { return DBTablesName.t_SUBJECT; }
        }

        public static string T_SUBJECT_TYPE
        {
            get { return DBTablesName.t_SUBJECT_TYPE; }
        }

        public static string T_USER
        {
            get { return DBTablesName.t_USER; }
        }

        public static string T_RECORD
        {
            get { return DBTablesName.t_RECORD; }
        } 
        #endregion
    }
}
