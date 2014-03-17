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
        public void Update()
        {
            List<PA.Model.Database.UpdateParm> lists = new List<Model.Database.UpdateParm>();
            PA.Model.Database.UpdateParm parm = new Model.Database.UpdateParm();
            parm.TableName = DBTablesName.T_USER;
            parm.Key = "PARENTID";
            parm.Value = "'asdasd'";
            parm.WhereParm = "vid=1";
            lists.Add(parm);
            db.UpdatePackage(lists);
        }
    }
}
