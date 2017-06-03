using System;

using log4net.Core;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Dot.Utility.Web;
using Dot.IOC;
using log4net;

namespace Dot.Log
{

    /// <summary>
    /// http://www.cnblogs.com/hb_cattle/articles/1560778.html
    /// </summary>
    public abstract class DotLogImpl : ILog
    {
        public abstract log4net.ILog Log { get; }

        public bool IsFatalEnabled => Log.IsFatalEnabled;

        public bool IsWarnEnabled => Log.IsWarnEnabled;

        public bool IsInfoEnabled => Log.IsInfoEnabled;

        public bool IsDebugEnabled => Log.IsDebugEnabled;

        public bool IsErrorEnabled => Log.IsErrorEnabled;

        public void Debug(object message)
        {
            Log.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            Log.Debug(message, exception);

        }

        public void DebugFormat(string format, params object[] args)
        {
            Log.DebugFormat(format, args);

        }

        public void Error(object message)
        {
            Log.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            Log.Error(message, exception);

        }

        public void ErrorFormat(string format, params object[] args)
        {
            Log.ErrorFormat(format, args);
        }

        public void Fatal(object message)
        {
            Log.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            Log.Fatal(message, exception);

        }

        public void FatalFormat(string format, params object[] args)
        {
            Log.FatalFormat(format, args);
        }

        public void Info(object message, Exception exception)
        {
            Log.Info(message, exception);

        }

        public void Info(object message)
        {
            Log.Info(message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            Log.InfoFormat(format, args);
        }

        public void Warn(object message)
        {
            Log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            Log.Warn(message, exception);

        }

        public void WarnFormat(string format, params object[] args)
        {
            Log.WarnFormat(format, args);
        }
    }

    [Export(typeof(ILog),Key = "DataLog")]

    public class DataLog : DotLogImpl, ILog
    {
        public override log4net.ILog Log => LogManager.GetLogger(LogConfig.BaseRepository.Name,"DataLog") ;
    }

    [Export(typeof(ILog), Key = "BusinessLog")]

    public class BusinessLog : DotLogImpl, ILog
    {
        public override  log4net.ILog Log => LogManager.GetLogger(LogConfig.BaseRepository.Name, "BusinessLog") ;

    }

    [Export(typeof(ILog), Key = "BusinessExceptionLog")]

    public class BusinessExceptionLog : DotLogImpl, ILog
    {
        public override log4net.ILog Log => LogManager.GetLogger(LogConfig.BaseRepository.Name, "BusinessExceptionLog");

    }

    [Export(typeof(ILog), Key = "RunningLog")]

    public class RunningLog : DotLogImpl, ILog
    {
        public override log4net.ILog Log => LogManager.GetLogger(LogConfig.BaseRepository.Name, "RunningLog");

    }

    [Export(typeof(ILog), Key = "ExceptionLog")]

    public class ExceptionLog : DotLogImpl, ILog
    {
        public override log4net.ILog Log => LogManager.GetLogger(LogConfig.BaseRepository.Name, "ExceptionLog");

    }


    [Export(typeof(ILog), Key = "PlatformExceptionLog")]

    public class PlatformExceptionLog : DotLogImpl, ILog
    {
        public override log4net.ILog Log => LogManager.GetLogger(LogConfig.BaseRepository.Name, "PlatformExceptionLog") ;

    }

    [Export(typeof(ILog), Key = "PlatformLog")]

    public class PlatformLog : DotLogImpl, ILog
    {
        public override log4net.ILog Log => LogManager.GetLogger(LogConfig.BaseRepository.Name, "PlatformLog");

    }
}
