using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MvcWeb.ApiControllers
{
    [EnableCors("*", "*", "*")]
    public class ACSController : ApiController
    {
    }
}
