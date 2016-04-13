using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Validation;

namespace Dot.WebApi
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config, string area = null)
        {
            if (Dot.Utility.Config.ConfigHelper.GetAppSettingOrDefault("EnableCors", false))
            {
                WebApiCors.EnableCors(config);
            }

            //日期格式化
            if (config.Formatters.Count(m => m.GetType() == typeof(DateTypeFormatter)) == 0)
            {
                config.Formatters.Insert(0, new DateTypeFormatter());
                config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            }
            config.Formatters.Remove(config.Formatters.XmlFormatter);


            config.Services.Replace(typeof(IBodyModelValidator), new EntityAwareBodyModelValidator());
            config.BindParameter(typeof(ODataQueryCriteria), new ODataQueryCriteriaBinder());

            #region PUT Id 参数绑定

            //PUT 时，Id 需要从路由中获取，这里需要提供一个额外的参数绑定实现。
            config.ParameterBindingRules.Add(parameter =>
            {
                Type parameterType = parameter.ParameterType;
                if (typeof(Entity).IsAssignableFrom(parameterType))
                {
                    var formatters = parameter.Configuration.Formatters;
                    var bodyModelValidator = parameter.Configuration.Services.GetBodyModelValidator();
                    return new IdFromRouteFormatterParameterBinding(parameter, formatters, bodyModelValidator);
                }
                return null;
            });
            //上述方案，也可以替换以下服务实现。
            //config.Services.Replace(typeof(IActionValueBinder), new iWSActionValueBinder()); 

            #endregion

            AreaWebApiConfig.Register(config, area);
        }
    }
}