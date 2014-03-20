using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.Model.DataGrid;
using PA.Helper.DataBase;

namespace PA.ViewModel
{
    class ViewModel_操作日志
    {
        private string sql = string.Empty;
        private DataBase db = new DataBase();
        /// <summary>
        /// 将用户访问插入到日志表中
        /// </summary>
        /// <param name="m"></param>
        public void Insert(Model_操作日志 m)
        {
            List<Model_操作日志> list = new List<Model_操作日志>();
            list.Add(m);
            db.InsertPackage("t_record", list.OfType<object>().ToList());
        }
    }
}
