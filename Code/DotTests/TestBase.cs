using Dot.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot
{
    public class TestBase
    {
        public TestBase()
        {
            new AppEngine().StartupApplication();
        }
    }

    public class AppEngine : AppImplementationBase
    {
        protected override void InitEnvironment()
        {
            PluginTable.Plugins.AddPlugin<Dot.Cache.CachePlugin>();

            base.InitEnvironment();
        }
    }
}
