using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Config.Model
{
    /// <summary>
    /// 客户端配置信息
    /// </summary>
    [Serializable]
    public class ClientSettings
    {
        public ClientSettings()
        {
            this.CommunicationSetting = new CommunicationSetting();
            this.DataBaseSetting = new DataBaseSetting();
            this.WCFSetting = new WCFSetting();
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
    }
}
