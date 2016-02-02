using System;

using log4net;

namespace Dot.Utility.Log
{
	public interface IWebLog : ILog
	{
        void Debug(LogMessage log);
        void Debug(LogMessage log, Exception ex);

        void Info(LogMessage log);
        void Info(LogMessage log, Exception ex);

        void Warn(LogMessage log);
        void Warn(LogMessage log, Exception ex);

        void Error(LogMessage log);
        void Error(LogMessage log, Exception ex);

        void Fatal(LogMessage log);
        void Fatal(LogMessage log, Exception ex);

     

    }
}