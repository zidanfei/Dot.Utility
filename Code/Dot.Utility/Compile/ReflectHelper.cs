using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Compile
{
    public class ReflectHelper
    {
        /// <summary>
        /// 根据对象实例获取对象内部的属性信息
        /// </summary>
        /// <param name="unityProxyObject">对象实例</param>
        /// <param name="propertyName">对象属性名称-支持私有变量</param>
        /// <returns></returns>
        public static object GetInstanceProperty(object unityProxyObject, string propertyName)
        {
            MemberInfo[] minss = unityProxyObject.GetType().GetMembers(BindingFlags.CreateInstance |
                                                   BindingFlags.Static |
                                                  BindingFlags.NonPublic | BindingFlags.GetField
                                                  | BindingFlags.GetProperty | BindingFlags.Instance
                                                  | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty | BindingFlags.FlattenHierarchy);

            //tempService.GetType().InvokeMember("get_ClassName", BindingFlags.InvokeMethod, null, Tb, null, null, null, null).ToString();
            if (minss != null && minss.Length > 0)
            {
                var tempMemberInfo = minss.Where(pre => pre.Name == propertyName).FirstOrDefault();
                var tempFieldInfo = tempMemberInfo as FieldInfo;
                var tempValue = tempFieldInfo.GetValue(unityProxyObject);
                return tempValue;
            }

            return null;
        }

        /// <summary>
        /// 克隆对象
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static object DeepClone(object instance)
        {
            return instance;
        }

        #region Lamda表达式-反射支持--高性能

        public Func<object, object> LmdGet(Type entityType, string propName)
        {
            #region 通过方法取值
            var p = entityType.GetProperty(propName);
            //对象实例
            var param_obj = Expression.Parameter(typeof(object), "obj");
            //值
            //var param_val = Expression.Parameter(typeof(object), "val");
            //转换参数为真实类型
            var body_obj = Expression.Convert(param_obj, entityType);

            //调用获取属性的方法
            var body = Expression.Call(body_obj, p.GetGetMethod());
            return Expression.Lambda<Func<object, object>>(body, param_obj).Compile();
            #endregion

            #region 表达式取值
            //var p = entityType.GetProperty(propName);
            ////lambda的参数u
            //var param_u = Expression.Parameter(entityType, "u");
            ////lambda的方法体 u.Age
            //var pGetter = Expression.Property(param_u, p);
            ////编译lambda
            //LmdGetProp = Expression.Lambda<Func<TestData, int>>(pGetter, param_u).Compile();
            #endregion
        }

        public Action<object, object> LmdSet(Type entityType, string propName)
        {
            var p = entityType.GetProperty(propName);
            //对象实例
            var param_obj = Expression.Parameter(typeof(object), "obj");
            //值
            var param_val = Expression.Parameter(typeof(object), "val");
            //转换参数为真实类型
            var body_obj = Expression.Convert(param_obj, entityType);
            var body_val = Expression.Convert(param_val, p.PropertyType);
            //调用给属性赋值的方法
            var body = Expression.Call(body_obj, p.GetSetMethod(), body_val);
            return Expression.Lambda<Action<object, object>>(body, param_obj, param_val).Compile();
        }

        #endregion

        #region Emit表达式-反射支持--高性能

        public delegate void SetValueDelegateHandler(object entity, object value);
        public SetValueDelegateHandler EmitSetValue;
        public void SetPropertyValueEmit(Type entityType, string propertyName)
        {
            //Type entityType = entity.GetType();
            Type parmType = typeof(object);
            // 指定函数名
            string methodName = "set_" + propertyName;
            // 搜索函数，不区分大小写 IgnoreCase
            var callMethod = entityType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic);
            // 获取参数
            var para = callMethod.GetParameters()[0];
            // 创建动态函数
            DynamicMethod method = new DynamicMethod("EmitCallable", null, new Type[] { entityType, parmType }, entityType.Module);
            // 获取动态函数的 IL 生成器
            var il = method.GetILGenerator();
            // 创建一个本地变量，主要用于 Object Type to Propety Type
            var local = il.DeclareLocal(para.ParameterType, true);
            // 加载第 2 个参数【(T owner, object value)】的 value
            il.Emit(OpCodes.Ldarg_1);
            if (para.ParameterType.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, para.ParameterType);// 如果是值类型，拆箱 string = (string)object;
            }
            else
            {
                il.Emit(OpCodes.Castclass, para.ParameterType);// 如果是引用类型，转换 Class = object as Class
            }
            il.Emit(OpCodes.Stloc, local);// 将上面的拆箱或转换，赋值到本地变量，现在这个本地变量是一个与目标函数相同数据类型的字段了。
            il.Emit(OpCodes.Ldarg_0); // 加载第一个参数 owner
            il.Emit(OpCodes.Ldloc, local);// 加载本地参数
            il.EmitCall(OpCodes.Callvirt, callMethod, null);//调用函数
            il.Emit(OpCodes.Ret); // 返回
            /* 生成的动态函数类似：
            * void EmitCallable(T owner, object value)
            * {
            * T local = (T)value;
            * owner.Method(local);
            * }
            */

            EmitSetValue = method.CreateDelegate(typeof(SetValueDelegateHandler)) as SetValueDelegateHandler;
        }

        public static object GetPropertyValueEmit(object entity, PropertyInfo property)
        {
            DynamicMethod method = new DynamicMethod("GetValue", property.PropertyType, new Type[] { entity.GetType() });

            ILGenerator ilGenerator = method.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.EmitCall(OpCodes.Callvirt, property.GetGetMethod(), null);
            ilGenerator.Emit(OpCodes.Ret);

            method.DefineParameter(1, ParameterAttributes.In, "target");
            var getFunc = (Func<object, object>)method.CreateDelegate(typeof(Func<object, object>));
            return getFunc(entity);
        }

        #endregion

        #region 复制对象

        /// <summary>
        /// 深复制对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="RealObject">待复制的对象</param>
        /// <returns></returns>
        public static T Clone<T>(T RealObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                //利用 System.Runtime.Serialization序列化与反序列化完成引用对象的复制  
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }

        #endregion
    }
}
