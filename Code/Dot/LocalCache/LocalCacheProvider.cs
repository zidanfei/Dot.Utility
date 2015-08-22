using Dot.Cache;
using Dot.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.LocalCache
{
    [Export(typeof(ICacheProvider))]
    public class LocalCacheProvider : ICacheProvider
    {
        private string defaultContext = "default";

        public void Add(string key, string valKey, object value)
        {
            var tempKey = defaultContext;
            CacheManager.AddOrUpdate(tempKey, key, value, DateTime.Now.AddMinutes(2));
        }

        public void Put(string key, string valKey, object value)
        {
            var tempKey = defaultContext;
            CacheManager.Update(tempKey, key, value);
        }

        public object Get(string key, string valKey)
        {
            var tempKey = defaultContext;
            return CacheManager.Get(tempKey, key);
        }

        public void Remove(string key)
        {
            CacheManager.Remove(defaultContext, key);
        }

        public bool Exists(string key)
        {
            return CacheManager.Contains(defaultContext, key);
        }

        public bool Exists(string key, string valKey)
        {
            var tempKey = defaultContext;
            return CacheManager.Contains(tempKey, key);
        }
    }
}
