using System;

using log4net;

namespace Dot.Utility.Log
{
	public interface IWebLog : ILog
	{
        void Debug(string log);
        void Debug(LogMessage log);
        void Debug(LogMessage log, Exception ex);

        void Info(string log);
        void Info(LogMessage log);
        void Info(LogMessage log, Exception ex);

        void Warn(string log);
        void Warn(LogMessage log);
        void Warn(LogMessage log, Exception ex);

        void Error(string log);
        void Error(LogMessage log);
        void Error(LogMessage log, Exception ex);

        void Fatal(string log);
        void Fatal(LogMessage log);
        void Fatal(LogMessage log, Exception ex);

     

    }
}