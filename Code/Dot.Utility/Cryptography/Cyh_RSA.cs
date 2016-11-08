using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Cryptography
{
    public class Cyh_RSA
    {
        public Cyh_RSA()
        {

        }

        /// <summary>
                /// 加密
                /// </summary>
                /// <param name="p_inputString">需要加密的字符串信息</param>
                /// <param name="p_strKeyPath">加密用的密钥所在的路径(*.cyh_publickey)</param>
                /// <returns>加密以后的字符串信息</returns>
        public string Encrypt(string p_inputString, string p_strKeyPath)
        {
            string fileString = null;
            string outString = null;
            if (File.Exists(p_strKeyPath))
            {
                StreamReader streamReader = new StreamReader(p_strKeyPath, true);
                fileString = streamReader.ReadToEnd();
                streamReader.Close();

            }

            if (fileString != null)
            {
                string bitStrengthString = fileString.Substring(0, fileString.IndexOf("</BitStrength>") + 14);
                fileString = fileString.Replace(bitStrengthString, "");
                int bitStrength = Convert.ToInt32(bitStrengthString.Replace("<BitStrength>", "").Replace("</BitStrength>", ""));
                try
                {
                    outString = EncryptString(p_inputString, bitStrength, fileString);
                }
                catch (Exception Ex)
                {
                    Log.LogFactory.ExceptionLog.Error(Ex.Message, Ex);
                }

            }

            return outString;
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="p_inputString">需要解密的字符串信息</param>
        /// <param name="p_strKeyPath">解密用的密钥所在的路径(*.cyh_primarykey)</param>
        /// <returns>解密以后的字符串信息</returns>
        public string Decrypt(string p_inputString, string p_strKeyPath)
        {
            string fileString = null;
            string outString = null;
            if (File.Exists(p_strKeyPath))
            {
                StreamReader streamReader = new StreamReader(p_strKeyPath, true);
                fileString = streamReader.ReadToEnd();
                streamReader.Close();

            }

            if (fileString != null)
            {
                string bitStrengthString = fileString.Substring(0, fileString.IndexOf("</BitStrength>") + 14);
                fileString = fileString.Replace(bitStrengthString, "");
                int bitStrength = Convert.ToInt32(bitStrengthString.Replace("<BitStrength>", "").Replace("</BitStrength>", ""));
                try
                {
                    outString = DecryptString(p_inputString, bitStrength, fileString);
                }
                catch (Exception Ex)
                {
                    Log.LogFactory.ExceptionLog.Error(Ex.Message, Ex);
                }

            }

            return outString;

        }

        /// <summary>
                /// 加密
                /// </summary>
                /// <param name="p_inputString">需要加密的字符串信息</param>
                /// <param name="p_xmlString">加密用的密钥</param>
                /// <returns>加密以后的字符串信息</returns>
        public string EncryptString(string p_inputString, string p_xmlString)
        {
            string outString = null;
            if (p_xmlString != null)
            {
                string bitStrengthString = p_xmlString.Substring(0, p_xmlString.IndexOf("</BitStrength>") + 14);
                p_xmlString = p_xmlString.Replace(bitStrengthString, "");
                int bitStrength = Convert.ToInt32(bitStrengthString.Replace("<BitStrength>", "").Replace("</BitStrength>", ""));
                try
                {
                    outString = EncryptString(p_inputString, bitStrength, p_xmlString);
                }
                catch (Exception Ex)
                {
                    Log.LogFactory.ExceptionLog.Error(Ex.Message, Ex);
                }

            }

            return outString;
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="p_inputString">需要解密的字符串信息</param>
        /// <param name="p_xmlString">解密用的密钥</param>
        /// <returns>解密以后的字符串信息</returns>
        public string DecryptString(string p_inputString, string p_xmlString)
        {
            string outString = null;
            if (p_xmlString != null)
            {
                string bitStrengthString = p_xmlString.Substring(0, p_xmlString.IndexOf("</BitStrength>") + 14);
                p_xmlString = p_xmlString.Replace(bitStrengthString, "");
                int bitStrength = Convert.ToInt32(bitStrengthString.Replace("<BitStrength>", "").Replace("</BitStrength>", ""));
                try
                {
                    outString = DecryptString(p_inputString, bitStrength, p_xmlString);
                }
                catch (Exception Ex)
                {
                    Log.LogFactory.ExceptionLog.Error(Ex.Message, Ex);
                }

            }

            return outString;

        }


        /// <summary>
                /// 加密
                /// </summary>
                /// <param name="p_inputString">需要加密的字符串</param>
                /// <param name="p_dwKeySize">密钥的大小</param>
                /// <param name="p_xmlString">包含密钥的XML文本信息</param>
                /// <returns>加密后的文本信息</returns>
        string EncryptString(string p_inputString, int p_dwKeySize, string p_xmlString)
        {
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider(p_dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(p_xmlString);
            int keySize = p_dwKeySize / 8;
            byte[] bytes = Encoding.UTF32.GetBytes(p_inputString);
            int maxLength = keySize - 42;
            int dataLength = bytes.Length;
            int iterations = dataLength / maxLength;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i <= iterations; i++)
            {
                byte[] tempBytes = new byte[(dataLength - maxLength * i > maxLength) ? maxLength : dataLength - maxLength * i];
                Buffer.BlockCopy(bytes, maxLength * i, tempBytes, 0, tempBytes.Length);
                byte[] encryptedBytes = rsaCryptoServiceProvider.Encrypt(tempBytes, true);
                Array.Reverse(encryptedBytes);
                stringBuilder.Append(Convert.ToBase64String(encryptedBytes));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
                /// 解密
                /// </summary>
                /// <param name="p_inputString">需要解密的字符串信息</param>
                /// <param name="p_dwKeySize">密钥的大小</param>
                /// <param name="p_xmlString">包含密钥的文本信息</param>
                /// <returns>解密后的文本信息</returns>
        string DecryptString(string inputString, int dwKeySize, string xmlString)
        {
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(xmlString);
            int base64BlockSize = ((dwKeySize / 8) % 3 != 0) ? (((dwKeySize / 8) / 3) * 4) + 4 : ((dwKeySize / 8) / 3) * 4;
            int iterations = inputString.Length / base64BlockSize;
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < iterations; i++)
            {
                byte[] encryptedBytes = Convert.FromBase64String(inputString.Substring(base64BlockSize * i, base64BlockSize));
                Array.Reverse(encryptedBytes);
                arrayList.AddRange(rsaCryptoServiceProvider.Decrypt(encryptedBytes, true));
            }
            return Encoding.UTF32.GetString(arrayList.ToArray(Type.GetType("System.Byte")) as byte[]);
        }

        /// <summary>
                /// 形成并保存公开密钥和私有密钥
                /// </summary>
                /// <param name="p_currentBitStrength">密钥大小</param>
        public void SaveKey(int p_currentBitStrength)
        {
            RSACryptoServiceProvider RSAProvider = new RSACryptoServiceProvider(p_currentBitStrength);
            string publicAndPrivateKeys = "<BitStrength>" + p_currentBitStrength.ToString() + "</BitStrength>" + RSAProvider.ToXmlString(true);
            string justPublicKey = "<BitStrength>" + p_currentBitStrength.ToString() + "</BitStrength>" + RSAProvider.ToXmlString(false);
            if (saveFile("Save Public/Private Keys As", "Public/Private Keys Document( *.cyh_primarykey )|*.cyh_primarykey", publicAndPrivateKeys))
            {
                while (!saveFile("Save Public Key As", "Public Key Document( *.cyh_publickey )|*.cyh_publickey", justPublicKey))
                {
                    ;
                }
            }
        }

        /// <summary>
                /// 形成并保存公开密钥和私有密钥
                /// </summary>
                /// <param name="p_currentBitStrength">密钥大小</param>
                /// <param name="pubKey">公开密钥</param>
                /// <param name="privateKey">私有密钥</param>
        public void SaveKey(int p_currentBitStrength, out string pubKey, out string privateKey)
        {
            RSACryptoServiceProvider RSAProvider = new RSACryptoServiceProvider(p_currentBitStrength);
            privateKey = "<BitStrength>" + p_currentBitStrength.ToString() + "</BitStrength>" + RSAProvider.ToXmlString(true);
            pubKey = "<BitStrength>" + p_currentBitStrength.ToString() + "</BitStrength>" + RSAProvider.ToXmlString(false);

        }

        /// <summary>
                /// 保存信息
                /// </summary>
                /// <param name="p_title">标题</param>
                /// <param name="p_filterString">过滤条件</param>
                /// <param name="p_outputString">输出内容</param>
                /// <returns>是否成功</returns>        
        private bool saveFile(string p_title, string p_filterString, string p_outputString)
        {

            string FileName = "";
            {
                try
                {
                    StreamWriter streamWriter = new StreamWriter(FileName, false);
                    if (p_outputString != null)
                    {
                        streamWriter.Write(p_outputString);
                    }
                    streamWriter.Close();
                    return true;
                }
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex.Message);
                    return false;
                }
            }

        }
    }
}
