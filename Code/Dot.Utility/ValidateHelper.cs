using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility
{
    public class ValidateHelper
    {
        /// <summary>
        /// 空警告
        /// </summary>
        /// <param name="val"></param>
        /// <param name="tip"></param>
        public static void NullError(object val, string tip)
        {
            if (val == null)
            {
                throw new  Exceptions.ValidationException(tip);
            }
        }


        /// <summary>
        /// 参数不能为空
        /// </summary>
        /// <param name="val"></param>
        /// <param name="tip"></param>
        public static void StringNull(object val, string tip)
        {
            if (val == null || string.IsNullOrWhiteSpace(val.ToString()))
            {
                throw new  Exceptions.ValidationException(tip + " 不能为空！ ");
            }
        }
 
    }
}
