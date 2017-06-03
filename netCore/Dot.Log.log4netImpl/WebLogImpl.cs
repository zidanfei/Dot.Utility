using System;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Dot.Utility.Web;
using log4net.Core;
using Dot.Utility.Net;
using Dot.IOC;
using log4net;

namespace Dot.Log
{

    /// <summary>
    /// http://www.cnblogs.com/hb_cattle/articles/1560778.html
    /// </summary>
    public class WebLogImpl : LogImpl, IWebLog
    {

        /// <summary>
        /// The fully qualified name of this declaring type not the type of any subclass.
        /// </summary>
        private readonly static Type ThisDeclaringType = typeof(WebLogImpl);

        public WebLogImpl(ILogger logger) : base(logger)
        {
        }


        #region Implementation of IWebLog


        public void Debug(string log)
        {
            DebugDetail(new LogMessage(log), null);
        }

        public void DebugDetail(LogMessage log)
        {
            DebugDetail(log, null);
        }

        public void DebugDetail(LogMessage log, System.Exception ex)
        {
            if (this.IsInfoEnabled)
            {
                LoggingEvent loggingEvent = new LoggingEvent(ThisDeclaringType, Logger.Repository, LogConfig.BaseRepository.Name, Level.Info, log.Message, ex);
                SetLogMessage(loggingEvent, log);
                Logger.Log(loggingEvent);
            }
        }

        public void Info(string log)
        {
            InfoDetail(new LogMessage(log), null);
        }
        public void InfoDetail(LogMessage log)
        {
            InfoDetail(log, null);
        }

        public void InfoDetail(LogMessage log, System.Exception ex)
        {
            if (this.IsInfoEnabled)
            {
                LoggingEvent loggingEvent = new LoggingEvent(ThisDeclaringType, Logger.Repository, LogConfig.BaseRepository.Name, Level.Info, log.Message, ex);
                SetLogMessage(loggingEvent, log);
                Logger.Log(loggingEvent);
            }
        }

        public void Warn(string log)
        {
            WarnDetail(new LogMessage(log), null);
        }

        public void WarnDetail(LogMessage log)
        {
            WarnDetail(log, null);
        }

        public void WarnDetail(LogMessage log, System.Exception ex)
        {
            if (this.IsWarnEnabled)
            {
                LoggingEvent loggingEvent = new LoggingEvent(ThisDeclaringType, Logger.Repository, LogConfig.BaseRepository.Name, Level.Warn, log.Message, ex);
                SetLogMessage(loggingEvent, log);
                Logger.Log(loggingEvent);
            }
        }

        public void Error(string log)
        {
            ErrorDetail(new LogMessage(log), null);
        }

        public void ErrorDetail(LogMessage log)
        {
            ErrorDetail(log, null);
        }

        public void ErrorDetail(LogMessage log, System.Exception ex)
        {
            if (this.IsErrorEnabled)
            {
                LoggingEvent loggingEvent = new LoggingEvent(ThisDeclaringType, Logger.Repository, LogConfig.BaseRepository.Name, Level.Error, log.Message, ex);
                SetLogMessage(loggingEvent, log);
                Logger.Log(loggingEvent);
            }
        }

        public void Fatal(string log)
        {
            Fatal(new LogMessage(log), null);
        }

        public void Fatal(LogMessage log)
        {
            Fatal(log, null);
        }

        public void Fatal(LogMessage log, System.Exception ex)
        {
            if (this.IsFatalEnabled)
            {
                LoggingEvent loggingEvent = new LoggingEvent(ThisDeclaringType, Logger.Repository, LogConfig.BaseRepository.Name, Level.Fatal, log.Message, ex);
                SetLogMessage(loggingEvent, log);
                Logger.Log(loggingEvent);
            }
        }

        #endregion Implementation of IWebLog

        private void SetLogMessage(LoggingEvent loggingEvent, LogMessage log)
        {
            try
            {

                if (!string.IsNullOrEmpty(log.ClientIP))
                {
                    loggingEvent.Properties["ClientIP"] = log.ClientIP;
                }
                else
                {
                    //loggingEvent.Properties["ClientIP"] = Net.IPHelper.GetClientIp();
                }

            }
            catch (Exception ex)
            {
            }
            try
            {

                if (!string.IsNullOrEmpty(log.RequestUrl))
                {
                    loggingEvent.Properties["RequestUrl"] = log.RequestUrl;
                }
                else if (null != HttpContext.Current)
                {
                    //loggingEvent.Properties["RequestUrl"] = HttpContext.Current.Request.Url.AbsoluteUri;
                }
            }
            catch (Exception ex)
            {
            }
            try
            {

                if (!string.IsNullOrEmpty(log.ServerIP))
                {
                    loggingEvent.Properties["ServerIP"] = log.ServerIP;
                }
                else if (null != HttpContext.Current)
                {
                    loggingEvent.Properties["ServerIP"] = IPHelper.GetLocalIP();
                }
            }
            catch (Exception ex)
            {
            }

            try
            {

                if (!string.IsNullOrEmpty(log.UserId))
                {
                    loggingEvent.Properties["UserId"] = log.UserId;
                }
            }
            catch (Exception ex)
            {
            }

            try
            {

                if (!string.IsNullOrEmpty(log.UserName))
                {
                    loggingEvent.Properties["UserName"] = log.UserName;
                }
                else if (System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated)
                {
                    loggingEvent.Properties["UserName"] = System.Threading.Thread.CurrentPrincipal.Identity.Name;
                }
                else if (!string.IsNullOrEmpty(loggingEvent.UserName))
                {
                    loggingEvent.Properties["UserName"] = loggingEvent.UserName;
                }
            }
            catch (Exception ex)
            {
            }

            try
            {

                if (!string.IsNullOrEmpty(log.DisplayName))
                    loggingEvent.Properties["DisplayName"] = log.DisplayName;
            }
            catch (Exception ex)
            {
            }

            try
            {

                if (!string.IsNullOrEmpty(log.AreaName))
                    loggingEvent.Properties["AreaName"] = log.AreaName;
            }
            catch (Exception ex)
            {
            }
            try
            {

                if (!string.IsNullOrEmpty(log.ControllerName))
                    loggingEvent.Properties["ControllerName"] = log.ControllerName;
            }
            catch (Exception ex)
            {
            }
            try
            {

                if (!string.IsNullOrEmpty(log.ActionName))
                    loggingEvent.Properties["ActionName"] = log.ActionName;
            }
            catch (Exception ex)
            {
            }

            try
            {

                foreach (var item in log.ExtendPropety)
                {
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        loggingEvent.Properties[item.Key] = item.Value;

                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

    }

    public abstract class WebLogImplBase : DotLogImpl, IWebLog
    {
        public abstract IWebLog WebLog { get; }



        public void DebugDetail(LogMessage log)
        {
            WebLog.DebugDetail(log);
        }

        public void DebugDetail(LogMessage log, Exception ex)
        {
            WebLog.DebugDetail(log, ex);

        }

        public void ErrorDetail(LogMessage log)
        {
            WebLog.ErrorDetail(log);

        }

        public void ErrorDetail(LogMessage log, Exception ex)
        {
            WebLog.ErrorDetail(log, ex);

        }

        public void Fatal(LogMessage log)
        {
            WebLog.Fatal(log);

        }

        public void Fatal(LogMessage log, Exception ex)
        {
            WebLog.Fatal(log, ex);

        }

        public void InfoDetail(LogMessage log)
        {
            WebLog.InfoDetail(log);

        }

        public void InfoDetail(LogMessage log, Exception ex)
        {
            WebLog.InfoDetail(log, ex);

        }

        public void WarnDetail(LogMessage log)
        {
            WebLog.WarnDetail(log);

        }

        public void WarnDetail(LogMessage log, Exception ex)
        {
            WebLog.WarnDetail(log, ex);

        }
    }


    [Export(typeof(IWebLog), Key = "DataLog")]

    public class WebDataLog : WebLogImplBase, IWebLog
    {

        public override log4net.ILog Log => LogManager.GetLogger(LogConfig.BaseRepository.Name, "DataLog");

        public override IWebLog WebLog => WebLogManager.GetLogger("DataLog");

    }

    [Export(typeof(IWebLog), Key = "BusinessLog")]

    public class WebBusinessLog : WebLogImplBase, ILog
    {
        public override log4net.ILog Log => LogManager.GetLogger(LogConfig.BaseRepository.Name, "BusinessLog");
        public override IWebLog WebLog => WebLogManager.GetLogger("BusinessLog");

    }

    [Export(typeof(IWebLog), Key = "BusinessExceptionLog")]

    public class WebBusinessExceptionLog : WebLogImplBase, ILog
    {
        public override log4net.ILog Log => LogManager.GetLogger(LogConfig.BaseRepository.Name, "BusinessExceptionLog");
        public override IWebLog WebLog => WebLogManager.GetLogger("BusinessExceptionLog");

    }

    [Export(typeof(IWebLog), Key = "RunningLog")]

    public class WebRunningLog : WebLogImplBase, ILog
    {
        public override log4net.ILog Log => LogManager.GetLogger(LogConfig.BaseRepository.Name, "RunningLog");
        public override IWebLog WebLog => WebLogManager.GetLogger("RunningLog");

    }

    [Export(typeof(IWebLog), Key = "ExceptionLog")]

    public class WebExceptionLog : WebLogImplBase, ILog
    {
        public override log4net.ILog Log => LogManager.GetLogger(LogConfig.BaseRepository.Name, "ExceptionLog");
        public override IWebLog WebLog => WebLogManager.GetLogger("ExceptionLog");

    }


    [Export(typeof(IWebLog), Key = "PlatformExceptionLog")]

    public class WebPlatformExceptionLog : WebLogImplBase, ILog
    {
        public override log4net.ILog Log => LogManager.GetLogger(LogConfig.BaseRepository.Name, "PlatformExceptionLog");
        public override IWebLog WebLog => WebLogManager.GetLogger("PlatformExceptionLog");

    }

    [Export(typeof(IWebLog), Key = "PlatformLog")]

    public class WebPlatformLog : WebLogImplBase, ILog
    {
        public override log4net.ILog Log => LogManager.GetLogger(LogConfig.BaseRepository.Name, "PlatformLog");
        public override IWebLog WebLog => WebLogManager.GetLogger("PlatformLog");

    }
}
