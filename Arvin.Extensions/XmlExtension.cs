using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Arvin.Extensions
{
    /// <summary>
    /// Xml相关操作
    /// </summary>
    public static class XmlExtension
    {
        public static XmlDocument ToXml(this string s)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(s);
            return xmlDoc;
        }

        public static string SaveToString(this XmlDocument doc)
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xmlSetting = new XmlWriterSettings();
            xmlSetting.Encoding = new UTF8Encoding(false);
            xmlSetting.Indent = true;//XmlDoc.save直接保存会有编码问题
            XmlWriter writer = XmlWriter.Create(ms, xmlSetting);
            doc.Save(writer);
            writer.Close();
            string xml = Encoding.UTF8.GetString(ms.ToArray());
            return xml;
        }

        public static XmlNodeList GetXmlNodeList(this XmlDocument doc, string nodeName)
        {
            return doc.GetElementsByTagName(nodeName);
        }


        public static List<T> ToList<T>(this XmlNodeList nodeList) where T : class
        {
            List<T> list = new List<T>();
            foreach (XmlNode item in nodeList)
            {
                list.Add(item.ToModel<T>());
            }
            return list;
        }

        public static T ToModel<T>(this XmlNode node) where T : class
        {
            return node.XmlSerialize().XmlDeserialize<T>();
        }

        public static XmlNode GetNode(this XmlDocument doc, string nodeName)
        {
            return doc.SelectSingleNode("//" + nodeName);
        }

        public static XmlNode GetNode(this XmlNode doc, string nodeName)
        {
            return doc.SelectSingleNode(nodeName);
        }

        public static string GetNodeValue(this XmlDocument doc, string nodeName, string defaultVal = "")
        {
            return doc.GetNode(nodeName).ToInnerTextOrDefault(defaultVal);
        }

        public static string GetNodeValue(this XmlNode doc, string nodeName, string defaultVal = "")
        {
            return doc.GetNode(nodeName).ToInnerTextOrDefault(defaultVal);
        }


        public static string GetXmlNodeText(this string s, string nodeName, string defaultVal = "")
        {
            if (s.IsNullOrEmpty()) return string.Empty;
            return s.ToXml().GetNode(nodeName).ToInnerTextOrDefault(defaultVal);
        }

        public static bool HasValue(this XmlNode node)
        {
            return node != null && !node.InnerText.IsNullOrWhiteSpace();
        }

        public static string ToInnerTextOrDefault(this XmlNode node, string defaultVal = "")
        {
            return node == null ? defaultVal : node.InnerText;
        }

        /// <summary>
        /// 将XML字符串转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(this string str)
            where T : class
        {
            try
            {
                using (StringReader sr = new StringReader(str))
                {
                    XmlSerializer xmldes = new XmlSerializer(typeof(T));
                    return xmldes.Deserialize(sr) as T;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("将XML转换成实体对象异常", ex);
            }
        }

        /// <summary>
        /// 将对象转换为XML字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string XmlSerialize(this object obj, bool isRemoveXmlDeclaration = false)
        {
            try
            {
                using (StringUTF8Writer sw = new StringUTF8Writer())
                {
                    XmlSerializer serializer = new XmlSerializer(obj.GetType());
                    //去掉要结点的 xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" 属性
                    XmlSerializerNamespaces _namespaces = new XmlSerializerNamespaces();
                    _namespaces.Add("", "");
                    serializer.Serialize(sw, obj, _namespaces);
                    sw.Close();


                    if (isRemoveXmlDeclaration)
                        return sw.ToString().TrimStart("<?xml version=\"1.0\" encoding=\"utf-8\"?>".ToCharArray());
                    else
                        return sw.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("将实体对象转换成XML异常", ex);
            }
        }


        public static string RemoveHead<T>(this string s)
        {
            string matchText = $"<{typeof(T).Name}>";
            int index = s.IndexOf(matchText);
            return s.Remove(0, index);
        }

        public static string RemoveEnd<T>(this string s)
        {
            string matchText = $"</{typeof(T).Name}>";
            int index = s.LastIndexOf(matchText);
            int pos = index + matchText.Length;
            return s.Remove(pos, s.Length - pos);
        }

        public static string ReplaceNodeName(this string s, string oldNodeName, string newNodeName)
        {
            return
                s.Replace($"<{oldNodeName}>", $"<{newNodeName}>")
                .Replace($"</{oldNodeName}>", $"</{newNodeName}>");
        }

        public class StringUTF8Writer : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }

        public static void InsertElement(this XmlDocument XmlDoc, string MainNode, string Element, string Content)
        {
            //插入一个节点，不带属性。 
            //XmlNode objNode = XmlDoc.SelectSingleNode("//"+MainNode);
            XmlNodeList NodeList = XmlDoc.SelectNodes("//" + MainNode);
            XmlNode objNode = NodeList[NodeList.Count - 1];
            XmlElement objElement = XmlDoc.CreateElement(Element);
            objElement.InnerText = Content;
            objNode.AppendChild(objElement);
        }

        /// <summary>
        /// 将 xml中的指定节点反序列化为指定模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static T ToModel<T>(this XmlDocument doc, string nodeName) where T : class
        {
            string nodeVal = doc.GetNodeValue(nodeName).XmlSerialize();
            return nodeVal.XmlDeserialize<T>();
        }

        /// <summary>
        /// 将 xml中的指定节点反序列化为指定模型列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static List<T> ToModelList<T>(this XmlDocument doc, string nodeName) where T : class
        {
            List<T> list = new List<T>();
            var nodeList = doc.GetXmlNodeList(nodeName);
            foreach (XmlNode item in nodeList)
            {
                string itemStr = item.XmlSerialize();
                list.Add(itemStr.XmlDeserialize<T>());
            }
            return list;
        }
    }
}
