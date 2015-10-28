using Dot.Demo;
using Dot.IOC;
using MvcWeb.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Security;

namespace MvcWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

           var dlist= new Dot.Demo.DBContext().Context.Database.SqlQuery<Project>("select * from project").ToList();


            IProjectService service = ObjectContainer.CreateInstance<IProjectService>();
            var list = service.GetList();
            var one = service.Get(m => m.Id == 1);
            HomeIndexModel model = new HomeIndexModel();
            model.div = JsonConvert.SerializeObject(Dot.Config.Model.PlatformConfig.ServerConfig.KeyValueSettings["test"]);
            return View(model);
        }
    }


    ///
    /// 基本验证Attribtue，用以Action的权限处理
    ///
    public class BasicAuthenticationAttribute : ActionFilterAttribute
    {
        ///
        /// 检查用户是否有该Action执行的操作权限
        ///
        ///
        //public override void OnActionExecuting(HttpActionContext actionContext)
        //{
        //    //检验用户ticket信息，用户ticket信息来自调用发起方
        //    if (actionContext.Request.Headers.Authorization != null)
        //    {
        //        //解密用户ticket,并校验用户名密码是否匹配
        //        var encryptTicket = actionContext.Request.Headers.Authorization.Parameter;
        //        if (ValidateUserTicket(encryptTicket))
        //            base.OnActionExecuting(actionContext);
        //        else
        //            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        //    }
        //    else
        //    {
        //        //检查web.config配置是否要求权限校验
        //        bool isRquired = (WebConfigurationManager.AppSettings["WebApiAuthenticatedFlag"].ToString() == "true");
        //        if (isRquired)
        //        {
        //            //如果请求Header不包含ticket，则判断是否是匿名调用
        //            var attr = actionContext.ActionDescriptor.GetCustomAttributes().OfType();
        //            bool isAnonymous = attr.Any(a => a is AllowAnonymousAttribute);
        //            //是匿名用户，则继续执行；非匿名用户，抛出“未授权访问”信息
        //            if (isAnonymous)
        //                base.OnActionExecuting(actionContext);
        //            else
        //                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        //        }
        //        else
        //        {
        //            base.OnActionExecuting(actionContext);
        //        }
        //    }
        //}
        ///
        /// 校验用户ticket信息
        ///
        ///
        ///
        private bool ValidateUserTicket(string encryptTicket)
        {
            var userTicket = FormsAuthentication.Decrypt(encryptTicket);
            var userTicketData = userTicket.UserData;
            string userName = userTicketData.Substring(0, userTicketData.IndexOf(":"));
            string password = userTicketData.Substring(userTicketData.IndexOf(":") + 1);
            //检查用户名、密码是否正确，验证是合法用户
            //var isQuilified = CheckUser(userName, password);
            return true;
        }
    }

}
