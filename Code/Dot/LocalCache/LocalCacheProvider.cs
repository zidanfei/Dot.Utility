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
            CacheManager.AddOrUpdate(key, valKey, value, DateTime.Now.AddMinutes(2));
        }

        public void Put(string key, string valKey, object value)
        {
            CacheManager.Update(key, valKey, value);
        }

        public object Get(string key, string valKey)
        {
            return CacheManager.Get(key, valKey);
        }

        public void Remove(string key, string valKey)
        {
            CacheManager.Remove(key, valKey);
        }

        //public bool Exists(string key)
        //{
        //    return CacheManager.Contains(key,"");
        //}

        public bool Exists(string key, string valKey)
        {
            return CacheManager.Contains(key, valKey);
        }
    }
}
