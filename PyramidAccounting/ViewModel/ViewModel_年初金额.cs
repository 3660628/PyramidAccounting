using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.Model.DataGrid;
using PA.ViewModel;
using PA.Helper.DataBase;
using PA.Helper.DataDefind;

namespace PA.ViewModel
{
    class ViewModel_年初金额
    {
        private ViewModel_科目管理 vm = new ViewModel_科目管理();
        private DataBase db = new DataBase();
        /// <summary>
        /// 执行插入语句
        /// </summary>
        /// <param name="bookid"></param>
        public void Insert(string bookid)
        {
            List<Model_科目管理> list = new List<Model_科目管理>();
            list = vm.GetAllSubject();
            List<string> sqlList = new List<string>();
            foreach (Model_科目管理 m in list)
            {
                string sql = "insert into " + DBTablesName.T_YEAR_FEE + "(subject_id,fee,bookid,parentid) values('" + m.科目编号 + "','0','" + bookid + "','0')";
                sqlList.Add(sql);
            }
            db.BatchOperate(sqlList);
        }

        public bool Update()
        {
            string sql = "insert into " + DBTablesName.T_FEE
                + "(period,subject_id,comments,fee,mark) select 0,a.subject_id,'承上年结余',a.fee,b.Borrow_Mark from " 
                + DBTablesName.T_YEAR_FEE + " a left join "
                +  DBTablesName.T_SUBJECT +  " b on a.subject_id=b.subject_id where a.parentid = '0' and a.bookid='" 
                + CommonInfo.账薄号 + "' order by a.subject_id";
            return db.Excute(sql);
        }

        public bool IsSaved()
        {
            bool flag = false;
            string sql = "select total(fee) from " + DBTablesName.T_YEAR_FEE 
                + " where bookid='" + CommonInfo.账薄号 + "'";
            string str = db.GetAllData(sql).Split('\t')[0].Split(',')[0];
            if (str.Equals("0"))
            {
                return false;
            }
            else
            {
                flag = true;
            }
            return flag;
        }
    }
}
