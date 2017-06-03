using System;

namespace Dot.Log
{
	public interface IWebLog : ILog
	{
        //void Debug(string log);
        void DebugDetail(LogMessage log);
        void DebugDetail(LogMessage log, Exception ex);

        //void Info(string log);
        void InfoDetail(LogMessage log);
        void InfoDetail(LogMessage log, Exception ex);

        //void Warn(string log);
        void WarnDetail(LogMessage log);
        void WarnDetail(LogMessage log, Exception ex);

        //void Error(string log);
        void ErrorDetail(LogMessage log);
        void ErrorDetail(LogMessage log, Exception ex);

        //void Fatal(string log);
        void Fatal(LogMessage log);
        void Fatal(LogMessage log, Exception ex);

     

    }
}