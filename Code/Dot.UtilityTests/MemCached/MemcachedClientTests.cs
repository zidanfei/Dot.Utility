using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dot.Utility.Memcached;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Collections;
namespace Dot.Utility.Memcached.Tests
{
    [TestClass()]
    public class MemcachedClientTests
    {
        String[] serverlist = { "10.2.3.62:11211", "10.2.3.173:11211" };
        [TestMethod()]
        public void MemcachedClientTest()
        {

            // initialize the pool for memcache servers
            SockIOPool pool = SockIOPool.GetInstance("test");
            pool.SetServers(serverlist);
            pool.Initialize();

            mc = new MemcachedClient();
            mc.PoolName = "test";
            mc.EnableCompression = false;

            test1();

            pool.Shutdown();


        }


        public static MemcachedClient mc = null;


        public static void test1()
        {
            mc.Set("foo", true);
            bool b = (bool)mc.Get("foo");
            Assert.AreEqual(b, true);

            mc.Set("foo", int.MaxValue);
            int i = (int)mc.Get("foo");

            Assert.AreEqual(i, int.MaxValue);

            string input = "test of string encoding";
            mc.Set("foo", input);
            string s = (string)mc.Get("foo");
            Assert.IsTrue(s == input);
            int unique = 2;
            s =  mc.Get("foo", unique).ToString();
            Assert.IsTrue(s == input);


            mc.Set("foo", 'z');
            char c = (char)mc.Get("foo");
            Assert.IsTrue(c == 'z');

            mc.Set("foo", (byte)127);
            byte b1 = (byte)mc.Get("foo");
            Assert.IsTrue(b1 == 127);

            mc.Set("foo", new StringBuilder("hello"));
            StringBuilder o = (StringBuilder)mc.Get("foo");
            Assert.IsTrue(o.ToString() == "hello");

            mc.Set("foo", (short)100);
            short o1 = (short)mc.Get("foo");
            Assert.IsTrue(o1 == 100);

            mc.Set("foo", long.MaxValue);
            long o2 = (long)mc.Get("foo");
            Assert.IsTrue(o2 == long.MaxValue);

            mc.Set("foo", 1.1d);
            double o3 = (double)mc.Get("foo");
            Assert.IsTrue(o3 == 1.1d);

            mc.Set("foo", 1.1f);
            float o4 = (float)mc.Get("foo");
            Assert.IsTrue(o4 == 1.1f);

            mc.Delete("foo");
            mc.Set("foo", 100, DateTime.Now);
            System.Threading.Thread.Sleep(1000);
            var o5 = mc.Get("foo");

            Assert.IsTrue(o5 != null);

            long i1 = 0;
            mc.StoreCounter("foo", i1);
            mc.Increment("foo"); // foo now == 1
            mc.Increment("foo", (long)5); // foo now == 6
            long j = mc.Decrement("foo", (long)2); // foo now == 4
            Assert.IsTrue(j == 4);
            Assert.IsTrue(j == mc.GetCounter("foo"));

            DateTime d1 = new DateTime();
            mc.Set("foo", d1);
            DateTime d2 = (DateTime)mc.Get("foo");
            Assert.IsTrue(d1 == d2);

            if (mc.KeyExists("foobar123"))
                mc.Delete("foobar123");
            Assert.IsTrue(!mc.KeyExists("foobar123"));
            mc.Set("foobar123", 100000);
            Assert.IsTrue(mc.KeyExists("foobar123"));

            if (mc.KeyExists("counterTest123"))
                mc.Delete("counterTest123");
            Assert.IsTrue(!mc.KeyExists("counterTest123"));
            mc.StoreCounter("counterTest123", 0);
            Assert.IsTrue(mc.KeyExists("counterTest123"));
        }

    }
}
