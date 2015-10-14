using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Config.Model
{
    /// <summary>
    /// 服务配置信息
    /// </summary>
    [Serializable]
    public class ServerSettings
    {
        public ServerSettings()
        {
            this.CommunicationSetting = new CommunicationSetting();

            this.DataBaseSetting = new DataBaseSetting();

            MappingAssembly assembly = new MappingAssembly();
            assembly.AssemblyName = "Test";
            this.DataBaseSetting.MappingAssemblys.Add(assembly);

            this.WCFSetting = new WCFSetting();
            this.IOCSetting = new IOCSetting();
            this.KeyValueSettings = new KeyValueSettings();
            this.DomainSetting = new DomainSetting();

            //ConfigurationItem item = new ConfigurationItem();
            //item.Key = "Domain";
            //item.Value = "XXXx";
            //item.LifeCycle = "new";
            //this.IOCSetting.ConfigurationItems.Add(item);
        }

        /// <summary>
        /// 通信服务设置
        /// </summary>
        [System.Xml.Serialization.XmlElement("CommunicationSetting")]
        public CommunicationSetting CommunicationSetting
        {
            get;
            set;
        }

        /// <summary>
        /// 数据库设置
        /// </summary>
        [System.Xml.Serialization.XmlElement("DataBaseSetting")]
        public DataBaseSetting DataBaseSetting
        {
            get;
            set;
        }

        /// <summary>
        /// WCF服务设置
        /// </summary>
        [System.Xml.Serialization.XmlElement("WCFSetting")]
        public WCFSetting WCFSetting
        {
            get;
            set;
        }

        /// <summary>
        /// IOC配置
        /// </summary>
        [System.Xml.Serialization.XmlElement("IOCSetting")]
        public IOCSetting IOCSetting
        {
            get;
            set;
        }

        /// <summary>
        /// Domain领域驱动配置
        /// </summary>
        [System.Xml.Serialization.XmlElement("DomainSetting")]
        public DomainSetting DomainSetting
        {
            get;
            set;
        }

        /// <summary>
        /// 键值对配置信息
        /// </summary>
        [System.Xml.Serialization.XmlElement("KeyValueSettings")]
        public KeyValueSettings KeyValueSettings
        {
            get;
            set;
        }

        /// <summary>
        /// 当前配置文件所读取的路径
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public string ConfigFilePath
        {
            get;
            set;
        }
    }
}
