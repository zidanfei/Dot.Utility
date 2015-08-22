using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dot.Cache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Dot.Cache.Tests
{
    [TestClass()]
    public class CacheManagerTests : TestBase
    {
        [TestMethod()]
        public void Test()
        {
            CacheManager.Cache.Add("a", "b", "c");

            var v= CacheManager.Cache.Get("a", "b");
            Assert.AreEqual(v, "c");
        }
    }
}
