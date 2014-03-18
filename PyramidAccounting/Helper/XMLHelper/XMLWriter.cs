using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace PA.Helper.XMLHelper
{
    public class XMLWriter
    {
        XmlDocument xmldoc;
        public XMLWriter()
        {
            xmldoc = new XmlDocument();
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Data\\config.xml"))
            {
                new InitXMLConfig().InitXML();
            }
        }

        public void WriteXML(string Element, string Text)
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
                            xe2.InnerText = Text;
                        }
                    }
                }
                xmldoc.Save(AppDomain.CurrentDomain.BaseDirectory + "Data\\config.xml");
            }
            catch (Exception)
            {

            }
        }
    }
}
