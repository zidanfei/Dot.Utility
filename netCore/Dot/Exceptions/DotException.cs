using Dot.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Dot.Utility.Exceptions
{

    [System.Serializable]
    public class DotException : Exception
    {

        public DotException(string message)
            : base(message)
        {
            msg = message;
        }

        public DotException(string message, Exception ex)
            : base(message, ex)
        {
            msg = message;
        }

        public DotException(LogMessage message)
        {
            if (message != null)
                LogMessage = message;
            else
                msg = "异常消息为空！";
        }

        string msg;
        public override string Message
        {
            get
            {
                if (msg == null)
                    return base.Message;
                else
                    return msg;
            }
        }

        public DotException()
            : base()
        {
        }

        public DotException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }

        public LogMessage LogMessage { get; set; }

        public object Data { get; set; }
    }
}