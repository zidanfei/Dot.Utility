using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Dot.AOP.Interceptors
{
    /// <summary>
    /// 方法调用基础拦截器
    /// </summary>
    public abstract class BaseInterceptionBehavior : IInterceptionBehavior
    {
        private System.Threading.AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        private string requestID = string.Empty;
        //public BaseInterceptionBehavior(Type[] types)
        //{
        //    this.currentTypes = types;
        //}

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            //检查参数是否存在
            if (input == null)
                throw new ArgumentNullException("input");
            if (getNext == null)
                throw new ArgumentNullException("getNext");

            IMethodReturn methodReturn = null;

            if (this.IsIntercption(input))
            {
                Debug.WriteLine("开始准备拦截调用");
                this.BeforeInvoke(input);

                Debug.WriteLine("开始执行调用");
                methodReturn = this.ExcuteInvoke(input, getNext);

                Debug.WriteLine("完成方法执行后调用");
                this.AfterInvoke(input);

                #region 组装-拦截消息IntercptionMessage
                //IntercptionMessage message = new IntercptionMessage();
                //message.Method = input.MethodBase as System.Reflection.MethodInfo;

                //message.RealClassType = input.Target.GetType();
                //System.Reflection.ParameterInfo[] parameterInfos = input.MethodBase.GetParameters();

                //if (parameterInfos != null)
                //{
                //    message.Parameters = new Dictionary<int, object>();

                //    int i = 0;
                //    foreach (var parameterInfo in parameterInfos)
                //    {
                //        i++;
                //        message.Parameters.Add(i, input.Arguments[parameterInfo.Name]);
                //    }
                //}

                //requestID = Guid.NewGuid().ToString();

                ////添加请求标识
                //message.Parameters.Add(message.Parameters.Count + 1, requestID);
                //message.Parameters.Add(message.Parameters.Count + 1, message.Method.ReturnType.FullName);
                #endregion

                //methodReturn= input.CreateMethodReturn(methodCallResult);
            }
            else
            {
                methodReturn= getNext()(input, getNext);
            }
            return methodReturn;
        }

        public bool WillExecute
        {
            get { return true; }
        }

        /// <summary>
        /// 判定是否执行拦截
        /// </summary>
        /// <param name="methodInvocation">方法调用信息</param>
        /// <returns></returns>
        public abstract bool IsIntercption(IMethodInvocation methodInvocation);

        protected abstract void BeforeInvoke(IMethodInvocation methodInvocation);

        protected abstract void AfterInvoke(IMethodInvocation methodInvocation);

        protected virtual IMethodReturn ExcuteInvoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            return getNext()(input, getNext);
        }
    }

    /// <summary>
    /// 拦截的方法调用的详细信息-将方法调用转换为消息
    /// </summary>
    public class IntercptionMessage
    {
        public Type RealClassType { get; set; }

        public System.Reflection.MethodInfo
            Method { get; set; }

        /// <summary>
        /// 方法参数列表-Key:参数类型,value-参数值
        /// </summary>
        public IDictionary<int, object> Parameters { get; set; }
    }
}
