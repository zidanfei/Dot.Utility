using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PMIS.Common
{
    /// <summary>
    /// Xml 操作辅助类
    /// </summary>
    public class XmlHelper
    {
        /// <summary>
        /// 序列化目标对象为Xml
        /// </summary>
        /// <param name="targetObject"></param>
        /// <returns></returns>
        public string XmlSerialize(object targetObject)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(targetObject.GetType());
            StringBuilder sb = new StringBuilder();
            System.Xml.XmlWriter xw = System.Xml.XmlWriter.Create(sb, new XmlWriterSettings() { Encoding = Encoding.UTF8 });
            serializer.Serialize(xw, targetObject);
            xw.Flush();
            xw.Close();
            return sb.ToString();
        }

        /// <summary>
        /// 对象序列化为Xml并保存为文件
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string XmlSerialize(object targetObject, string filePath)
        {
            string xml = XmlSerialize(targetObject);
            System.IO.File.WriteAllText(filePath, xml, Encoding.UTF8);
            return xml;
        }

        /// <summary>
        /// 反序列化Xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetFile"></param>
        /// <returns></returns>
        public T XmlFileDeserialize<T>(string targetFile)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            StringBuilder sb = new StringBuilder();
            System.Xml.XmlReader reader = System.Xml.XmlReader.Create(targetFile);
            object deobj = serializer.Deserialize(reader);
            reader.Close();
            return (T)deobj;
        }

        /// <summary>
        /// 反序列化xml
        /// </summary>
        /// <typeparam name="t"></typeparam>
        /// <param name="targetfile"></param>
        /// <returns></returns>
        public T XmlDeserialize<T>(string xml)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

            System.IO.MemoryStream stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(xml));
            System.Xml.XmlReader reader = System.Xml.XmlReader.Create(stream);
            object deobj = serializer.Deserialize(reader);
            reader.Close();
            stream.Close();
            return (T)deobj;
        }

    }
}
