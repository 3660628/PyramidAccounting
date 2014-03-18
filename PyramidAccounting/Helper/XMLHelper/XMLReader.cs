using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace PA.Helper.XMLHelper
{
    public class XMLReader
    {
        XmlDocument xmldoc;

        public XMLReader()
        {
            xmldoc = new XmlDocument();
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Data\\config.xml"))
            {
                new InitXMLConfig().InitXML();
            }
        }

        public string ReadXML(string Element)
        {
            try
            {
                xmldoc.Load(AppDomain.CurrentDomain.BaseDirectory + "Data\\config.xml");
                XmlNodeList rootList = xmldoc.SelectSingleNode("root").ChildNodes;
                foreach (XmlNode xn1 in rootList)
                {
                    XmlElement xe1 = (XmlElement)xn1;
                    XmlNodeList xnl1 = xe1.ChildNodes;
                    foreach (XmlNode xn2 in xnl1)
                    {
                        XmlElement xe2 = (XmlElement)xn2;
                        if (xe2.Name == Element)
                        {
                            return xe2.InnerText;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return "";
        }
    }
}
