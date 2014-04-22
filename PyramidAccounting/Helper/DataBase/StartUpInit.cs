using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PA.Helper.XMLHelper;
using PA.Helper.DataDefind;

namespace PA.Helper.DataBase
{
    class StartUpInit
    {
        public StartUpInit()
        {
            
        }

        public void Init()
        {
            string currentDBName = new XMLReader().ReadXML("数据库");
            List<string> folderList = new List<string>();
            folderList.Add("Data");
            folderList.Add("Log");
            folderList.Add("Excel");
            folderList.Add("Excel\\打印");
            this.CheckFolder(folderList);

            if (!File.Exists("Data\\" + currentDBName))
            {
                new DBInitialize().Initialize();
            }
            //判断是否将无密码数据库拷贝回来，再进行加密操作
            if (new XMLReader().ReadXML("注册").Equals("芝麻关门"))
            {
                DBInitialize.ChangeDBPassword();
                new XMLWriter().WriteXML("注册", "true");
            }
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
