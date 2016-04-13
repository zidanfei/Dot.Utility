using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Dot.WebApi
{
    public class WebApiCors
    {
        public static void EnableCors(HttpConfiguration config)
        {
            //支持跨域访问
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
        }
    }
}
