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

        public void Update(List<Model_科目管理> list)
        {
            List<string> sqlList = new List<string>();
            foreach (Model_科目管理 m in list)
            {
                Model_年初金额 mm = new Model_年初金额();

                //映射相等
                mm.科目编号 = m.科目编号;
                mm.年初金额 = m.年初金额;

                string sql = "update " + DBTablesName.T_YEAR_FEE + " set fee='"
                    + mm.年初金额 + "' where parentid=0 and bookid='" 
                    + CommonInfo.账薄号 + "' and subject_id='" + mm.科目编号 + "'";
                sqlList.Add(sql);
            }
            db.BatchOperate(sqlList);
        }

        public bool IsSaved()
        {
            bool flag = false;
            string sql = "select sum(fee) from " + DBTablesName.T_YEAR_FEE 
                + " where bookid='" + CommonInfo.账薄号 + "'";
            string str = db.GetAllData(sql).Split('\t')[0];
            if (str.Equals(","))
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
