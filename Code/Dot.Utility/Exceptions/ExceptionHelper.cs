using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Exceptions
{
    public class ExceptionHelper
    {
        public static string ToString(Exception ex)
        {
            StringBuilder sb = new StringBuilder(255);             
            while (ex != null)
            {
                sb.AppendFormat("{0}\r\n{1}\r\n", ex.Message, ex.StackTrace);
                ex = ex.InnerException;
            }

            return sb.ToString();
        }
    }
}
