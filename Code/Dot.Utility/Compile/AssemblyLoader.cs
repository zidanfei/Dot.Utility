using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Dot.Utility.Compile
{
    /// <summary>
    /// 根据程序集获取数据的工具类
    /// </summary>
    public class AssemblyLoader
    {
        /// <summary>
        /// 根据程序集文件获取具有指定名称的类的实例. 若未能找到该类或无法创建, 则返回Null.
        /// </summary>
        /// <param name="assemblyPath">程序集文件物理地址</param>
        /// <param name="className">要搜索的类的全名</param>
        /// <returns>得到的实例</returns>
        public static Object GetObjectByAssembly(String assemblyPath, String className)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(assemblyPath);

                Object obj = assembly.CreateInstance(className, true);

                return obj;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 根据给定的类型在指定程序集中搜索所有该类型的子类, 并创建实例.
        /// </summary>
        /// <typeparam name="T">给定的类型</typeparam>
        /// <param name="assemblyPath">程序集物理路径</param>
        /// <param name="ignoreTypes">指定的类型将不包含在返回的实例集合中</param>
        /// <returns>实例</returns>
        public static T[] GetImplementations<T>(String path, params Type[] ignoreTypes)
        {
            return GetImplementations<T>(Assembly.LoadFrom(path), ignoreTypes);
        }

        /// <summary>
        /// 根据给定的类型在指定程序集中搜索所有该类型的子类, 并创建实例.
        /// </summary>
        /// <typeparam name="T">给定的类型</typeparam>
        /// <param name="assemblyPath">程序集</param>
        /// <param name="ignoreTypes">指定的类型将不包含在返回的实例集合中</param>
        /// <returns>实例</returns>
        public static T[] GetImplementations<T>(Assembly assembly, params Type[] ignoreTypes)
        {
            Type typeOfParam = typeof(T);

            if (typeOfParam.IsEnum)
                return new T[0];

            Boolean isInterface = typeOfParam.IsInterface;

            try
            {

                //Assembly assembly = Assembly.LoadFrom(assemblyPath);


                List<Type> types = new List<Type>();
                Module[] modules = assembly.GetModules(false);


                foreach (Module m in modules)
                {
                    Type[] t = m.GetTypes();

                    foreach (Type i in t)
                        if (IsImplemented(i, typeOfParam))
                            types.Add(i);
                }



                List<T> objs = new List<T>();

                foreach (Type type in types)
                {
                    try
                    {
                        if (ignoreTypes.Contains<Type>(type))
                            continue;

                        if (ignoreTypes.AsQueryable<Type>().Where<Type>((typ, boo) => typ.IsSubclassOf(typ)).Count<Type>() > 0)
                            continue;

                        T o = (T)type.Assembly.CreateInstance(type.FullName);

                        objs.Add(o);
                    }
                    catch
                    {
                    }
                }

                return objs.ToArray();

            }
            catch
            {
                return new T[0];
            }
        }

        /// <summary>
        /// 获取指定的类型A是否继承于类型B
        /// </summary>
        /// <param name="toTest">要测试的类型A</param>
        /// <param name="t">要测试的类型A</param>
        /// <returns>类型A是否继承于类型B</returns>
        public static Boolean IsImplemented(Type a, Type b)
        {
            if (a == null || b == null)
                return false;

            if (b.IsInterface)
                return a.GetInterfaces().Contains<Type>(b);
            else
                return a.IsSubclassOf(b);
        }

        public static T[] GetCustomAttributes<T>(Type type) where T : Attribute
        {
            if (type == null)
                return default(T[]);

            return (T[])type.GetCustomAttributes(typeof(T), false);
        }

        public static T GetCustomAttribute<T>(Type type) where T : Attribute
        {
            if (type == null)
                return default(T);

            return (T)type.GetCustomAttribute(typeof(T), false);
        } 
    }
}
