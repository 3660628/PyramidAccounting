using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace PA.Helper.XMLHelper
{
    public class InitXMLConfig
    {
        XmlDocument xmldoc;

        public InitXMLConfig()
        {
            xmldoc = new XmlDocument();
        }

        public void InitXML()
        {
            XmlDeclaration xmldecl = xmldoc.CreateXmlDeclaration("1.0", "utf-8", null);
            xmldoc.AppendChild(xmldecl);

            XmlElement xmlelem = xmldoc.CreateElement("", "root", "");
            xmldoc.AppendChild(xmlelem);

            XmlNode root = xmldoc.SelectSingleNode("root");

            #region 1.s系统信息
            XmlElement s系统信息 = xmldoc.CreateElement("系统信息");
            XmlElement 单位 = xmldoc.CreateElement("单位");
            单位.InnerText = "单位";
            XmlElement 帐套信息 = xmldoc.CreateElement("账套信息");
            帐套信息.InnerText = "新建账套";
            XmlElement 数据库 = xmldoc.CreateElement("数据库");
            数据库.InnerText = "PyramidAccounting.db";
            XmlElement 会计制度 = xmldoc.CreateElement("会计制度");
            会计制度.InnerText = "0";
            XmlElement period = xmldoc.CreateElement("期");
            period.InnerText = "0";
            s系统信息.AppendChild(单位);
            s系统信息.AppendChild(帐套信息);
            s系统信息.AppendChild(数据库);
            s系统信息.AppendChild(会计制度);
            s系统信息.AppendChild(period);
            root.AppendChild(s系统信息);
            #endregion

            #region 2.备份信息
            XmlElement 自动备份标志 = xmldoc.CreateElement("自动备份标志");
            自动备份标志.InnerText = "true";
            XmlElement 备份时间 = xmldoc.CreateElement("备份时间");
            备份时间.InnerText = "7";
            XmlElement 备份路径 = xmldoc.CreateElement("备份路径");
            备份路径.InnerText = "D:\\";
            XmlElement 还原路径 = xmldoc.CreateElement("还原路径");
            还原路径.InnerText = "D:\\";
            s系统信息.AppendChild(自动备份标志);
            s系统信息.AppendChild(备份时间);
            s系统信息.AppendChild(备份路径);
            s系统信息.AppendChild(还原路径);

            #endregion

            #region 3.注册
            XmlElement register = xmldoc.CreateElement("注册");
            register.InnerText = "false";
            s系统信息.AppendChild(register);
            #endregion

            xmldoc.Save(AppDomain.CurrentDomain.BaseDirectory + "Data\\config.xml");
        }
    }
}
