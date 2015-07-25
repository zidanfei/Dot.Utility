using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Exceptions
{
    /// <summary>
    /// 验证失败的异常信息
    /// </summary>
    [Serializable]
    public class ValidationException : iWSException
    {
        public ValidationException(string message)
            : base(message)
        {
        }

        public ValidationException(string message, Exception ex)
            : base(message, ex)
        {
        }

        public ValidationException()
            : base()
        {
        }

        public ValidationException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }
    }
}
