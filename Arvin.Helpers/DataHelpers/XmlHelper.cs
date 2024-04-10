using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Arvin.Helpers
{
    /// <summary>
    /// Xml帮助类
    /// </summary>
    public sealed class XmlHelper
    {
        XmlDocument XmlDoc = new XmlDocument();

        public XmlHelper(string xml)
        {
            try
            {
                XmlDoc.LoadXml(xml);
                XmlNodeList xnl = XmlDoc.SelectSingleNode("//head").ChildNodes;
                string propertyname = "";
                foreach (XmlNode xn in xnl)
                {
                    propertyname = xn.Name;
                    if (propertyname != "")
                    {
                        if (this.GetType().GetProperty(propertyname) != null)
                        {
                            this.GetType().GetProperty(propertyname).SetValue(this, Convert.ChangeType(xn.InnerText.Trim(), this.GetType().GetProperty(propertyname).PropertyType), null);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                xml = ex.Message;
            }
        }

        public void InsertElement(string MainNode, string Element, string Content)
        {
            XmlNodeList NodeList = XmlDoc.SelectNodes("//" + MainNode);
            XmlNode objNode = NodeList[NodeList.Count - 1];
            XmlElement objElement = XmlDoc.CreateElement(Element);
            objElement.InnerText = Content;
            objNode.AppendChild(objElement);
        }

        public string SaveToString()
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xmlSetting = new XmlWriterSettings();
            xmlSetting.Encoding = new UTF8Encoding(false);
            xmlSetting.Indent = true;//XmlDoc.save直接保存会有编码问题
            XmlWriter writer = XmlWriter.Create(ms, xmlSetting);
            XmlDoc.Save(writer);
            writer.Close();
            string xml = Encoding.UTF8.GetString(ms.ToArray());
            return xml;
        }

        public XmlNodeList GetXmlNodeList(string MainNode)
        {
            XmlNodeList xnl = XmlDoc.SelectNodes("//" + MainNode);
            return xnl;
        }

        public void AppendNode(XmlNode mainNode, string nodeName, string content)
        {
            XmlElement element = XmlDoc.CreateElement(nodeName);
            element.InnerText = content;
            mainNode.AppendChild(element);
        }

    }
}
