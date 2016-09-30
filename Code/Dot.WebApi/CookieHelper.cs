using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Dot.WebApi
{
   public  class CookieHelper
    {

        //public object GetCookies()
        //{
        //    Collection<CookieHeaderValue> cookieValues = Request.Headers.GetCookies();

        //    //1.在WebApi中这种方式获取cookie 可以成功
        //    //2.在WebApi 中这种凡是获取Form,QueryString 中的参数是有效的
        //    HttpCookieCollection collection = HttpContext.Current.Request.Cookies;

        //    //1.获取传统context,再获取cookie 可以成功
        //    HttpContextBase baseContext = (HttpContextBase)Request.Properties["MS_HttpContext"];
        //    HttpCookieCollection cookie2 = baseContext.Request.Cookies;


        //    //2.api中推荐的获取操作cooki 方式
        //    Collection<CookieHeaderValue> cookieValues = Request.Headers.GetCookies();


        //    return CookieHelper.GetString("username");
        //}
    }
}
