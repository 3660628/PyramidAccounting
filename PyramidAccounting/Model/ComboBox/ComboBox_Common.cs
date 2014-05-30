using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PA.Model.DataGrid;
using PA.Helper.DataBase;
using PA.ViewModel;
using PA.Helper.DataDefind;
using System.IO;

namespace PA.Model.ComboBox
{
    class ComboBox_Common
    {
        private string sql = string.Empty;
        private DataSet ds = new DataSet();
        private DataBase db = new DataBase();
        private ViewModel_Books vmb = new ViewModel_Books();

        public List<Model_账套> GetComboBox_账套()
        {
            List<Model_账套> list = new List<Model_账套>();
            Model_账套 def = new Model_账套();
            def.ID = "0";
            def.账套名称 = "新建账套";
            list.Add(def);
            sql = "select id,book_name from t_books where delete_mark=0 order by create_date desc";
            ds = db.Query(sql);
            if (ds != null)
            {
                DataTable dt = db.Query(sql).Tables[0];
                foreach(DataRow d in dt.Rows)
                {
                    Model_账套 m = new Model_账套();
                    m.ID = d[0].ToString();
                    m.账套名称 = d[1].ToString();
                    list.Add(m);
                }
            }
            return list;
        }

        /// <summary>
        /// 创建账套显示的会计制度列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetComboBox_会计制度()
        {
            List<string> list = new List<string>();
            string[] filesnames = Directory.GetFiles("Data//科目");
            foreach (string filename in filesnames)
            {
                list.Add(filename.Replace("Data//科目\\","").Split('.')[0]);
            }
            //list.Add("《行政单位会计制度》财预字[1998]49号");
            return list;
        }

        public List<string> GetComboBox_审核()
        {
            List<string> list = new List<string>();
            list.Add("全部");
            list.Add("已审核");
            list.Add("未审核");
            return list;
        }


        /// <summary>
        /// 获取期数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<string> GetComboBox_期数(int type)
        {
            List<string> list = new List<string>();
            int count = 0;
            switch (type)
            {
                case 0:
                    list.Add("全部");
                    count = CommonInfo.当前期;
                    break;
                case 1:
                    count = CommonInfo.当前期 - 1;
                    break;
            }
            string str = vmb.GetValue();
            string value = str.Split('\t')[0].Split(',')[0].Split('年')[0];
            for (int i = 1; i <= count; i++)
            {
                string s = value + "年" + i + "期";
                list.Add(s);
            }
            if (count == 0)
            {
                string s = value + "年1期";
                list.Add(s);
            }
            return list;
        }
    }
}
