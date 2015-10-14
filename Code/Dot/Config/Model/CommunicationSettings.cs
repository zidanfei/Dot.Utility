using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Config.Model
{
    /// <summary>
    /// 服务配置信息-通信服务器配置
    /// </summary>
    [Serializable]
    public class CommunicationSetting
    {
        private int communicationPort = 4789;
        /// <summary>
        /// 发送消息的端口号-负责监听连接终端 默认：4789
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("CommunicationPort")]
        public int CommunicationPort
        {
            get
            {
                return this.communicationPort;
            }
            set
            {
                this.communicationPort = value;
            }
        }

        private string communicationIP = "";
        /// <summary>
        /// 中枢服务器的IP地址-默认为本机IPV4地址
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("CommunicationIP")]
        public string CommunicationIP
        {
            get
            {
                return this.communicationIP;
            }
            set
            {
                this.communicationIP = value;
            }
        }

        private int fileTransferPort = 6666;
        /// <summary>
        /// 发送文件传输端口 默认：6666
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("FileTransferPort")]
        public int FileTransferPort
        {
            get
            {
                return this.fileTransferPort;
            }
            set
            {
                this.fileTransferPort = value;
            }
        }

        private string fileTransferProtocol = "tcp";
        /// <summary>
        /// 发送文件传输协议 默认：tcp
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("FileTransferProtocol")]
        public string FileTransferProtocol
        {
            get
            {
                return this.fileTransferProtocol;
            }
            set
            {
                this.fileTransferProtocol = value;
            }
        }

        private bool isAutoRun = false;
        /// <summary>
        /// 是否默认启动
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("IsAutoRun")]
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
