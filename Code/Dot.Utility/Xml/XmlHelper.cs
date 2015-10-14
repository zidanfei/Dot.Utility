
using Dot.Utility.Config;
using Dot.Utility.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Dot.Utility.Xml
{
    /// <summary>
    /// XML文件的操作辅助类
    /// </summary>
    public class XmlHelper
    {
        private static readonly object lock_flag = new object();
        public static XmlHelper context = null;
        private static ConcurrentDictionary<Type, XmlSerializer> _cache;
        private static XmlSerializerNamespaces _defaultNamespace;

        public static XmlHelper Instance
        {
            get
            {
                if (context == null)
                {
                    lock (lock_flag)
                    {
                        if (context == null)
                        {
                            context = new XmlHelper();
                        }
                    }
                }
                return context;
            }
        }

        static XmlHelper()
        {
           
            _defaultNamespace = new XmlSerializerNamespaces();
            _defaultNamespace.Add(string.Empty, string.Empty);

            _cache = new ConcurrentDictionary<Type, XmlSerializer>();
        }


        internal static XmlSerializer GetSerializer<T>()
        {
            var type = typeof(T);
            return _cache.GetOrAdd(type, XmlSerializer.FromTypes(new[] { type }).FirstOrDefault());
        }

        internal static XmlSerializer GetSerializer(Type type)
        {
            return _cache.GetOrAdd(type, XmlSerializer.FromTypes(new[] { type }).FirstOrDefault());
        }

        #region 存取配置文件

      


        /// <summary>
        /// 加载配置
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <returns></returns>
        public static T LoadConfig<T>(string path) where T : class, new()
        {
            Type t = typeof(T);
            if (!System.IO.File.Exists(path))
            {
                throw new System.IO.IOException("配置文件不存在");
            }
            T obj = null;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(path, System.Text.Encoding.Unicode))
            {
                obj = GetSerializer<T>().Deserialize(sr) as T;
                sr.Close();
            }
            return obj;
        }

       

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="cfg">要保存的配置对象</param>
        public static void SaveConfig(string configPath, object cfg)
        {
            if (cfg == null)
                throw new ArgumentNullException();

            try
            {
                Type t = cfg.GetType();

                if (!Directory.Exists(configPath))
                {
                    Directory.CreateDirectory(configPath);
                }

                string path = System.IO.Path.Combine(configPath, t.Name + ".xml");
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));

                using (System.IO.StreamWriter sr = new System.IO.StreamWriter(path, false, System.Text.Encoding.Unicode))
                {
                    GetSerializer(t).Serialize(sr, cfg);
                    sr.Close();
                }
            }
            catch
            {
            }
        }

        #endregion

        #region 修改指定XML节的信息        
        /// <summary>
        /// 修改指定XML节的信息       
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="value">The value.</param>
        public void SetNhibernateXmlAttributeValue(string filePath, string attributeName, string value)
        {
            try
            {
                if (!System.IO.File.Exists(filePath))
                    return;

                System.Xml.XmlDocument document = new System.Xml.XmlDocument();
                document.Load(filePath);

                //hibernate-configuration
                var rootNode = document.DocumentElement;

                //session-factory
                var firstNode = rootNode.FirstChild;

                //propertys
                var propertyNodes = firstNode.ChildNodes;

                bool isBreak = false;
                foreach (System.Xml.XmlNode propertyNode in propertyNodes)
                {
                    var attributes = propertyNode.Attributes;

                    if (attributes.Count > 0)
                    {
                        foreach (System.Xml.XmlAttribute attribute in attributes)
                        {
                            if (attribute.Value == attributeName)
                            {
                                isBreak = true;
                                break;
                            }
                        }
                    }

                    if (isBreak)
                    {
                        var connectionStringNode = propertyNode.FirstChild;
                        connectionStringNode.Value = value;
                        break;
                    }
                }

                document.Save(filePath);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取指定XML节的信息    
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public string GetNhibernateXmlAttributeValue(string filePath, string attributeName)
        {
            string returnValue = string.Empty;
            try
            {
                if (!System.IO.File.Exists(filePath))
                    return returnValue;

                System.Xml.XmlDocument document = new System.Xml.XmlDocument();
                document.Load(filePath);

                //hibernate-configuration
                var rootNode = document.DocumentElement;

                //session-factory
                var firstNode = rootNode.FirstChild;

                //propertys
                var propertyNodes = firstNode.ChildNodes;

                bool isBreak = false;
                foreach (System.Xml.XmlNode propertyNode in propertyNodes)
                {
                    var attributes = propertyNode.Attributes;

                    if (attributes.Count > 0)
                    {
                        foreach (System.Xml.XmlAttribute attribute in attributes)
                        {
                            if (attribute.Value == attributeName)
                            {
                                isBreak = true;
                                break;
                            }
                        }
                    }

                    if (isBreak)
                    {
                        var connectionStringNode = propertyNode.FirstChild;
                        returnValue = connectionStringNode.Value;
                        break;
                    }
                }

                return returnValue;
            }
            catch (Exception)
            {
                return returnValue;
            }
        }
        #endregion

        #region 将Object通过xml序列化的方式，序列化为xml字符串

        /**/
        /// <summary> 
        /// 序列化对象 
        /// </summary> 
        /// <typeparam name=\"T\">对象类型</typeparam> 
        /// <param name=\"t\">对象</param> 
        /// <returns></returns> 
        public static string Serialize(object t)
        {
            if (t == null)
                throw new ArgumentNullException("待保存的配置文件信息不能为null");

            using (StringWriter sw = new StringWriter())
            {
                GetSerializer(t.GetType()).Serialize(sw, t);
                return sw.ToString();
            }
        }

        /**/
        /// <summary> 
        /// 反序列化为对象 
        /// </summary> 
        /// <param name=\"type\">对象类型</param> 
        /// <param name=\"s\">对象序列化后的Xml字符串</param> 
        /// <returns></returns> 
        public static object Deserialize(Type type, string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentNullException("配置文件内容为null，无法读取配置文件内容");

            using (StringReader sr = new StringReader(s))
            {
                return GetSerializer(type).Deserialize(sr);
            }
        }

        #endregion

        #region XmlDocument 操作


        #endregion
    }
}
