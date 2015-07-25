using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility
{
    /// <summary>
    /// 密码类
    /// </summary>
    public class PasswordHelper
    {

        /// <summary>
        /// 随机生成密码
        /// </summary>
        /// <param name="pwdlen">密码长度</param>
        /// <returns></returns>
        public static string MakePassword(int pwdlen, bool strong = true)
        {
            string alphabets = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string special = "!@#$%^&*()";
            string num = "0123456789";
            string pwd = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()0123456789";
            StringBuilder password = new StringBuilder();
            Random r = new Random();
            if (strong)
            {
                password.Append(alphabets[r.Next(alphabets.Length)]);
                password.Append(special[r.Next(special.Length)]);
                password.Append(num[r.Next(num.Length)]);
                for (int i = 0; i < pwdlen - 3; i++)
                {
                    password.Append(pwd[r.Next(pwd.Length)]);
                }
            }
            else
            {
                for (int i = 0; i < pwdlen; i++)
                {
                    password.Append(pwd[r.Next(pwd.Length)]);
                }
            }
            return password.ToString();
        }
    }
}
