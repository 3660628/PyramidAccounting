using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.Model.DataGrid;
using PA.Helper.DataBase;
using PA.Helper.DataDefind;
using System.Data;

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
        /// <summary>
        /// 获取当前用户状态，用于操作日志
        /// </summary>
        /// <returns>操作日志模型</returns>
        public Model_操作日志 GetTOperateLog()
        {
            Model_操作日志 mr = new Model_操作日志();
            mr.用户名 = CommonInfo.用户名;
            mr.姓名 = CommonInfo.真实姓名;
            mr.日期 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return mr;
        }

        public List<Model_操作日志> GetData(string start, string end)
        {
            List<Model_操作日志> list = new List<Model_操作日志>();
            string sql = "select strftime(op_time),username,realname,log from " + DBTablesName.T_RECORD + " where op_time between '" 
                + start + "' and '" + end + "'";
            DataTable dt = new DataTable();
            dt = db.Query(sql).Tables[0];
            int rowid = 1;
            foreach (DataRow d in dt.Rows)
            {
                Model_操作日志 m = new Model_操作日志();
                m.序号 = rowid;
                m.日期 = d[0].ToString();
                m.用户名 = d[1].ToString();
                m.姓名 = d[2].ToString();
                m.日志 = d[3].ToString();
                rowid++;
                list.Add(m);
            }
            return list;
        }
    }
}
