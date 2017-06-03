using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Config.Model
{
    /// <summary>
    /// IOC容器基础配置
    /// </summary>
    [Serializable]
    public class IOCSetting
    {
        private List<ConfigurationItem> configurationItems = new List<ConfigurationItem>();
        private List<InterceptorItem> interceptorItems = new List<InterceptorItem>();
        private List<ObjectItem> objectItems = new List<ObjectItem>();
        private List<CustomObjectItem> customObjectItems = new List<CustomObjectItem>();

        string baseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
        private string fileDiscoveryPath;
        /// <summary>
        /// IOC注册时程序集查找的目录
        /// </summary>
        [System.Xml.Serialization.XmlElement("FilePath")]
        public string AssemblyFilePath
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(fileDiscoveryPath))
                {
                    return fileDiscoveryPath;
                }
                else if (!string.IsNullOrEmpty(ApplicationEnvironment) && ApplicationEnvironment.ToLower().Contains("web"))
                {
                    fileDiscoveryPath = System.IO.Path.Combine(baseDirectory, "bin");
                }
                else
                {
                    fileDiscoveryPath = baseDirectory;
                }
                return fileDiscoveryPath;
            }
            set
            {
                this.fileDiscoveryPath = value;
            }
        }
        /// <summary>
        /// 当前应用程序环境
        /// </summary>
        [System.Xml.Serialization.XmlElement("Environment")]
        public string ApplicationEnvironment
        {
            get;
            set;
        }

        private bool automaticSniffing = false;
        /// <summary>
        /// IOC注册时程序集查找的目录--如果该目录下不包含目标dll 则会自动嗅探查找dll
        /// </summary>
        [System.Xml.Serialization.XmlElement("AutomaticSniffing")]
        public bool AutomaticSniffing
        {
            get
            {
                return this.automaticSniffing;
            }
            set
            {
                this.automaticSniffing = value;
            }
        }

        /// <summary>
        /// 当前IOC的配置项信息
        /// </summary>
        [System.Xml.Serialization.XmlArray("ConfigurationItems")]
        public List<ConfigurationItem> ConfigurationItems
        {
            get
            {
                return this.configurationItems;
            }
            set
            {
                this.configurationItems = value;
            }
        }

        /// <summary>
        /// 当前IOC的配置项信息
        /// </summary>
        [System.Xml.Serialization.XmlArray("InterceptorItems")]
        public List<InterceptorItem> InterceptorItems
        {
            get
            {
                return this.interceptorItems;
            }
            set
            {
                this.interceptorItems = value;
            }
        }

        private string searchPath = "";
        /// <summary>
        /// IOC初始化查找的程序集目录--目录格式：文件夹名称 或 空 如果为空 则默认从当前的应用程序域内查找制定的程序集
        /// </summary>
        public string SearchPath
        {
            get
            {
                return this.searchPath;
            }
            set
            {
                this.searchPath = value;
            }
        }

        /// <summary>
        /// 当前IOC的配置项信息
        /// </summary>
        [System.Xml.Serialization.XmlArray("ObjectItems")]
        public List<ObjectItem> ObjectItems
        {
            get
            {
                return this.objectItems;
            }
            set
            {
                this.objectItems = value;
            }
        }

        /// <summary>
        /// 当前IOC的配置项信息--主要是配置用户自定义
        /// </summary>
        [System.Xml.Serialization.XmlArray("CustomObjectItems")]
        public List<CustomObjectItem> CustomObjectItems
        {
            get
            {
                return this.customObjectItems;
            }
            set
            {
                this.customObjectItems = value;
            }
        }
    }

    /// <summary>
    /// IOC配置项信息
    /// </summary>
    public class ConfigurationItem
    {
        private string key = "";
        /// <summary>
        /// 配置项名称
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("Key")]
        public string Key
        {
            get
            {
                return this.key;
            }
            set
            {
                this.key = value;
            }
        }

        private string value = "";
        /// <summary>
        /// 配置项的值
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("Value")]
        public string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        private string intanceCycle = "Singleton";
        /// <summary>
        /// 配置当前程序集范围内的对象的生命周期
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("LifeCycle")]
        public string LifeCycle
        {
            get
            {
                return this.intanceCycle;
            }
            set
            {
                this.intanceCycle = value;
            }
        }

        private string lineMode = "all";
        /// <summary>
        /// 配置当前程序集范围内的对象的生命周期
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("LineMode")]
        public string LineMode
        {
            get
            {
                return this.lineMode;
            }
            set
            {
                this.lineMode = value;
            }
        }
    }

    /// <summary>
    /// IOC拦截器配置信息
    /// </summary>
    public class InterceptionItem
    {
        private List<InterceptorItem> interceptorItems = new List<InterceptorItem>();
        private string name = "";
        /// <summary>
        /// 待拦截的程序集对象
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("AssemblyName")]
        public string AssemblyName
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        [System.Xml.Serialization.XmlArray("InterceptorItems")]
        public List<InterceptorItem> InterceptorItems
        {
            get
            {
                return this.interceptorItems;
            }
            set
            {
                this.interceptorItems = value;
            }
        }
    }

    /// <summary>
    /// 配置IOC中具体的组件的接口与实现的注册项
    /// </summary>
    public class ObjectItem
    {
        /// <summary>
        /// 名称
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("Name")]
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 基类标示名称
        /// 区分不同的实例
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("KeyName")]
        public string KeyName
        {
            get;
            set;
        }
        /// <summary>
        /// 接口类型
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("Interface")]
        public string Interface
        {
            get;
            set;
        }

        /// <summary>
        /// 实现类型格式 （程序集名称；类完全限定名）
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("Implement")]
        public string Implement
        {
            get;
            set;
        }

        private string intanceCycle = "Singleton";
        /// <summary>
        /// 配置当前程序集范围内的对象的生命周期
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("LifeCycle")]
        public string LifeCycle
        {
            get
            {
                return this.intanceCycle;
            }
            set
            {
                this.intanceCycle = value;
            }
        }
    }

    /// <summary>
    /// IOC拦截器配置信息
    /// </summary>
    public class InterceptorItem
    {
        private string name = "";
        /// <summary>
        /// 拦截器名称
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("Name")]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        private string typeName = "";
        /// <summary>
        /// 待拦截的程序集对象
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("TypeName")]
        public string TypeName
        {
            get
            {
                return this.typeName;
            }
            set
            {
                this.typeName = value;
            }
        }
        private string level = "";
        /// <summary>
        /// 拦截级别
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("InterceptLevel")]
        public string InterceptLevel
        {
            get
            {
                return this.level;
            }
            set
            {
                this.level = value;
            }
        }
        private List<InterceptorAssembly> interceptorAssemblies = new List<InterceptorAssembly>();

        /// <summary>
        /// 当前拦截器拦截的程序集列表
        /// </summary>
        [System.Xml.Serialization.XmlArray("InterceptorAssemblys")]
        public List<InterceptorAssembly> InterceptorAssemblys
        {
            get
            {
                return this.interceptorAssemblies;
            }
            set
            {
                this.interceptorAssemblies = value;
            }
        }

        private List<InterceptorPolicy> interceptorPolicies = new List<InterceptorPolicy>();

        /// <summary>
        /// 当前拦截器拦截的策略列表
        /// </summary>
        [System.Xml.Serialization.XmlArray("InterceptorPolicies")]
        public List<InterceptorPolicy> InterceptorPolicies
        {
            get
            {
                return this.interceptorPolicies;
            }
            set
            {
                this.interceptorPolicies = value;
            }
        }
    }

    /// <summary>
    /// 配置拦截器拦截的程序集信息
    /// </summary>
    public class InterceptorAssembly
    {
        private List<InterceptorTypeIgnore> interceptorTypeIgnores = new List<InterceptorTypeIgnore>();

        /// <summary>
        /// 当前程序集内忽略的对象类型列表
        /// </summary>
        [System.Xml.Serialization.XmlArray("InterceptorTypeIgnores")]
        public List<InterceptorTypeIgnore> InterceptorTypeIgnores
        {
            get
            {
                return this.interceptorTypeIgnores;
            }
            set
            {
                this.interceptorTypeIgnores = value;
            }
        }

        private string name = "";
        /// <summary>
        /// 待拦截的程序集对象
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("AssemblyName")]
        public string AssemblyName
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }



    }

    /// <summary>
    /// 拦截器忽略的对象类型
    /// </summary>
    public class InterceptorTypeIgnore
    {
        private string name = "";
        /// <summary>
        /// 待忽略的对象类型名称
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("Type")]
        public string Type
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
    }

    /// <summary>
    /// 拦截器策略
    /// </summary>
    public class InterceptorPolicy
    {
        private string name = "";
        /// <summary>
        /// 策略类型
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("Type")]
        public string Type
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        private string value = "";
        /// <summary>
        /// 策略的条件值
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("Value")]
        public string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        private string handler = "";
        /// <summary>
        /// 策略拦截后具体的处理器
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("Handler")]
        public string Handler
        {
            get
            {
                return this.handler;
            }
            set
            {
                this.handler = value;
            }
        }
    }

    /// <summary>
    /// 配置IOC中具体的组件的接口与实现的注册项
    /// </summary>
    public class CustomObjectItem
    {
        /// <summary>
        /// 名称
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("Name")]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 接口类型
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("Interface")]
        public string Interface
        {
            get;
            set;
        }

        /// <summary>
        /// 实现类型格式 （程序集名称；类完全限定名）
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("Implement")]
        public string Implement
        {
            get;
            set;
        }

        private string intanceCycle = "new";
        /// <summary>
        /// 配置当前程序集范围内的对象的生命周期
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("LifeCycle")]
        public string LifeCycle
        {
            get
            {
                return this.intanceCycle;
            }
            set
            {
                this.intanceCycle = value;
            }
        }
    }
}
