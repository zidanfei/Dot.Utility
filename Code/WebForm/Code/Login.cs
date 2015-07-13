using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace WebForm.Code
{
    public class Login
    {

        /// <summary>
        /// 执行用户登录操作
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <param name="userData">与登录名相关的用户信息</param>
        /// <param name="expiration">登录Cookie的过期时间，单位：分钟。</param>
        private void SignIn(string loginName, string userData, int expiration)
        {
            if (string.IsNullOrEmpty(loginName))
                throw new ArgumentNullException("loginName");
            if (userData == null)
                throw new ArgumentNullException("userData");

            // 1. 把需要保存的用户数据转成一个字符串。
            string data = null;
            if (userData != null) data = (new JavaScriptSerializer()).Serialize(userData);

            // 2. 创建一个FormsAuthenticationTicket，它包含登录名以及额外的用户数据。
            //FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(2, loginName, DateTime.Now, DateTime.Now.AddDays(1), true, data);

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, loginName, DateTime.Now, DateTime.Now.Add(FormsAuthentication.Timeout), false, data);

            // 3. 加密Ticket，变成一个加密的字符串。
            string cookieValue = FormsAuthentication.Encrypt(ticket);

            // 4. 根据加密结果创建登录Cookie
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieValue);
            cookie.HttpOnly = true;
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Domain = FormsAuthentication.CookieDomain;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (expiration > 0)
                cookie.Expires = DateTime.Now.AddDays(expiration);

            HttpContext context = HttpContext.Current;
            if (context == null)
                throw new InvalidOperationException();

            // 5. 写登录Cookie
            context.Response.Cookies.Remove(cookie.Name);
            context.Response.Cookies.Add(cookie);
        }
    }
}