using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Dot.Utility.DTO
{
    /// <summary>
    /// 服务处理错误
    /// </summary>
    [DataContract]
    public class BusinessLogicFault
    {
        /// <summary>
        /// 服务调用名称
        /// </summary>
        [DataMember]
        public string Operation { get; set; }

        /// <summary>
        /// 服务调用过程中的错误信息
        /// </summary>
        [DataMember]
        public string Message { get; set; }
    }
}
