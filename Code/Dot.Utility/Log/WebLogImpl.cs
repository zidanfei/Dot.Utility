using System;

using log4net.Core;
using System.ComponentModel.DataAnnotations;

namespace Dot.Utility.Log
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

        public WebLogImpl(ILogger logger)
            : base(logger)
        {
        }

        #region Implementation of IWebLog


        public void Debug(LogMessage log)
        {
            Debug(log, null);
        }

        public void Debug(LogMessage log, System.Exception ex)
        {
            if (this.IsInfoEnabled)
            {
                LoggingEvent loggingEvent = new LoggingEvent(ThisDeclaringType, Logger.Repository, Logger.Name, Level.Info, log.Message, ex);
                SetLogMessage(loggingEvent, log);
                Logger.Log(loggingEvent);
            }
        }


        public void Info(LogMessage log)
        {
            Info(log, null);
        }

        public void Info(LogMessage log, System.Exception ex)
        {
            if (this.IsInfoEnabled)
            {
                LoggingEvent loggingEvent = new LoggingEvent(ThisDeclaringType, Logger.Repository, Logger.Name, Level.Info, log.Message, ex);
                SetLogMessage(loggingEvent, log);              
                Logger.Log(loggingEvent);
            }
        }

        public void Warn(LogMessage log)
        {
            Warn(log, null);
        }

        public void Warn(LogMessage log, System.Exception ex)
        {
            if (this.IsWarnEnabled)
            {
                LoggingEvent loggingEvent = new LoggingEvent(ThisDeclaringType, Logger.Repository, Logger.Name, Level.Warn, log.Message, ex);
                SetLogMessage(loggingEvent, log);
                Logger.Log(loggingEvent);
            }
        }

        public void Error(LogMessage log)
        {
            Error(log, null);
        }

        public void Error(LogMessage log, System.Exception ex)
        {
            if (this.IsErrorEnabled)
            {
                LoggingEvent loggingEvent = new LoggingEvent(ThisDeclaringType, Logger.Repository, Logger.Name, Level.Error, log.Message, ex);
                SetLogMessage(loggingEvent, log);
                Logger.Log(loggingEvent);
            }
        }

        public void Fatal(LogMessage log)
        {
            Fatal(log, null);
        }

        public void Fatal(LogMessage log, System.Exception ex)
        {
            if (this.IsFatalEnabled)
            {
                LoggingEvent loggingEvent = new LoggingEvent(ThisDeclaringType, Logger.Repository, Logger.Name, Level.Fatal, log.Message, ex);
                SetLogMessage(loggingEvent, log);
                Logger.Log(loggingEvent);
            }
        }

        #endregion Implementation of IWebLog

        private void SetLogMessage(LoggingEvent loggingEvent, LogMessage log)
        {
            if (!string.IsNullOrEmpty(log.ClientIP))
            {
                loggingEvent.Properties["ClientIP"] = log.ClientIP;
            }
            else
            {
                loggingEvent.Properties["ClientIP"] = Net.IPHelper.GetClientIp();
            }
            if (!string.IsNullOrEmpty(log.RequestUrl))
            {
                loggingEvent.Properties["RequestUrl"] = log.RequestUrl;
            }
            else if (null != System.Web.HttpContext.Current)
            {
                loggingEvent.Properties["RequestUrl"] = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            }
            if (!string.IsNullOrEmpty(log.ServerIP))
            {
                loggingEvent.Properties["ServerIP"] = log.ServerIP;
            }
            else if (null != System.Web.HttpContext.Current)
            {
                loggingEvent.Properties["ServerIP"] = Net.IPHelper.GetLocalIP();
            }
            if (!string.IsNullOrEmpty(log.UserId))
            {
                loggingEvent.Properties["UserId"] = log.UserId;
            }

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
            if (!string.IsNullOrEmpty(log.DisplayName))
                loggingEvent.Properties["DisplayName"] = log.DisplayName;
            if (!string.IsNullOrEmpty(log.AreaName))
                loggingEvent.Properties["AreaName"] = log.AreaName;
            if (!string.IsNullOrEmpty(log.ControllerName))
                loggingEvent.Properties["ControllerName"] = log.ControllerName;
            if (!string.IsNullOrEmpty(log.ActionName))
                loggingEvent.Properties["ActionName"] = log.ActionName;

            foreach (var item in log.ExtendPropety)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    loggingEvent.Properties[item.Key] = item.Value;

                }
            }
        }

    }


}
