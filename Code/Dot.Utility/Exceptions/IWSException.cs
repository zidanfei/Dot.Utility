using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Dot.Utility.Exceptions
{

    [System.Serializable]
    public class iWSException : Exception
    {
      
         public iWSException(string message) : base(message)
        {
        }

        public iWSException(string message,Exception ex)
            : base(message,ex)
        {
        }

        public iWSException()
            : base()
        {
        }

        public iWSException(SerializationInfo serializationInfo,StreamingContext context)
            : base(serializationInfo,context)
        {
        }
    }
}