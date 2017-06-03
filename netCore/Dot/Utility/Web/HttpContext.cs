using System;
using System.Collections.Generic;
using System.Text;

namespace Dot.Utility.Web
{
    /// <summary>
    /// 参考 http://www.cnblogs.com/Leo_wl/p/6195683.html
    /// </summary>
    public static class HttpContext
    {
        public static IServiceProvider ServiceProvider;

        static Microsoft.AspNetCore.Http.HttpContext _context;

        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                if (_context == null)
                {
                    object factory = ServiceProvider.GetService(typeof(Microsoft.AspNetCore.Http.IHttpContextAccessor));
                    _context = ((Microsoft.AspNetCore.Http.HttpContextAccessor)factory).HttpContext;
                }
                return _context;
            }
        }

    }
}
