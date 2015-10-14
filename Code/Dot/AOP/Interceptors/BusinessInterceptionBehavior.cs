using Dot.Utility;
using Dot.IOC;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Diagnostics;
using Dot.Utility.Log;
using Dot.Utility.Net;

namespace Dot.AOP.Interceptors
{
    /// <summary>
    /// 表示用于业务日志记录的拦截器。
    /// </summary>
    public class BusinessLoggingBehavior : IInterceptionBehavior
    {
        public BusinessLoggingBehavior()
        {
            this.LogDetail = true;
        }

        /// <summary>
        /// 是否注册完整的日志。默认为 true。
        /// </summary>
        public bool LogDetail { get; set; }

        #region IInterceptionBehavior Members

        /// <summary>
        /// 通过实现此方法来拦截调用并执行所需的拦截行为。
        /// </summary>
        /// <param name="input">调用拦截目标时的输入信息。</param>
        /// <param name="getNext">通过行为链来获取下一个拦截行为的委托。</param>
        /// <returns>从拦截目标获得的返回信息。</returns>
        [DebuggerStepThrough]
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            DateTime start = DateTime.Now;
            var methodReturn = getNext().Invoke(input, getNext);
            try
            {
                TimeSpan duration = DateTime.Now - start;
                var logMessage = "Running time:" + duration.TotalSeconds +
                    ";  IP:" + IPHelper.GetClientIp() + "; " +
                    input.Target.ToString() + "-" + input.MethodBase.Name + ";  ";

                if (this.LogDetail)
                {
                    StringBuilder sb = new StringBuilder(255);
                    if (input.Arguments.Count > 0)
                    {
                        foreach (var item in input.Arguments)
                        {
                            if (item is IList)
                            {
                                var list = item as IList;
                                foreach (var detailitem in list)
                                {
                                    sb.AppendFormat("{0};", detailitem);
                                }
                            }
                            else
                            {
                                sb.AppendFormat("{0};", item);
                            }
                        }
                    }
                    logMessage += "Arguments:" + sb;
                }

                LogFactory.BusinessLog.Info(logMessage);
            }
            catch (Exception ex)
            {
                string exMessage = "记录业务日志出错";

                LogFactory.ExceptionLog.Fatal(exMessage, ex);
            }
            return methodReturn;
        }

        /// <summary>
        /// 获取一个<see cref="Boolean"/>值，该值表示当前拦截行为被调用时，是否真的需要执行
        /// 某些操作。
        /// </summary>
        public bool WillExecute
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 获取当前行为需要拦截的对象类型接口。
        /// </summary>
        /// <returns>所有需要拦截的对象类型接口。</returns>
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        #endregion
    }
}
