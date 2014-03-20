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
                string sql = "insert into " + DBTablesName.T_YEAR_FEE + "(subject_id,bookid) values('" + m.科目编号 + "','" + bookid + "')";
                sqlList.Add(sql);
            }
            db.BatchOperate(sqlList);
        }

    }
}
