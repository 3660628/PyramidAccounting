using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Helper.DataDefind
{
    class M_Enum
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
            U盘标识 = 555
        }
    }
}
