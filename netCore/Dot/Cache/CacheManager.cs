using System;
using Dot.IOC;
using System.Collections.Generic;

namespace Dot.Cache
{
    /// <summary>
    /// 表示向应用系统提供缓存功能的缓存管理器。
    /// </summary>
    public sealed class CacheManager
    {
        #region Private Fields

        private static readonly ICacheProvider instance = ObjectContainer.CreateInstance<ICacheProvider>();
        private static readonly IDictionary<string, ICacheProvider> instances = new Dictionary<string, ICacheProvider>();
        private static object _cacheLock = new object();

        #endregion

        #region Ctor
        static CacheManager() { }

        private CacheManager()
        {
            //cacheProvider = Composer.ObjectContainer.Resolve<ICacheProvider>();
            //cacheProvider = ObjectContainer.CreateInstance<ICacheProvider>();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// 获取<c>CacheManager</c>类型的单件（Singleton）实例。
        /// </summary>
        public static ICacheProvider Cache
        {
            get { return instance; }
        }
        #endregion

        #region Public method

        public static ICacheProvider GetCache(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return instance;
            }
            if (!instances.ContainsKey(key))
            {
                lock (_cacheLock)
                {
                    if (!instances.ContainsKey(key))
                    {
                        instances.Add(key, ObjectContainer.CreateInstance<ICacheProvider>(key));
                    }
                }
            }
            return instances[key];
        }

        #endregion

    }
}
