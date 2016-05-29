using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Cryptography
{
    public class MD5Encryption
    {


        public static string Encrypt(string pwd)
        {
            string res = string.Empty;
            var md = System.Security.Cryptography.MD5.Create();

            byte[] bytes = md.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pwd));

            foreach (byte b in bytes)
            {

                res += b.ToString();

            }
            return res;
        }
        /// <summary>
        /// MD5　32位加密 加密后密码为大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Encrypt32Bit(string str)
        {
            return Encrypt32Bit(str, Encoding.UTF8);
        }

        /// <summary>
        /// MD5　32位加密 加密后密码为大写
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string Encrypt32Bit(string str, Encoding encoding)
        {
            MD5 md5 = MD5.Create();//实例化一个md5对像
            string cl = str;
            string pwd = "";
            string p;
            byte[] s = md5.ComputeHash(encoding.GetBytes(cl)); // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                p = s[i].ToString("X");
                if (p.Length == 1)
                {
                    p = "0" + p;
                }
                pwd = pwd + p;
            }
            return pwd;
        }

        /// <summary>
        /// MD5 16位加密 加密后密码为大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Encrypt16Bit(string str)
        {
            return Encrypt16Bit(str, Encoding.UTF8);
        }

        /// <summary>
        /// MD5 16位加密 加密后密码为大写
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string Encrypt16Bit(string str, Encoding encoding)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(encoding.GetBytes(str)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }
    }
}
