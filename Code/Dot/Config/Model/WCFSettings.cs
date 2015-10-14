using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Config.Model
{
    /// <summary>
    /// 服务配置信息-WCF服务配置
    /// </summary>
    [Serializable]
    public class WCFSetting
    {
        private string serviceName = "localhost";
        string baseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// 服务器名称
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("ServiceName")]
        public string ServiceName
        {
            get
            {
                return this.serviceName;
            }
            set
            {
                this.serviceName = value;
            }
        }

        private string serviceIP = "";
        /// <summary>
        /// 服务器 IP地址
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("ServiceIP")]
        public string ServiceIP
        {
            get
            {
                return this.serviceIP;
            }
            set
            {
                this.serviceIP = value;
            }
        }

        private int port = 4545;
        /// <summary>
        /// 服务端口号 默认端口号：4545
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("Port")]
        public int Port
        {
            get
            {
                return this.port;
            }
            set
            {
                this.port = value;
            }
        }

        private bool isEnableDebug = false;
        /// <summary>
        /// 默认不发送调试信息-如果调试状态，请将该属性设置为true
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("IsEnableDebug")]
        public bool IsEnableDebug
        {
            get
            {
                return this.isEnableDebug;
            }
            set
            {
                this.isEnableDebug = value;
            }
        }

        private string baseAddress = "";
        /// <summary>
        /// 服务器基地址
        /// </summary>
        public string BaseAddress
        {
            get
            {
                return this.baseAddress;
            }
            set
            {
                this.baseAddress = value;
            }
        }

        private string binding = "nettcpbinding";
        /// <summary>
        /// 默认采用的通信协议-TCP
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("Binding")]
        public string Binding
        {
            get
            {
                return this.binding;
            }
            set
            {
                this.binding = value;
            }
        }

        private string publishType = "WcfService";
        /// <summary>
        /// 默认采用的服务发布方式-WcfService
        /// </summary>
        [System.Xml.Serialization.XmlElement("PublishType")]
        public string PublishType
        {
            get
            {
                return this.publishType;
            }
            set
            {
                this.publishType = value;
            }
        }

        private string serverDiscoveryPath = System.AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// 服务器发现的默认地址-平台启动后就查找的路径
        /// </summary>
        [System.Xml.Serialization.XmlElement("ServerDiscoveryPath")]
        public string ServerDiscoveryPath
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(serverDiscoveryPath))
                {
                    return serverDiscoveryPath;
                }
                else if (ApplicationEnvironment.ToLower().Contains("web"))
                {
                    serverDiscoveryPath = System.IO.Path.Combine(baseDirectory, "bin");
                }
                else
                {
                    serverDiscoveryPath = baseDirectory;
                }
                return serverDiscoveryPath;
            }
            set
            {
                this.serverDiscoveryPath = value;
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

        private string defaultServiceCycle = "singleton";
        /// <summary>
        /// 服务的调用方式
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("ServiceCycle")]
        public string ServiceCycle
        {
            get
            {
                return this.defaultServiceCycle;
            }
            set
            {
                this.defaultServiceCycle = value;
            }
        }

        private string filter = ".Service.Implement.dll";
        /// <summary>
        /// 服务查找的程序集过滤器-以指定过滤器结尾的程序集名称
        /// </summary>
        [System.Xml.Serialization.XmlElement("AssemblyFilter")]
        public string AssemblyFilter
        {
            get
            {
                return this.filter;
            }
            set
            {
                this.filter = value;
            }
        }

        private string serviceMode = "local";
        /// <summary>
        /// 服务运行模式-local：局域网模式；distribute：分布式
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("ServiceMode")]
        public string ServiceMode
        {
            get
            {
                return this.serviceMode;
            }
            set
            {
                this.serviceMode = value;
            }
        }

        private bool isAutoRun = false;
        /// <summary>
        /// 是否默认启动
        /// </summary>
        [System.Xml.Serialization.XmlElement("IsAutoRun")]
        public bool IsAutoRun
        {
            get
            {
                return this.isAutoRun;
            }
            set
            {
                this.isAutoRun = value;
            }
        }
    }
}
