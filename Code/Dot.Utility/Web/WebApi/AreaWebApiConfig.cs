using Dot.Utility.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Http.Routing.Constraints;

namespace Dot.Utility.Web
{
    internal class AreaWebApiConfig
    {

        internal static void Register(HttpConfiguration config, string area = null)
        {
            

            string routeName;
            if (!string.IsNullOrWhiteSpace(area))
            {
                if (area.EndsWith("/"))
                {
                    routeName = area.TrimEnd('/');
                }
                else
                {
                    routeName = area;
                    area = area + "/";
                }
            }
            else
            {
                area = routeName = string.Empty;
            }

            /*********************** 代码块解释 *********************************
             * 匹配：
             * GET /Controller/Action/a4e89a64-4e09-4604-9232-be9cfc7cd65a
             * GET /Controller/Action/1
            **********************************************************************/
            config.Routes.MapHttpRoute(
                name: routeName + "_Action_Id",
                routeTemplate: "api/" + area + "{controller}/{action}/{id}/",
                defaults: new { action = "" },
                constraints: new
                {
                    method = new HttpMethodConstraint(HttpMethod.Get),
                    id = new IDIsGuidOrIntRouteConstraint()
                }
            );

            /*********************** 代码块解释 *********************************
             * 匹配：
             * GET /Controller/
             * GET /Controller/a4e89a64-4e09-4604-9232-be9cfc7cd65a
             * GET /Controller/1
             * GET /Controller/?$filter=title%20startswith%20%27order%27
            **********************************************************************/
            config.Routes.MapHttpRoute(
                name: routeName + "_IdAndOData",
                routeTemplate: "api/" + area + "{controller}/{id}/",
                defaults: new { id = RouteParameter.Optional, action = "Get" },
                constraints: new
                {
                    method = new HttpMethodConstraint(HttpMethod.Get),
                    id = new IDIsGuidOrIntRouteConstraint()
                }
            );


            /*********************** 代码块解释 *********************************
            * 匹配：
            * GET /Controller/
            * GET /Controller/a4e89a64-4e09-4604-9232-be9cfc7cd65a
            * GET /Controller/?$filter=title%20startswith%20%27order%27
            //**********************************************************************/
            //config.Routes.MapHttpRoute(
            //    name: routeName + "_Post",
            //    routeTemplate: "api/" + area + "{controller}/{post}/",
            //    defaults: new { post = RouteParameter.Optional, action = "Add" },
            //    constraints: new
            //    {
            //        post = new MustBeOptionalRouteConstraint(),
            //        method = new HttpMethodConstraint(HttpMethod.Post),
            //    }
            //);


            /*********************** 代码块解释 *********************************
             * 匹配：
             * Post     /Controller/a4e89a64-4e09-4604-9232-be9cfc7cd65a
             * Post     /Controller/1
             * PUT      /Controller/a4e89a64-4e09-4604-9232-be9cfc7cd65a
             * PUT      /Controller/1
             * DELETE   /Controller/a4e89a64-4e09-4604-9232-be9cfc7cd65a
             * DELETE   /Controller/1
            **********************************************************************/
            config.Routes.MapHttpRoute(
                name: routeName + "_PUTAndDelete",
                routeTemplate: "api/" + area + "{controller}/{id}/",
                defaults: new object(),
                constraints: new
                {
                    method = new HttpMethodConstraint(HttpMethod.Post, HttpMethod.Put, HttpMethod.Delete),
                    id = new IDIsGuidOrIntRouteConstraint()
                }
            );

            /*********************** 代码块解释 *********************************
             * 匹配：
             * POST|DELETE|PUT      /Controller/
             * GET|POST|PUT|DELETE  /Controller/Action
            **********************************************************************/
            config.Routes.MapHttpRoute(
                name: routeName + "_Api",
                routeTemplate: "api/" + area + "{controller}/{action}/",
                defaults: new { action = RouteParameter.Optional }
            );
        }
    }
}
