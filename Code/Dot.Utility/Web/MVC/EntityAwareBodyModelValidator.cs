using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;
using System.Web.Http.Validation;

namespace Dot.Utility.Web
{
    /// <summary>
    /// 传入的实体校验
    /// </summary>
    public class EntityAwareBodyModelValidator : IBodyModelValidator// DefaultBodyModelValidator
    {
        private DefaultBodyModelValidator _inner = new DefaultBodyModelValidator();

        public bool Validate(object model, Type type, ModelMetadataProvider metadataProvider, HttpActionContext actionContext, string keyPrefix)
        {          

            return _inner.Validate(model, type, metadataProvider, actionContext, keyPrefix);
        }
    }
}