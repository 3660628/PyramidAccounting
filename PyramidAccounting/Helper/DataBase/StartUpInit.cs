using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PA.Helper.XMLHelper;

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
            this.CheckFolder(folderList);

            if (!File.Exists("Data\\" + currentDBName))
            {
                new DBInitialize().Initialize();
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
