using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.DTO
{ 
        /// <summary>
        /// 操作结果类
        /// </summary>
        [DataContract]
        public class WSResponseDto
        {
            public WSResponseDto()
            {
                IsSuccess = true;
            }

            /// <summary>
            /// 是否操作成功
            /// </summary>
            [DataMember]
            public bool IsSuccess
            {
                get;
                set;
            }

            /// <summary>
            /// 如果失败返回失败原因
            /// </summary>
            [DataMember]
            public string Message
            {
                get;
                set;
            }
        }

        [DataContract]
        public class WSResponseDto<T> : WSResponseDto
        {
            [DataMember]
            public T Result
            {
                get;
                set;
            }
            /// <summary>
            /// 数据总数
            /// </summary>
            [DataMember]
            public int TotalCount
            {
                get;
                set;
            }
        } 
}
