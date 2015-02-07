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
        /// 从数据库中=查询数据
        /// </summary>
        /// <returns></returns>
        public string GetSystemValue(int key)
        {
            string sql = "select value from " + DBTablesName.T_SYSTEMINFO + " where rkey='" + key + "'";
            return db.GetSelectValue(sql);
        }

        public bool UpdateStandardIndex(int index)
        {
            string sql = "update " + DBTablesName.T_SYSTEMINFO + " set value='" + index + "' where rkey='" + (int)ENUM.EM_KEY.会计制度 + "'";
            return db.Excute(sql);
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
            if (CommonInfo.SoftwareState != (int)ENUM.EM_SOFTWARESTATE.过期)
            {
                flag = true;
            }
            return flag;
        }

        #region 备份
        /// <summary>
        /// 是否现在备份
        /// </summary>
        /// <param name="day">自定义备份周期</param>
        /// <returns></returns>
        public bool IsBackupNow()
        {
            bool flag = false;
            string sql = "select case when (julianday(datetime('now','localtime'))-julianday(OP_TIME)) >= value then 'true' else 'false' end from " + DBTablesName.T_SYSTEMINFO + " where rkey='" + (int)ENUM.EM_KEY.备份标识 + "'" ;
            string value = db.GetSelectValue(sql);
            if (value.Equals("true"))
            {
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// 启用自动备份
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public bool UpdateAutoBackTag(string day) 
        {
            this.DeleteAutoBackTag();
            string sql = "insert into  " + DBTablesName.T_SYSTEMINFO + " (op_time,rkey,value,comments) values('"
                + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + (int)ENUM.EM_KEY.备份标识 + "','"
                + day + "','备份标识')";
            return db.Excute(sql);
        }
        /// <summary>
        /// 取消自动备份
        /// </summary>
        /// <returns></returns>
        public bool DeleteAutoBackTag()
        {
            string delSql = "delete from " + DBTablesName.T_SYSTEMINFO + " where rkey='" + (int)ENUM.EM_KEY.备份标识 + "'";
            return db.Excute(delSql);
        }

        #endregion
    }
}
