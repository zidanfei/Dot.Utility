
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dot.IOC;

namespace Dot.Log
{
    public class LogFactory
    {


        /// <summary>
        /// 数据操作日志
        /// </summary>
        public static ILog DataLog
        {
            get
            {
                return ObjectContainerFactory.CreateContainer().Resolve<ILog>("DataLog");
            }
        }

        /// <summary>
        /// 运行日志
        /// </summary>
        public static ILog RunningLog
        {
            get
            {
                return ObjectContainerFactory.CreateContainer().Resolve<ILog>("RunningLog");
            }
        }

        /// <summary>
        /// 平台日志
        /// </summary>
        public static ILog PlatformLog
        {
            get
            {
                return ObjectContainerFactory.CreateContainer().Resolve<ILog>("PlatformLog");
            }
        }

        /// <summary>
        /// 业务运行日志
        /// </summary>
        public static ILog BusinessLog
        {
            get
            {
                return ObjectContainerFactory.CreateContainer().Resolve<ILog>("BusinessLog");
            }
        }

        /// <summary>
        /// 不可预期的异常日志
        /// </summary>
        public static ILog ExceptionLog
        {
            get
            {
                return ObjectContainerFactory.CreateContainer().Resolve<ILog>("ExceptionLog");
            }
        }

        /// <summary>
        /// 平台异常日志
        /// </summary>
        public static ILog PlatformExceptionLog
        {
            get
            {
                return ObjectContainerFactory.CreateContainer().Resolve<ILog>("PlatformExceptionLog");
            }
        }

        /// <summary>
        /// 业务异常日志
        /// </summary>
        public static ILog BusinessExceptionLog
        {
            get
            {
                return ObjectContainerFactory.CreateContainer().Resolve<ILog>("BusinessExceptionLog");
            }
        }

        #region WebLog

        /// <summary>
        /// 数据操作日志
        /// </summary>
        public static IWebLog WebDataLog
        {
            get
            {
                return ObjectContainerFactory.CreateContainer().Resolve<IWebLog>("DataLog");
            }
        }

        /// <summary>
        /// 运行日志
        /// </summary>
        public static IWebLog WebRunningLog
        {
            get
            {
                return ObjectContainerFactory.CreateContainer().Resolve<IWebLog>("RunningLog");
            }
        }

        /// <summary>
        /// 平台日志
        /// </summary>
        public static IWebLog WebPlatformLog
        {
            get
            {
                return ObjectContainerFactory.CreateContainer().Resolve<IWebLog>("PlatformLog");
            }
        }

        /// <summary>
        /// 业务运行日志
        /// </summary>
        public static IWebLog WebBusinessLog
        {
            get
            {
                return ObjectContainerFactory.CreateContainer().Resolve<IWebLog>("BusinessLog");
            }
        }

        /// <summary>
        /// 不可预期的异常日志
        /// </summary>
        public static IWebLog WebExceptionLog
        {
            get
            {
                return ObjectContainerFactory.CreateContainer().Resolve<IWebLog>("ExceptionLog");
            }
        }

        /// <summary>
        /// 平台异常日志
        /// </summary>
        public static IWebLog WebPlatformExceptionLog
        {
            get
            {
                return ObjectContainerFactory.CreateContainer().Resolve<IWebLog>("PlatformExceptionLog");
            }
        }

        /// <summary>
        /// 业务异常日志
        /// </summary>
        public static IWebLog WebBusinessExceptionLog
        {
            get
            {
                return ObjectContainerFactory.CreateContainer().Resolve<IWebLog>("BusinessExceptionLog");
            }
        }
        #endregion

    }
}
