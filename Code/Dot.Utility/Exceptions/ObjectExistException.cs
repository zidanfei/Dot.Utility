using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

namespace Dot.Utility.Exceptions
{
    /// <summary>
    /// 对象已存在的异常信息
    /// </summary>
    [Serializable]
    public class ObjectExistException : DotException
    {
        /// <summary>
        /// Contstructor.
        /// </summary>
        public ObjectExistException()
        {

        }

        /// <summary>
        /// Contstructor for serializing.
        /// </summary>
        public ObjectExistException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public ObjectExistException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public ObjectExistException(string message, System.Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
