using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Log
{
    public class LogFactory
    {
        /// <summary>
        /// 数据操作日志
        /// </summary>
        public static readonly ILog DataLog = LogManager.GetLogger("DataLog");

        
        /// <summary>
        /// 运行日志
        /// </summary>
        public static readonly ILog RunningLog = LogManager.GetLogger("RunningLog");

        /// <summary>
        /// 平台日志
        /// </summary>
        public static readonly ILog PlatformLog = LogManager.GetLogger("PlatformLog");

        /// <summary>
        /// 业务运行日志
        /// </summary>
        public static readonly ILog BusinessLog = LogManager.GetLogger("BusinessLog");

        /// <summary>
        /// 不可预期的异常日志
        /// </summary>
        public static readonly ILog ExceptionLog = LogManager.GetLogger("ExceptionLog");

        /// <summary>
        /// 平台异常日志
        /// </summary>
        public static readonly ILog PlatformExceptionLog = LogManager.GetLogger("PlatformExceptionLog");

        /// <summary>
        /// 业务异常日志
        /// </summary>
        public static readonly ILog BusinessExceptionLog = LogManager.GetLogger("BusinessExceptionLog");

        #region WebLog

        /// <summary>
        /// 数据操作日志
        /// </summary>
        public static readonly IWebLog WebDataLog = WebLogManager.GetLogger("DataLog");

        /// <summary>
        /// 运行日志
        /// </summary>
        public static readonly IWebLog WebRunningLog = WebLogManager.GetLogger("RunningLog");

        /// <summary>
        /// 平台日志
        /// </summary>
        public static readonly IWebLog WebPlatformLog = WebLogManager.GetLogger("PlatformLog");

        /// <summary>
        /// 业务运行日志
        /// </summary>
        public static readonly IWebLog WebBusinessLog = WebLogManager.GetLogger("BusinessLog");

        /// <summary>
        /// 不可预期的异常日志
        /// </summary>
        public static readonly IWebLog WebExceptionLog = WebLogManager.GetLogger("ExceptionLog");

        /// <summary>
        /// 平台异常日志
        /// </summary>
        public static readonly IWebLog WebPlatformExceptionLog = WebLogManager.GetLogger("PlatformExceptionLog");

        /// <summary>
        /// 业务异常日志
        /// </summary>
        public static readonly IWebLog WebBusinessExceptionLog = WebLogManager.GetLogger("BusinessExceptionLog");
        #endregion

    }
}
