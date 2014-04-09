using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.Helper.DataBase;
using PA.Helper.DataDefind;
using PA.Helper.Tools;

namespace PA.ViewModel
{
    class ViewModel_系统管理
    {
        DataBase db = new DataBase();
        UsbController usb = new UsbController();
        /// <summary>
        /// 从数据库中获取第一次运行的存入的U盘标识
        /// </summary>
        /// <returns></returns>
        public string GetUsbDeviceID()
        {
            string sql = "select value from t_systeminfo where rkey='" + (int)M_Enum.EM_KEY.U盘标识 + "'";
            return db.GetSelectValue(sql);
        }

        /// <summary>
        /// 验证是否拷贝到其他地方运行
        /// </summary>
        /// <returns></returns>
        public bool GetRunningFlag()
        {
            bool flag = false;
            string str = usb.getSerialNumberFromDriveLetter();
            if (CommonInfo.U盘设备ID.Equals(str))
            {
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// 是否过期
        /// </summary>
        /// <returns></returns>
        public bool ValidateRuning()
        {
            bool flag = false;
            if (CommonInfo.SoftwareState != (int)M_Enum.EM_SOFTWARESTATE.过期)
            {
                flag = true;
            }
            return flag;
        }
    }
}
