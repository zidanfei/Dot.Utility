using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;

namespace Dot.IOC
{
    public class ObjectContainer
    {
        public static T CreateInstance<T>()
            where T : class
        {
            if (IsRegistered(typeof(T)))
            {
                return Composer.ObjectContainer.Resolve<T>();
            }
            return default(T);
        }

        public static T CreateInstance<T>(string name)
            where T : class
        {
            if (IsRegistered(typeof(T), name))
            {
                return Composer.ObjectContainer.Resolve<T>(name);
            }
            return default(T);

        }

        public static object CreateInstance(Type type)
        {
            if (IsRegistered(type))
            {
                return Composer.ObjectContainer.Resolve(type);
            }
            return null;
        }

        public static object CreateInstance(Type type, string name)
        {
            if (IsRegistered(type, name))
            {
                return Composer.ObjectContainer.Resolve(type, name);
            }
            return null;
        }

        public static void RegisterType(Type from, Type to, string key = null)
        {
            Composer.ObjectContainer.RegisterType(from, to, key);
        }

        public static void RegisterType<TFrom, TTo>(string key = null) where TTo : TFrom
        {
            Composer.ObjectContainer.RegisterType<TFrom, TTo>(key);
        }

        public static bool IsRegistered(Type interfaceType, string keyName)
        {
            var unitContainer = UnityAdapterHelper.GetUnityContainer(Composer.ObjectContainer);
            return unitContainer.IsRegistered(interfaceType, keyName);
        }

        public static bool IsRegistered(Type interfaceType)
        {
            var unitContainer = UnityAdapterHelper.GetUnityContainer(Composer.ObjectContainer);
            return unitContainer.IsRegistered(interfaceType);
        }
    }
}