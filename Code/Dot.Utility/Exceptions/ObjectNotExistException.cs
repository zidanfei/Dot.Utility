using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

namespace Dot.Utility.Exceptions
{
    /// <summary>
    /// 不存在指定对象时的异常信息
    /// </summary>
    [Serializable]
    public class ObjectNotExistException : iWSException
    {
        /// <summary>
        /// Contstructor.
        /// </summary>
        public ObjectNotExistException()
        {

        }

        /// <summary>
        /// Contstructor for serializing.
        /// </summary>
        public ObjectNotExistException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public ObjectNotExistException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public ObjectNotExistException(string message, System.Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
