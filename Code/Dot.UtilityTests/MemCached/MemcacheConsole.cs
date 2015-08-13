using Dot.Utility.Memcached;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.UtilityTests.MemCached
{
    public class MemcacheConsole
    {

        [STAThread]
        public static void Main(String[] args)
        {
            sockpool();
        }


        public static void sockpool()
        {
            String[] serverlist = { "10.2.3.62:11211", "10.2.3.173:11211" };
            int runs = 100;
            int start = 200;

            // initialize the pool for memcache servers
            SockIOPool pool = SockIOPool.GetInstance();
            pool.SetServers(serverlist);

            pool.InitConnections = 3;
            pool.MinConnections = 3;
            pool.MaxConnections = 5;

            pool.SocketConnectTimeout = 1000;
            pool.SocketTimeout = 3000;

            pool.MaintenanceSleep = 30;
            pool.Failover = true;

            pool.Nagle = false;
            pool.Initialize();

            //			// get client instance
            MemcachedClient mc = new MemcachedClient();
            mc.EnableCompression = false;

            //			MemcachedClient mc = new MemcachedClient();
            //			mc.CompressEnable = false;
            //			mc.CompressThreshold = 0;
            //			mc.Serialize = true;

            string keyBase = "testKey";
            string obj = "This is a test of an object blah blah es, serialization does not seem to slow things down so much.  The gzip compression is horrible horrible performance, so we only use it for very large objects.  I have not done any heavy benchmarking recently";

            long begin = DateTime.Now.Ticks;
            for (int i = start; i < start + runs; i++)
            {
                mc.Set(keyBase + i, obj);
            }
            long end = DateTime.Now.Ticks;
            long time = end - begin;

            Console.WriteLine(runs + " sets: " + new TimeSpan(time).ToString() + "ms");

            begin = DateTime.Now.Ticks;
            int hits = 0;
            int misses = 0;
            for (int i = start; i < start + runs; i++)
            {
                string str = (string)mc.Get(keyBase + i);
                if (str != null)
                    ++hits;
                else
                    ++misses;
            }
            end = DateTime.Now.Ticks;
            time = end - begin;

            Console.WriteLine(runs + " gets: " + new TimeSpan(time).ToString() + "ms");
            Console.WriteLine("Cache hits: " + hits.ToString());
            Console.WriteLine("Cache misses: " + misses.ToString());

            IDictionary stats = mc.Stats();
            foreach (string key1 in stats.Keys)
            {
                Console.WriteLine(key1);
                Hashtable values = (Hashtable)stats[key1];
                foreach (string key2 in values.Keys)
                {
                    Console.WriteLine(key2 + ":" + values[key2]);
                }
                Console.WriteLine();
            }

            SockIOPool.GetInstance().Shutdown();
            Console.ReadLine();

        }
    }
}
