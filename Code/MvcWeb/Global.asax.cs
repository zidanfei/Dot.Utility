using Dot.ComponentModel;
using Dot.Demo;
using Dot.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace MvcWeb
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            new AppEngine().StartupApplication();
            DBMigration.Init();
        }

        /// <summary>
        /// 重写Init方法并设置会话行为类型
        /// </summary>
        public override void Init()
        {
            this.PostAuthenticateRequest += (sender, e) => HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            base.Init();
        }
    }

    public class AppEngine : DotAppStart
    {
        protected override void InitEnvironment()
        {
            PluginTable.Plugins.AddPlugin<DotDemoPlugin>();
            PluginTable.Plugins.AddPlugin<MvcWebPlugin>();
            base.InitEnvironment();
        }
    }

    public class MvcWebPlugin : AppPlugin
    {
        public override void Initialize(IApp app)
        {

        }
    }
}
