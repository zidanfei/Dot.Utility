using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility
{
    public class AuthCodeHelper
    {

        /// <summary>
        /// 随机生成验证码
        /// </summary>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public static string MakeCode(int len)
        {

            string code = "abcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder codeStr = new StringBuilder();
            Random r = new Random();

            for (int i = 0; i < len; i++)
            {
                codeStr.Append(code[r.Next(code.Length)]);
            }
            return codeStr.ToString();
        }
    }
}
