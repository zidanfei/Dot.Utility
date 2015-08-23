using Dot.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Cache
{
    [Export(typeof(ICacheProvider), "MemCache")]

    public class MemCacheProvider : ICacheProvider
    {

        public void Add(string key, string valKey, object value)
        {
            throw new NotImplementedException();
        }

        public void Put(string key, string valKey, object value)
        {
            throw new NotImplementedException();
        }

        public object Get(string key, string valKey)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string key)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string key, string valKey)
        {
            throw new NotImplementedException();
        }
    }
}
