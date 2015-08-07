using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Dot.Utility.Exceptions
{

    [System.Serializable]
    public class DotException : Exception
    {
      
         public DotException(string message) : base(message)
        {
        }

        public DotException(string message,Exception ex)
            : base(message,ex)
        {
        }

        public DotException()
            : base()
        {
        }

        public DotException(SerializationInfo serializationInfo,StreamingContext context)
            : base(serializationInfo,context)
        {
        }
    }
}