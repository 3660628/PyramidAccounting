using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PA.Helper.XMLHelper;
using PA.Helper.DataDefind;
using PA.View.ResourceDictionarys.MessageBox;

namespace PA.Helper.DataBase
{
    class StartUpInit
    {
        public StartUpInit()
        {
            
        }

        public bool Init()
        {
            string currentDBName = new XMLReader().ReadXML("数据库");
            List<string> folderList = new List<string>();
            folderList.Add("Data");
            folderList.Add("Log");
            folderList.Add("Excel");
            folderList.Add("Excel\\打印");
            this.CheckFolder(folderList);
            string str = "Data\\" + currentDBName;
            FileInfo fi = new FileInfo(str);
            if (!File.Exists(str)||fi.Length==0)
            {
                new DBInitialize().Initialize();
            }
            //判断是否将无密码数据库拷贝回来，再进行加密操作
            if (new XMLReader().ReadXML("注册").Equals("芝麻关门"))
            {
                DBInitialize.ChangeDBPassword();
                new XMLWriter().WriteXML("注册", "true");
            }
            return true;
        }

        /// <summary>
        /// 检测文件夹
        /// </summary>
        /// <param name="Listpath"></param>
        private void CheckFolder(List<string> Listpath)
        {
            foreach (string s in Listpath)
            {
                if (!Directory.Exists(s))
                {
                    Directory.CreateDirectory(s);
                }
            }
        }


    }
}
