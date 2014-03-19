using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Helper.DataDefind
{
    class DBTablesName
    {
        private static readonly string t_BOOKS = "T_BOOKS";
        private static readonly string t_VOUCHER = "T_VOUCHER_";
        private static readonly string t_VOUCHER_DETAIL = "T_VOUCHER_DETAIL_";
        private static readonly string t_SUBJECT = "T_SUBJECT_";
        private static readonly string t_SUBJECT_TYPE = "T_SUBJECT_TYPE";
        private static readonly string t_USER = "T_USER";
        private static readonly string t_RECORD = "T_RECORD_";
        
        #region GETSET
        public static string T_BOOKS
        {
            get { return DBTablesName.t_BOOKS; }
        } 

        public static string T_VOUCHER
        {
            get { return DBTablesName.t_VOUCHER + CommonInfo.账薄号; }
        }

        public static string T_VOUCHER_DETAIL
        {
            get { return DBTablesName.t_VOUCHER_DETAIL + CommonInfo.账薄号; }
        }

        public static string T_SUBJECT
        {
            get { return DBTablesName.t_SUBJECT + CommonInfo.制度索引; }
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
            get { return DBTablesName.t_RECORD + CommonInfo.账薄号; }
        } 
        #endregion
    }
}
