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
            switch (i)
            {
                case 0:
                    CommonInfo.SoftwareState = (int)M_Enum.EM_SOFTWARESTATE.过期;
                    break;
                case 1:
                    CommonInfo.SoftwareState = (int)M_Enum.EM_SOFTWARESTATE.未注册;
                    break;
                case 2:
                    CommonInfo.SoftwareState = (int)M_Enum.EM_SOFTWARESTATE.已注册;
                    break;
            }
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


        public bool UpdateSoftwareRegisterCode(string value)
        {
            string sql = "update t_systeminfo set value='" + value + "' where rkey='" + (int)M_Enum.EM_KEY.注册码 + "'";
            return db.Excute(sql);
        }
        public int NumsOfDayRemaining()
        {
            string sql = "select 7-(julianday(datetime('now','localtime'))-julianday(OP_TIME)) from t_systeminfo where rkey='" + (int)M_Enum.EM_KEY.软件版本 + "'";
            double d = 0;
            double.TryParse(db.GetSelectValue(sql), out d);
            int i = (int) d;
            return i;
        }
        /// <summary>
        /// 返回版本信息
        /// </summary>
        /// <returns></returns>
        public string GetVersionMessage()
        {
            int status = GetVersionType();
            string msg = string.Empty;
            switch (status)
            {
                case 0:
                    msg = "试用期已过，部分功能使用受限！";
                    break;
                case 1:
                    int i = NumsOfDayRemaining();
                    if (i < 0)
                    {
                        i = 0;
                        UpdateSoftwareVersionStatus((int)M_Enum.EM_SOFTWARESTATE.过期);
                        GetVersionMessage();
                    }
                    else
                    {
                        msg = "试用版：\t" + "还剩余" + i + "天";
                    }
                    break;
                case 2:
                    msg = "正式版";
                    break;
            }
            return msg;
        }
    }
}
