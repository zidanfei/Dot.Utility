using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Dot.Utility.Exceptions
{
    /// <summary>
    /// 配置的异常信息
    /// </summary>
    [System.Serializable]
    public class ConfigException : iWSException
    {

        public ConfigException(string message)
            : base(message)
        {
        }

        public ConfigException(string message, Exception ex)
            : base(message, ex)
        {
        }

        public ConfigException()
            : base()
        {
        }

        public ConfigException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }
    }
}