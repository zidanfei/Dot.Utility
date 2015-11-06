using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Cryptography
{
    public class MD5Encryption
    {

        public static string Encryption(string pwd)
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
    }
}
