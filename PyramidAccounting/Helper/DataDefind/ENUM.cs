using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Helper.DataDefind
{
    class ENUM
    {
        public enum EM_SOFTWARESTATE
        {
            过期,
            未注册,
            已注册
        }

        public enum EM_KEY
        {
            软件版本 = 999,
            注册码 = 777,
            U盘标识 = 555,
            备份标识 = 333,
            会计制度 = 111
        }
    }
}
