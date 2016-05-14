using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace Dot.Utility.Web.WebApi
{
    /// <summary>
    /// web Api �Զ�������
    /// </summary>
    public class WebApiSelfHost
    {
        //public static HttpSelfHostServer HttpSelfHostServer { get; set; }
        public static void Start(string url)
        {
            //var config = new HttpSelfHostConfiguration(url);
            //config.Routes.MapHttpRoute("default", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            //var server = new HttpSelfHostServer(config);
            //server.OpenAsync().Wait();
            //Console.WriteLine("Server is opened");

            HttpSelfHostConfiguration configuration = new HttpSelfHostConfiguration(url);
            configuration.Routes.MapHttpRoute("default", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            //WebApiHttpStarter.Register(configuration);
            using (var HttpSelfHostServer = new HttpSelfHostServer(configuration))
            {
                HttpSelfHostServer.OpenAsync().Wait();
            }
            Console.WriteLine("Server is opened");
        }



    }
}
