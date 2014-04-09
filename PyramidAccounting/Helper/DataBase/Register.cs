using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.Helper.DataDefind;

namespace PA.Helper.DataBase
{
    class Register
    {
        DataBase db = new DataBase();
        public int GetVersionType()
        {
            string sql = "select value from t_systeminfo where rkey='" + (int)M_Enum.EM_KEY.软件版本 + "'";
            int i = 0;
            int.TryParse(db.GetSelectValue(sql), out i);
            return i;
        }

        /// <summary>
        /// 更新软件版本状态
        /// </summary>
        /// <returns></returns>
        public bool UpdateSoftwareVersionStatus(int i)
        {
            string sql = "update t_systeminfo set value='" + i + "' where rkey='" + (int)M_Enum.EM_KEY.软件版本 + "'";
            return db.Excute(sql);
        }

        public int NumsOfDayRemaining()
        {
            string sql = "select 7-(julianday(datetime('now','localtime'))-julianday(OP_TIME)) from t_systeminfo where rkey='" + (int)M_Enum.EM_KEY.软件版本 + "'";
            int i = 0;
            int.TryParse(db.GetSelectValue(sql), out i);
            if (i < 0)
            {
                UpdateSoftwareVersionStatus((int)M_Enum.EM_SOFTWARESTATE.过期);
                i = 0;
            }
            return i;
        }
    }
}
