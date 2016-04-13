using Dot.Utility.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Log
{
    public class LogMessage// : EntityBaseWithId
    {
        public LogMessage()
        {
            ExtendPropety = new Dictionary<string, string>();
        }
        public LogMessage(string message) : this()
        {
            Message = message;
        }
        /// <summary>
        /// 客户端Ip
        /// </summary>
        [Display(Name = "客户端Ip")]
        public string ClientIP { get; set; }

        /// <summary>
        /// 服务器Ip
        /// </summary>
        [Display(Name = "服务器Ip")]
        public string ServerIP { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        [Display(Name = "请求地址")]
        public string RequestUrl { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Display(Name = "用户ID")]
        public string UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        /// <summary>
        /// 显示名
        /// </summary>
        [Display(Name = "显示名")]
        public string DisplayName { get; set; }

        /// <summary>
        /// 日志记录
        /// </summary>
        [Display(Name = "日志记录")]
        public string Message { get; set; }

        /// <summary>
        /// Area
        /// </summary>
        [Display(Name = "Area")]
        public string AreaName { get; set; }

        /// <summary>
        /// 控制器
        /// </summary>
        [Display(Name = "Controller")]
        public string ControllerName { get; set; }

        /// <summary>
        /// 行为
        /// </summary>
        [Display(Name = "Action")]
        public string ActionName { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        public Dictionary<string, string> ExtendPropety { get; set; }



        public LogMessage Clone()
        {
            LogMessage log = MemberwiseClone() as LogMessage;
            return log;
        }

    }

    public class DBLogMessage : LogMessage
    {
        public DateTime? Date { get; set; }
        public string Msg { get; set; }
        public string Logger { get; set; }
        public string Level { get; set; }
        public string Thread { get; set; }
        public string Exception { get; set; }
        public long? RunTime { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public int? LineNumber { get; set; }
    }
}
