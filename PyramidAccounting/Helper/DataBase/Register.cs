using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Helper.DataBase
{
    class Register
    {
        DataBase db = new DataBase();
        public string GetVersionType()
        {
            string sql = "select 7-(julianday(datetime('now','localtime'))-julianday(OP_TIME)),value from t_systeminfo where rkey='999'";
            return db.GetAllData(sql).Split('\t')[0];
        }

        /// <summary>
        /// 更新软件版本状态
        /// </summary>
        /// <returns></returns>
        public bool UpdateSoftwareVersionStatus(int i)
        {
            string sql = "update t_systeminfo set value='" + i + "' where rkey = '999'";
            return true;
        }
    }
}
