using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Licenses
{
    /// <summary>
    /// 序列信息
    /// </summary>
    [Serializable]
    [System.Xml.Serialization.XmlType("LicenseInfo")]
    public class LicenseInfo
    {
        /// <summary>
        /// 机器码
        /// </summary>
        [System.Xml.Serialization.XmlElement("MachineCode")]
        public string MachineCode { get; set; }

        /// <summary>
        /// 产品
        /// </summary>
        [System.Xml.Serialization.XmlElement("Product")]
        public string Product { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        [System.Xml.Serialization.XmlElement("Version")]
        public string Version { get; set; }

        /// <summary>
        /// 最大用户数
        /// </summary>
        [System.Xml.Serialization.XmlElement("MaxUser")]
        public int MaxUser { get; set; }

        /// <summary>
        /// 发布日期
        /// </summary>
        [System.Xml.Serialization.XmlElement("IssuedDate")]
        public DateTime IssuedDate { get; set; }

        /// <summary>
        /// 到期日期
        /// </summary>
        [System.Xml.Serialization.XmlElement("ExpireDate")]
        public DateTime ExpireDate { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        [System.Xml.Serialization.XmlElement("Customer")]
        public string Customer { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        [System.Xml.Serialization.XmlElement("SignatureInfo")]
        public string SignatureInfo { get; set; }

        public override string ToString()
        {
            return MachineCode + "|||" + Product + "|||" + Version + "|||" + MaxUser + "|||" + IssuedDate + "|||" + ExpireDate + "|||" + Customer;
        }
    }
    public enum Version { Enterprise, Professional, Personal }
}
