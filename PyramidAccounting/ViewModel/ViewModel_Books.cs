using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PA.Model.DataGrid;
using PA.Helper.DataDefind;
using PA.Helper.DataBase;

namespace PA.ViewModel
{
    class ViewModel_Books
    {
        private DataBase db = new DataBase();
        public void Insert(List<Model_帐套> list)
        {
            db.InsertPackage(DBTablesName.T_BOOKS, list.OfType<object>().ToList());
        }
    }
}
