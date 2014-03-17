using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.Helper.DataBase;
using PA.Model.DataGrid;
using PA.Helper.DataDefind;

namespace PA.ViewModel
{
    class ViewModel_用户
    {
        DataBase db = new DataBase();
        public void Insert()
        {

        }
        public bool UpdatePassword(string username, string password)
        {
            string sql = "UPDATE T_USER SET PASSWORD='" + password + "' where USER_NAME='" + username + "'";
            return db.Excute(sql);
        }
        public bool ValidatePassword(string username,string password)
        {
            string sql = "SELECT * "
                + " FROM T_USER "
                + " WHERE USER_NAME='" + username + "'"
                + " AND PASSWORD='" + password + "'";
            return db.IsExist(sql);
        }
    }
}
