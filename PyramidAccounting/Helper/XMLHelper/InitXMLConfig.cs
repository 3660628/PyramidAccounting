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

            #region 
            XmlElement 系统信息 = xmldoc.CreateElement("系统信息");
            XmlElement 公司 = xmldoc.CreateElement("公司");
            公司.InnerText = "公司";
            XmlElement 帐套信息 = xmldoc.CreateElement("帐套信息");
            帐套信息.InnerText = "新建帐套";
            XmlElement 数据库 = xmldoc.CreateElement("数据库");
            数据库.InnerText = "PyramidAccounting.db";
            系统信息.AppendChild(公司);
            系统信息.AppendChild(帐套信息);
            系统信息.AppendChild(数据库);

            root.AppendChild(系统信息);
            #endregion

            xmldoc.Save(AppDomain.CurrentDomain.BaseDirectory + "Data\\config.xml");
        }
    }
}
