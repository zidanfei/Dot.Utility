using System;
using System.Collections.Generic;
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
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = ex.Message;
                res.StatusCode = (int)HttpStatusCode.InternalServerError;
                if (Dot.Utility.Log.LogFactory.ExceptionLog.IsErrorEnabled)
                    Dot.Utility.Log.LogFactory.ExceptionLog.Error(ex);
            }
            return res;
        }
    }
}
