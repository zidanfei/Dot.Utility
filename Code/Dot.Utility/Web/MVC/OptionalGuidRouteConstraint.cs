using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace Dot.Utility.Web
{
    public class IDIsGuidOrIntRouteConstraint : IHttpRouteConstraint
    {
        public bool Match(System.Net.Http.HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            object value = null;
            if (values.TryGetValue(parameterName, out value))
            {
                if (value == RouteParameter.Optional) return true;

                Guid res;
                int resInt;
                return Guid.TryParse(value.ToString(), out res) || int.TryParse(value.ToString(), out resInt);
            }
            return false;
        }
    }

   
    public class MustBeOptionalRouteConstraint : IHttpRouteConstraint
    {
        public bool Match(System.Net.Http.HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            object value = null;
            if (values.TryGetValue(parameterName, out value))
            {
                return value == RouteParameter.Optional;
            }
            return true;
        }
    }
}