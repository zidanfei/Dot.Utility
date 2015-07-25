using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Dot.Utility.DTO
{
    /// <summary>
    /// WCF通用错误信息
    /// </summary>
    [DataContract]
    public class GenericFault
    {
        /// <summary>
        /// 操作名称
        /// </summary>
        [DataMember]
        public string Operation { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        [DataMember]
        public string Message { get; set; }
    }
}
