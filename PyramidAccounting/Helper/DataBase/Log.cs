using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PA.Helper.DataBase
{
    class Log
    {
        public static void Write(string log)
        {
            FileStream fs;
            string filePath = "Log\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            if(!File.Exists(filePath))
            {
                fs = new FileStream(filePath, FileMode.CreateNew);
            }
            else
            {
                fs = new FileStream(filePath, FileMode.Append);
            }
            StreamWriter sw = new StreamWriter(fs);
            log = DateTime.Now + "  =========  " + log;
            sw.WriteLine(log);
            //清空缓冲区  
            sw.Flush();
            //关闭流  
            sw.Close();
            fs.Close();
        }

    }
}
