using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.Helper.DataBase;
using PA.Model.DataGrid;
using PA.Helper.DataDefind;
using System.Data;

namespace PA.ViewModel
{
    class ViewModel_用户
    {
        DataBase db = new DataBase();
        private string sql = string.Empty;
        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        public List<Model_用户> GetAllUser()
        {
            sql = "select * from " + DBTablesName.T_USER + " where username not in ('root','admin') order by delete_mark";
            List<Model_用户> list = new List<Model_用户>();
            DataSet ds = db.Query(sql);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Model_用户 m = new Model_用户();
                    DataRow d = dt.Rows[i];
                    m.ID = Convert.ToInt32(d[0].ToString());
                    m.序号 = i + 1;
                    m.用户名 = d[1].ToString();
                    m.真实姓名 = d[2].ToString();
                    m.手机号码 = d[4].ToString();
                    m.用户权限 = RollbackAuthority(d[5].ToString());
                    m.日期 = Convert.ToDateTime(d[6].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    m.用户说明 = d[7].ToString();
                    m.是否使用 = Convert.ToInt32(d[8].ToString()) == 0 ? "正使用" : "停用";
                    list.Add(m);
                }
            }
            return list;
        }
        public bool StopUse(int userid)
        {
            sql = "update " + DBTablesName.T_USER + " set delete_mark= 1 where userid=" + userid;
            return db.Excute(sql);
        }
        /// <summary>
        /// 根据ID获取一个用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model_用户 GetUserInfo(int id)
        {
            Model_用户 m = new Model_用户();
            sql = "select * from " + DBTablesName.T_USER + " where userid=" + id;
            DataSet ds = db.Query(sql);
            DataRow d = ds.Tables[0].Rows[0];
            m.用户名 = d[1].ToString();
            m.真实姓名 = d[2].ToString();
            m.用户权限 = RollbackAuthority(d[5].ToString());
            m.用户说明 = d[7].ToString();
            return m;
        }
        /// <summary>
        /// 根据账号获取用户信息
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public Model_用户 GetUserInfo(string username)
        {
            Model_用户 m = new Model_用户();
            sql = "select * from " + DBTablesName.T_USER + " where username='" + username + "'";
            DataSet ds = db.Query(sql);
            DataRow d = ds.Tables[0].Rows[0];
            m.真实姓名 = d[2].ToString();
            m.用户权限 = RollbackAuthority(d[5].ToString());
            m.权限值 = Int32.Parse(d[5].ToString());
            m.用户说明 = d[7].ToString();
            return m;
        }
        public bool Update(Model_用户 m)
        {
            sql = "update " + DBTablesName.T_USER + " set realname='" + m.真实姓名 + "',authority=" + m.权限值 + ",comments='" + m.用户说明 + "' where userid=" + m.ID;
            return db.Excute(sql);
        }
        private string RollbackAuthority(string i)
        {
            string value = string.Empty;
            switch (i)
            {
                case "0":
                    value = "记账员";
                    break;
                case "1":
                    value = "审核员";
                    break;
                case "2":
                    value = "会计主管";
                    break;
                case "3":
                    value = "管理员";
                    break;
                case "4":
                    value = "超级管理员";
                    break;
            }
            return value;
        }

        public bool Insert(List<Model_用户> list)
        {
            return db.InsertPackage(DBTablesName.T_USER, list.OfType<object>().ToList());
        }
        public bool UpdatePassword(string username, string password)
        {
            sql = "UPDATE " + DBTablesName.T_USER + " SET PASSWORD='" + password + "' where USERNAME='" + username + "'";
            return db.Excute(sql);
        }
        public bool ValidateAccount(string username,string password)
        {
            sql = "SELECT * "
                + " FROM " + DBTablesName.T_USER + " WHERE USERNAME='" + username + "'"
                + " AND PASSWORD='" + password + "' and delete_mark=0";
            return db.IsExist(sql);
        }

        /// <summary>
        /// 用户名
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool ValidateUserName(string username)
        {
            sql = "select 1 from " + DBTablesName.T_USER + " where username='" + username + "' and delete_mark=0";
            return db.IsExist(sql);
        }

        public bool ValidateAccountOfficer(int authority)
        {
            sql = "select 1 from " + DBTablesName.T_USER + " where authority='" + authority + "' and delete_mark=0";
            return db.IsExist(sql);
        }

        public string GetUserName(int authority)
        {
            string sql = "select REALNAME from " + DBTablesName.T_USER + " where authority=" + authority + " and delete_mark=0";
            string result = db.GetSelectValue(sql);
            return string.IsNullOrEmpty(result)?CommonInfo.真实姓名:result;
        }
    }
}
