using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Service
{
    public class ResponseResult
    {
        public static Result Response(Func<object> fun)
        {
            Result res = new Result();
            try
            {
                res.Data = fun();
                res.StatusCode = (int)HttpStatusCode.OK;
                res.Success = true;
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in ex.EntityValidationErrors)
                {
                    if (!item.IsValid)
                    {
                        foreach (var error in item.ValidationErrors)
                        {
                            sb.AppendFormat(string.Format("{0};", error.ErrorMessage));
                        }
                    }
                }
                res.Success = false;
                res.Message = sb.ToString();
                res.StatusCode = (int)HttpStatusCode.InternalServerError;
                if (Dot.Utility.Log.LogFactory.ExceptionLog.IsErrorEnabled)
                    Dot.Utility.Log.LogFactory.ExceptionLog.Error(ex);
                res.Exception = ex;
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = ex.Message;
                res.StatusCode = (int)HttpStatusCode.InternalServerError;
                if (Dot.Utility.Log.LogFactory.ExceptionLog.IsErrorEnabled)
                    Dot.Utility.Log.LogFactory.ExceptionLog.Error(ex);
                res.Exception = ex;
            }
            return res;
        }
    }
}
