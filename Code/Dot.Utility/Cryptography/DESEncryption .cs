
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
namespace Dot.Utility.Cryptography
{
    /// <summary>
    /// DES加密/解密类。
    /// </summary>
    public class DESEncryption 
    {

        #region ========加密========

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Encrypt(string Text)
        {
            return Encrypt(Text, "MATICSOFT");
        }
        /// <summary> 
        /// 加密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string Encrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(Text);
            des.Key = ASCIIEncoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        #endregion

        #region ========解密========


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Decrypt(string Text)
        {
            return Decrypt(Text, "MATICSOFT");
        }
        /// <summary> 
        /// 解密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string Decrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = Text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }

        #endregion


        #region 加密文件
        /// <summary>
        /// 加密文件
        /// </summary>
        /// <param name="filePath">输入文件路径</param>
        /// <param name="savePath">加密后输出文件路径</param>
        /// <param name="sKey">密码，可以为“”</param>
        /// <returns></returns>  
        public bool EncryptFile(string filePath, string savePath, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            FileStream fs = File.OpenRead(filePath);
            byte[] inputByteArray = new byte[fs.Length];
            fs.Read(inputByteArray, 0, (int)fs.Length);
            fs.Close();
            byte[] keyByteArray = Encoding.Default.GetBytes(sKey);
            SHA1 ha = new SHA1Managed();
            byte[] hb = ha.ComputeHash(keyByteArray);
            des.Key = ASCIIEncoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));           
           
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            fs = File.OpenWrite(savePath);
            foreach (byte b in ms.ToArray())
            {
                fs.WriteByte(b);
            }
            fs.Close();
            cs.Close();
            ms.Close();
            return true;
        }
        #endregion

        #region 解密文件
        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="filePath">输入文件路径</param>
        /// <param name="savePath">解密后输出文件路径</param>
        /// <param name="sKey">密码，可以为“”</param>
        /// <returns></returns>    
        public bool DecryptFile(string filePath, string savePath, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
          
            FileStream fs = File.OpenRead(filePath);
            byte[] inputByteArray = new byte[fs.Length];
            fs.Read(inputByteArray, 0, (int)fs.Length);
            fs.Close();
            byte[] keyByteArray = Encoding.Default.GetBytes(sKey);
            SHA1 ha = new SHA1Managed();
            byte[] hb = ha.ComputeHash(keyByteArray);
            des.Key = ASCIIEncoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));    
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            fs = File.OpenWrite(savePath);
            foreach (byte b in ms.ToArray())
            {
                fs.WriteByte(b);
            }
            fs.Close();
            cs.Close();
            ms.Close();
            return true;
        }
        #endregion
    }
}
