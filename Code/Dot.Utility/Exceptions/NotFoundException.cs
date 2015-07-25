using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Dot.Utility.Exceptions
{
    /// <summary>
    /// 未找到的异常信息
    /// </summary>
    public class NotFoundException : iWSException
    {
        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string message,Exception ex)
            : base(message,ex)
        {
        }

        public NotFoundException()
            : base()
        {
        }

        public NotFoundException(SerializationInfo serializationInfo,StreamingContext context)
            : base(serializationInfo,context)
        {
        }
    }
}
