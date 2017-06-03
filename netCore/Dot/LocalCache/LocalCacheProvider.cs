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
        public LocalCacheProvider()
        {

        }

        public void Add(string key, string valKey, object value)
        {
            LocalCacheManager.AddOrUpdate(key, valKey, value, DateTime.Now.AddMinutes(2));
        }

        public void Put(string key, string valKey, object value)
        {
            LocalCacheManager.Update(key, valKey, value);
        }

        public object Get(string key, string valKey)
        {
            return LocalCacheManager.Get(key, valKey);
        }

        public void Remove(string key, string valKey)
        {
            LocalCacheManager.Remove(key, valKey);
        }

        //public bool Exists(string key)
        //{
        //    return CacheManager.Contains(key,"");
        //}

        public bool Exists(string key, string valKey)
        {
            return LocalCacheManager.Contains(key, valKey);
        }
    }
}
