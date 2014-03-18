using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PA.Helper.DataBase
{
    class StartUpInit
    {
        private string defaultDBName = "PyramidAccounting.db";
        private string guidePath = "Guid.jb";
        private string bookInfoFile = "Info.inf";
        public StartUpInit()
        {
            
        }

        public void Init()
        {
            string currentDBName = LoadDBName();
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
        /// <summary>
        /// 获取启动数据库的名称
        /// </summary>
        /// <returns></returns>
        public string LoadDBName()
        {
            string dbname = defaultDBName;
            if (!File.Exists(guidePath))
            {
                FileStream fs = new FileStream(guidePath, FileMode.CreateNew);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(defaultDBName);   //默认数据库名称
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            else
            {
                StreamReader sr = File.OpenText(guidePath);
                dbname = sr.ReadToEnd();
                sr.Close();
            }
            return dbname;
        }

        public string LoadBookName()
        {
            string bookName = "新建账套";
            if (!File.Exists(bookInfoFile))
            {
                FileStream fs = new FileStream(bookInfoFile, FileMode.CreateNew);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(bookName);  
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            else
            {
                StreamReader sr = File.OpenText(bookInfoFile);
                bookName = sr.ReadToEnd();
                sr.Close();
            }
            return bookName;
        }

        /// <summary>
        /// 改写文件内容
        /// </summary>
        /// <param name="name"></param>
        public void Truncate(string comments,string filePath)
        {
            FileStream fs;
            if (!File.Exists(filePath))
            {
                fs = new FileStream(filePath, FileMode.CreateNew);
            }
            else
            {
                fs = new FileStream(filePath, FileMode.Truncate);
            }
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(comments);
            sw.Flush();
            sw.Close();
            fs.Close();
        }
    }
}
