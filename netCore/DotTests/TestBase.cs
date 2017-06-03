using Dot.ComponentModel;
using Dot.Log.Log4netImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotTests
{
    public class TestBase
    {
        public TestBase()
        {
            new AppEngine().StartupApplication();
        }
    }

    public class AppEngine : DotAppStart
    {
        protected override void InitEnvironment()
        {
            //PluginTable.Plugins.AddPlugin<Dot.Cache.CachePlugin>();
            PluginTable.Plugins.AddPlugin<DotTestPlugin>();
            PluginTable.Plugins.AddPlugin<log4netPlugin>();

            base.InitEnvironment();
        }
    }
    public class DotTestPlugin : Dot.ComponentModel.AppPlugin
    {
        public override void Initialize(IApp app)
        {

        }



    }
}
