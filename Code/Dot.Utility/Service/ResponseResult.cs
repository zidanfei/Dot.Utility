using Dot.Utility.Exceptions;
using Dot.Utility.Log;
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
                if (Dot.Utility.Log.LogFactory.WebExceptionLog.IsErrorEnabled)
                    Dot.Utility.Log.LogFactory.WebExceptionLog.Error(ex);
                res.Exception = ex;
            }
            catch (DotException ex)
            {
                res.Success = false;
                if (ex.LogMessage != null)
                {
                    if (ex.LogMessage.Message != null)
                        res.Message = ex.LogMessage.Message.ToString();
                    else
                        res.Message = ex.Message.ToString();
                }
                else
                {
                    res.Message = ex.Message;
                }
                res.StatusCode = (int)HttpStatusCode.InternalServerError;
                if (Dot.Utility.Log.LogFactory.WebExceptionLog.IsErrorEnabled)
                {
                    if (ex.LogMessage != null)
                    {
                        Dot.Utility.Log.LogFactory.WebExceptionLog.Error(ex.LogMessage, ex.InnerException);
                    }
                    else
                    {
                        Dot.Utility.Log.LogFactory.WebExceptionLog.Error(ex.Message, ex);
                    }
                }
                res.Exception = ex;
                res.Data = ex.Data;
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = ex.Message;
                res.StatusCode = (int)HttpStatusCode.InternalServerError;
                if (Dot.Utility.Log.LogFactory.WebExceptionLog.IsErrorEnabled)
                    Dot.Utility.Log.LogFactory.WebExceptionLog.Error(ex);
                res.Exception = ex;
            }
            return res;
        }
    }
}
