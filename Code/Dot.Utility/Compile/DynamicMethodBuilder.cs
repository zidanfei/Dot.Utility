/*
 * Author:hot
 */
using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Dot.Utility.Compile
{
    public static class DynamicMethodBuilder
    {
        #region Instance

        public static Action<TInstance> BuildAction<TInstance>(MethodInfo methodInfo)
        {
            return (Action<TInstance>)BuildDynamicDelegate(methodInfo);
        }

        public static Action<TInstance, T1> BuildAction<TInstance, T1>(MethodInfo methodInfo)
        {
            return (Action<TInstance, T1>)BuildDynamicDelegate(methodInfo);
        }

        public static Action<TInstance, T1, T2> BuildAction<TInstance, T1, T2>(MethodInfo methodInfo)
        {
            return (Action<TInstance, T1, T2>)BuildDynamicDelegate(methodInfo);
        }

        public static Action<TInstance, T1, T2, T3> BuildAction<TInstance, T1, T2, T3>(MethodInfo methodInfo)
        {
            return (Action<TInstance, T1, T2, T3>)BuildDynamicDelegate(methodInfo);
        }

        public static Action<TInstance, T1, T2, T3, T4> BuildAction<TInstance, T1, T2, T3, T4>(MethodInfo methodInfo)
        {
            return (Action<TInstance, T1, T2, T3, T4>)BuildDynamicDelegate(methodInfo);
        }

        #endregion

        #region Static

        public static Action BuildStaticAction(MethodInfo methodInfo)
        {
            return (Action)BuildDynamicDelegate(methodInfo);
        }

        public static Action<T1> BuildStaticAction<T1>(MethodInfo methodInfo)
        {
            return (Action<T1>)BuildDynamicDelegate(methodInfo);
        }

        public static Action<T1, T2> BuildStaticAction<T1, T2>(MethodInfo methodInfo)
        {
            return (Action<T1, T2>)BuildDynamicDelegate(methodInfo);
        }

        public static Action<T1, T2, T3> BuildStaticAction<T1, T2, T3>(MethodInfo methodInfo)
        {
            return (Action<T1, T2, T3>)BuildDynamicDelegate(methodInfo);
        }

        public static Action<T1, T2, T3, T4> BuildStaticAction<T1, T2, T3, T4>(MethodInfo methodInfo)
        {
            return (Action<T1, T2, T3, T4>)BuildDynamicDelegate(methodInfo);
        }

        #endregion

        #region Instance Func

        public static Func<TInstance, TReturn> BuildFunc<TInstance, TReturn>(MethodInfo methodInfo)
        {
            return (Func<TInstance, TReturn>)BuildDynamicDelegate(methodInfo);
        }

        public static Func<TInstance, T1, TReturn> BuildFunc<TInstance, T1, TReturn>(MethodInfo methodInfo)
        {
            return (Func<TInstance, T1, TReturn>)BuildDynamicDelegate(methodInfo);
        }

        public static Func<TInstance, T1, T2, TReturn> BuildFunc<TInstance, T1, T2, TReturn>(MethodInfo methodInfo)
        {
            return (Func<TInstance, T1, T2, TReturn>)BuildDynamicDelegate(methodInfo);
        }

        public static Func<TInstance, T1, T2, T3, TReturn> BuildFunc<TInstance, T1, T2, T3, TReturn>(MethodInfo methodInfo)
        {
            return (Func<TInstance, T1, T2, T3, TReturn>)BuildDynamicDelegate(methodInfo);
        }

        public static Func<TInstance, T1, T2, T3, T4, TReturn> BuildFunc<TInstance, T1, T2, T3, T4, TReturn>(MethodInfo methodInfo)
        {
            return (Func<TInstance, T1, T2, T3, T4, TReturn>)BuildDynamicDelegate(methodInfo);
        }

        #endregion

        #region Static Func

        public static Func<TReturn> BuildStaticFunc<TReturn>(MethodInfo methodInfo)
        {
            return (Func<TReturn>)BuildDynamicDelegate(methodInfo);
        }

        public static Func<T1, TReturn> BuildStaticFunc<T1, TReturn>(MethodInfo methodInfo)
        {
            return (Func<T1, TReturn>)BuildDynamicDelegate(methodInfo);
        }

        public static Func<T1, T2, TReturn> BuildStaticFunc<T1, T2, TReturn>(MethodInfo methodInfo)
        {
            return (Func<T1, T2, TReturn>)BuildDynamicDelegate(methodInfo);
        }

        public static Func<T1, T2, T3, TReturn> BuildStaticFunc<T1, T2, T3, TReturn>(MethodInfo methodInfo)
        {
            return (Func<T1, T2, T3, TReturn>)BuildDynamicDelegate(methodInfo);
        }

        public static Func<T1, T2, T3, T4, TReturn> BuildStaticFunc<T1, T2, T3, T4, TReturn>(MethodInfo methodInfo)
        {
            return (Func<T1, T2, T3, T4, TReturn>)BuildDynamicDelegate(methodInfo);
        }

        #endregion

        /// <summary>
        /// 动态构造委托
        /// </summary>
        /// <param name="methodInfo">方法元数据</param>
        /// <returns>委托</returns>
        public static Delegate BuildDynamicDelegate(MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new ArgumentNullException("methodInfo");

            var paramExpressions = methodInfo.GetParameters().Select((p, i) =>
            {
                var name = "param" + (i + 1).ToString(CultureInfo.InvariantCulture);
                return Expression.Parameter(p.ParameterType, name);
            }).ToList();

            MethodCallExpression callExpression;
            if (methodInfo.IsStatic)
            {
                //Call(params....)
                callExpression = Expression.Call(methodInfo, paramExpressions);
            }
            else
            {
                var instanceExpression = Expression.Parameter(methodInfo.ReflectedType, "instance");
                //insatnce.Call(params)
                callExpression = Expression.Call(instanceExpression, methodInfo, paramExpressions);
                paramExpressions.Insert(0, instanceExpression);
            }
            var lambdaExpression = Expression.Lambda(callExpression, paramExpressions);
            return lambdaExpression.Compile();
        }
    }
}