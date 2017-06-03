using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Cryptography
{
    public class RSACryption
    {
        /// <summary>
                /// 加密
                /// </summary>
                /// <param name="p_inputString">需要加密的字符串信息</param>
                /// <param name="p_strKeyPath">加密用的密钥所在的路径(*.cyh_publickey)</param>
                /// <returns>加密以后的字符串信息</returns>
        public static string Encrypt(string p_inputString, string p_strKeyPath)
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
        public static string Decrypt(string p_inputString, string p_strKeyPath)
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
                /// <param name="p_xmlPubString">加密用的密钥</param>
                /// <returns>加密以后的字符串信息</returns>
        public static string EncryptString(string p_inputString, string p_xmlPubString)
        {
            string outString = null;
            if (p_xmlPubString != null)
            {
                string bitStrengthString = p_xmlPubString.Substring(0, p_xmlPubString.IndexOf("</BitStrength>") + 14);
                p_xmlPubString = p_xmlPubString.Replace(bitStrengthString, "");
                int bitStrength = Convert.ToInt32(bitStrengthString.Replace("<BitStrength>", "").Replace("</BitStrength>", ""));
                try
                {
                    outString = EncryptString(p_inputString, bitStrength, p_xmlPubString);
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
        /// <param name="p_xmlPriString">解密用的密钥</param>
        /// <returns>解密以后的字符串信息</returns>
        public static string DecryptString(string p_inputString, string p_xmlPriString)
        {
            string outString = null;
            if (p_xmlPriString != null)
            {
                string bitStrengthString = p_xmlPriString.Substring(0, p_xmlPriString.IndexOf("</BitStrength>") + 14);
                p_xmlPriString = p_xmlPriString.Replace(bitStrengthString, "");
                int bitStrength = Convert.ToInt32(bitStrengthString.Replace("<BitStrength>", "").Replace("</BitStrength>", ""));
                try
                {
                    outString = DecryptString(p_inputString, bitStrength, p_xmlPriString);
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
        static string EncryptString(string p_inputString, int p_dwKeySize, string p_xmlString)
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
        static string DecryptString(string inputString, int dwKeySize, string xmlString)
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
        static void SaveKey(int p_currentBitStrength)
        {
            RSACryptoServiceProvider RSAProvider = new RSACryptoServiceProvider(p_currentBitStrength);
            string publicAndPrivateKeys = "<BitStrength>" + p_currentBitStrength.ToString() + "</BitStrength>" + RSAProvider.ToXmlString(true);
            string justPublicKey = "<BitStrength>" + p_currentBitStrength.ToString() + "</BitStrength>" + RSAProvider.ToXmlString(false);
            RSAProvider.Clear();
            if (saveFile("Save public static/Private Keys As", "public static/Private Keys Document( *.cyh_primarykey )|*.cyh_primarykey", publicAndPrivateKeys))
            {
                while (!saveFile("Save public static Key As", "public static Key Document( *.cyh_publickey )|*.cyh_publickey", justPublicKey))
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
        public static void SaveKey(int p_currentBitStrength, out string pubKey, out string privateKey)
        {
            RSACryptoServiceProvider RSAProvider = new RSACryptoServiceProvider(p_currentBitStrength);
            privateKey = "<BitStrength>" + p_currentBitStrength.ToString() + "</BitStrength>" + RSAProvider.ToXmlString(true);
            pubKey = "<BitStrength>" + p_currentBitStrength.ToString() + "</BitStrength>" + RSAProvider.ToXmlString(false);
            RSAProvider.Clear();

        }

        /// <summary>
                /// 保存信息
                /// </summary>
                /// <param name="p_title">标题</param>
                /// <param name="p_filterString">过滤条件</param>
                /// <param name="p_outputString">输出内容</param>
                /// <returns>是否成功</returns>        
        private static bool saveFile(string p_title, string p_filterString, string p_outputString)
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

        #region RSA数字签名

        #region 获取Hash描述表        
        /// <summary>
        /// 获取Hash描述表
        /// </summary>
        /// <param name="strSource">待签名的字符串</param>
        /// <param name="HashData">Hash描述</param>
        /// <returns></returns>
        public static bool GetHash(string strSource, out byte[] HashData)
        {
            try
            {
                byte[] Buffer;
                System.Security.Cryptography.HashAlgorithm MD5 = System.Security.Cryptography.HashAlgorithm.Create("MD5");
                Buffer = System.Text.Encoding.GetEncoding("GB2312").GetBytes(strSource);
                HashData = MD5.ComputeHash(Buffer);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取Hash描述表
        /// </summary>
        /// <param name="strSource">待签名的字符串</param>
        /// <param name="strHashData">Hash描述</param>
        /// <returns></returns>
        public static bool GetHash(string strSource, out string strHashData)
        {
            try
            {
                //从字符串中取得Hash描述 
                byte[] Buffer;
                byte[] HashData;
                System.Security.Cryptography.HashAlgorithm MD5 = System.Security.Cryptography.HashAlgorithm.Create("MD5");
                Buffer = System.Text.Encoding.GetEncoding("GB2312").GetBytes(strSource);
                HashData = MD5.ComputeHash(Buffer);
                strHashData = Convert.ToBase64String(HashData);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取Hash描述表
        /// </summary>
        /// <param name="objFile">待签名的文件</param>
        /// <param name="HashData">Hash描述</param>
        /// <returns></returns>
        public static bool GetHash(System.IO.FileStream objFile, out byte[] HashData)
        {
            try
            {
                //从文件中取得Hash描述 
                System.Security.Cryptography.HashAlgorithm MD5 = System.Security.Cryptography.HashAlgorithm.Create("MD5");
                HashData = MD5.ComputeHash(objFile);
                objFile.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取Hash描述表
        /// </summary>
        /// <param name="objFile">待签名的文件</param>
        /// <param name="strHashData">Hash描述</param>
        /// <returns></returns>
        public static bool GetHash(System.IO.FileStream objFile, out string strHashData)
        {
            try
            {
                //从文件中取得Hash描述 
                byte[] HashData;
                System.Security.Cryptography.HashAlgorithm MD5 = System.Security.Cryptography.HashAlgorithm.Create("MD5");
                HashData = MD5.ComputeHash(objFile);
                objFile.Close();
                strHashData = Convert.ToBase64String(HashData);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        static System.Text.RegularExpressions.Regex regBitStrength = new System.Text.RegularExpressions.Regex("<BitStrength>.*</BitStrength>");
        #region RSA签名
        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="strKeyPrivate">私钥</param>
        /// <param name="HashbyteSignature">待签名Hash描述</param>
        /// <param name="EncryptedSignatureData">签名后的结果</param>
        /// <returns></returns>
        public static bool SignatureFormatter(string strKeyPrivate, byte[] HashbyteSignature, out byte[] EncryptedSignatureData)
        {
            try
            {
                System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();

                strKeyPrivate = regBitStrength.Replace(strKeyPrivate, "");
                RSA.FromXmlString(strKeyPrivate);
                System.Security.Cryptography.RSAPKCS1SignatureFormatter RSAFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(RSA);
                //设置签名的算法为MD5 
                RSAFormatter.SetHashAlgorithm("MD5");
                //执行签名 
                EncryptedSignatureData = RSAFormatter.CreateSignature(HashbyteSignature);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="strKeyPrivate">私钥</param>
        /// <param name="HashbyteSignature">待签名Hash描述</param>
        /// <param name="m_strEncryptedSignatureData">签名后的结果</param>
        /// <returns></returns>
        public static bool SignatureFormatter(string strKeyPrivate, byte[] HashbyteSignature, out string strEncryptedSignatureData)
        {
            try
            {
                byte[] EncryptedSignatureData;
                System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();
                strKeyPrivate = regBitStrength.Replace(strKeyPrivate, "");
                RSA.FromXmlString(strKeyPrivate);
                System.Security.Cryptography.RSAPKCS1SignatureFormatter RSAFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(RSA);
                //设置签名的算法为MD5 
                RSAFormatter.SetHashAlgorithm("MD5");
                //执行签名 
                EncryptedSignatureData = RSAFormatter.CreateSignature(HashbyteSignature);
                strEncryptedSignatureData = Convert.ToBase64String(EncryptedSignatureData);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="strKeyPrivate">私钥</param>
        /// <param name="strHashbyteSignature">待签名Hash描述</param>
        /// <param name="EncryptedSignatureData">签名后的结果</param>
        /// <returns></returns>
        public static bool SignatureFormatter(string strKeyPrivate, string strHashbyteSignature, out byte[] EncryptedSignatureData)
        {
            try
            {
                byte[] HashbyteSignature;

                HashbyteSignature = Convert.FromBase64String(strHashbyteSignature);
                System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();

                strKeyPrivate = regBitStrength.Replace(strKeyPrivate, "");
                RSA.FromXmlString(strKeyPrivate);
                System.Security.Cryptography.RSAPKCS1SignatureFormatter RSAFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(RSA);
                //设置签名的算法为MD5 
                RSAFormatter.SetHashAlgorithm("MD5");
                //执行签名 
                EncryptedSignatureData = RSAFormatter.CreateSignature(HashbyteSignature);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="strKeyPrivate">私钥</param>
        /// <param name="strHashbyteSignature">待签名Hash描述</param>
        /// <param name="strEncryptedSignatureData">签名后的结果</param>
        /// <returns></returns>
        public static bool SignatureFormatter(string strKeyPrivate, string strHashbyteSignature, out string strEncryptedSignatureData)
        {
            try
            {
                byte[] HashbyteSignature;
                byte[] EncryptedSignatureData;
                HashbyteSignature = Convert.FromBase64String(strHashbyteSignature);
                System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();
                strKeyPrivate = regBitStrength.Replace(strKeyPrivate, "");
                RSA.FromXmlString(strKeyPrivate);
                System.Security.Cryptography.RSAPKCS1SignatureFormatter RSAFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(RSA);
                //设置签名的算法为MD5 
                RSAFormatter.SetHashAlgorithm("MD5");
                //执行签名 
                EncryptedSignatureData = RSAFormatter.CreateSignature(HashbyteSignature);
                strEncryptedSignatureData = Convert.ToBase64String(EncryptedSignatureData);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="priRSA">私钥RSA</param>
        /// <param name="strHashbyteSignature">待签名Hash描述</param>
        /// <param name="strEncryptedSignatureData">签名后的结果</param>
        /// <returns></returns>
        public static bool SignatureFormatter(System.Security.Cryptography.RSACryptoServiceProvider priRSA, string strHashbyteSignature, out string strEncryptedSignatureData)
        {
            try
            {
                byte[] HashbyteSignature;
                byte[] EncryptedSignatureData;
                HashbyteSignature = Convert.FromBase64String(strHashbyteSignature);
                System.Security.Cryptography.RSAPKCS1SignatureFormatter RSAFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(priRSA);
                //设置签名的算法为MD5 
                RSAFormatter.SetHashAlgorithm("MD5");
                //执行签名 
                EncryptedSignatureData = RSAFormatter.CreateSignature(HashbyteSignature);
                strEncryptedSignatureData = Convert.ToBase64String(EncryptedSignatureData);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region RSA 签名验证
        /// <summary>
        /// RSA签名验证
        /// </summary>
        /// <param name="strKeyPublic">公钥</param>
        /// <param name="HashbyteDeformatter">Hash描述</param>
        /// <param name="DeformatterData">签名后的结果</param>
        /// <returns></returns>
        public static bool SignatureDeformatter(string strKeyPublic, byte[] HashbyteDeformatter, byte[] DeformatterData)
        {
            try
            {
                System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();
                strKeyPublic = regBitStrength.Replace(strKeyPublic, "");
                RSA.FromXmlString(strKeyPublic);
                System.Security.Cryptography.RSAPKCS1SignatureDeformatter RSADeformatter = new System.Security.Cryptography.RSAPKCS1SignatureDeformatter(RSA);
                //指定解密的时候HASH算法为MD5 
                RSADeformatter.SetHashAlgorithm("MD5");
                if (RSADeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// RSA签名验证
        /// </summary>
        /// <param name="strKeyPublic">公钥</param>
        /// <param name="strHashbyteDeformatter">Hash描述</param>
        /// <param name="DeformatterData">签名后的结果</param>
        /// <returns></returns>
        public static bool SignatureDeformatter(string strKeyPublic, string strHashbyteDeformatter, byte[] DeformatterData)
        {
            try
            {
                byte[] HashbyteDeformatter;
                HashbyteDeformatter = Convert.FromBase64String(strHashbyteDeformatter);
                System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();
                strKeyPublic = regBitStrength.Replace(strKeyPublic, "");
                RSA.FromXmlString(strKeyPublic);
                System.Security.Cryptography.RSAPKCS1SignatureDeformatter RSADeformatter = new System.Security.Cryptography.RSAPKCS1SignatureDeformatter(RSA);
                //指定解密的时候HASH算法为MD5 
                RSADeformatter.SetHashAlgorithm("MD5");
                if (RSADeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// RSA签名验证
        /// </summary>
        /// <param name="strKeyPublic">公钥</param>
        /// <param name="HashbyteDeformatter">Hash描述</param>
        /// <param name="strDeformatterData">签名后的结果</param>
        /// <returns></returns>
        public static bool SignatureDeformatter(string strKeyPublic, byte[] HashbyteDeformatter, string strDeformatterData)
        {
            try
            {
                byte[] DeformatterData;
                System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();
                strKeyPublic = regBitStrength.Replace(strKeyPublic, "");
                RSA.FromXmlString(strKeyPublic);
                System.Security.Cryptography.RSAPKCS1SignatureDeformatter RSADeformatter = new System.Security.Cryptography.RSAPKCS1SignatureDeformatter(RSA);
                //指定解密的时候HASH算法为MD5 
                RSADeformatter.SetHashAlgorithm("MD5");
                DeformatterData = Convert.FromBase64String(strDeformatterData);
                if (RSADeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// RSA签名验证
        /// </summary>
        /// <param name="strKeyPublic">公钥</param>
        /// <param name="strHashbyteDeformatter">Hash描述</param>
        /// <param name="strDeformatterData">签名后的结果</param>
        /// <returns></returns>
        public static bool SignatureDeformatter(string strKeyPublic, string strHashbyteDeformatter, string strDeformatterData)
        {
            try
            {
                byte[] DeformatterData;
                byte[] HashbyteDeformatter;
                HashbyteDeformatter = Convert.FromBase64String(strHashbyteDeformatter);
                System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();
                strKeyPublic = regBitStrength.Replace(strKeyPublic, "");
                RSA.FromXmlString(strKeyPublic);
                System.Security.Cryptography.RSAPKCS1SignatureDeformatter RSADeformatter = new System.Security.Cryptography.RSAPKCS1SignatureDeformatter(RSA);
                //指定解密的时候HASH算法为MD5 
                RSADeformatter.SetHashAlgorithm("MD5");
                DeformatterData = Convert.FromBase64String(strDeformatterData);
                if (RSADeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// RSA签名验证
        /// </summary>
        /// <param name="pubRSA">公钥 RSA</param>
        /// <param name="strHashbyteDeformatter">Hash描述</param>
        /// <param name="strDeformatterData">签名后的结果</param>
        /// <returns></returns>
        public static bool SignatureDeformatter(System.Security.Cryptography.RSACryptoServiceProvider pubRSA, string strHashbyteDeformatter, string strDeformatterData)
        {
            try
            {
                byte[] DeformatterData;
                byte[] HashbyteDeformatter;
                HashbyteDeformatter = Convert.FromBase64String(strHashbyteDeformatter);
                System.Security.Cryptography.RSAPKCS1SignatureDeformatter RSADeformatter = new System.Security.Cryptography.RSAPKCS1SignatureDeformatter(pubRSA);
                //指定解密的时候HASH算法为MD5 
                RSADeformatter.SetHashAlgorithm("MD5");
                DeformatterData = Convert.FromBase64String(strDeformatterData);
                if (RSADeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #endregion


        #region 创建加解密RSA


        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="rsaCryptoServiceProvider">rsa类</param>
        /// <param name="p_inputString">需要加密的字符串</param>
        /// <returns>加密后的文本信息</returns>
        public static string EncryptString(RSACryptoServiceProvider rsaCryptoServiceProvider, string p_inputString)
        {
            int keySize = rsaCryptoServiceProvider.KeySize / 8;
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
        /// <param name="rsaCryptoServiceProvider">rsa类</param>
        /// <param name="inputString">需要解密的字符串信息</param>
        /// <returns>解密后的文本信息</returns>
        public static string DecryptString(RSACryptoServiceProvider rsaCryptoServiceProvider, string inputString)
        {
            int dwKeySize = rsaCryptoServiceProvider.KeySize;
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
        /// 创建加密RSA
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <returns></returns>
        public RSACryptoServiceProvider CreatePublicKeyRSA(string publicKey)
        {
            try
            {
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSA.FromXmlString(publicKey);
                return RSA;
            }
            catch (CryptographicException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 创建解密RSA
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        public RSACryptoServiceProvider CreatePrivateKeyRSA(string privateKey)
        {
            try
            {
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSA.FromXmlString(privateKey);
                return RSA;
            }
            catch (CryptographicException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据安全证书创建加密RSA
        /// </summary>
        /// <param name="certfile">公钥文件</param>
        /// <returns></returns>
        public RSACryptoServiceProvider X509CertFileCreatePublicKeyRSA(string certfile)
        {
            try
            {
                X509Certificate2 x509Cert = new X509Certificate2(certfile);
                RSACryptoServiceProvider RSA = (RSACryptoServiceProvider)x509Cert.PublicKey.Key;
                return RSA;
            }
            catch (CryptographicException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据私钥文件创建私钥RSA
        /// </summary>
        /// <param name="keyfile">私钥文件</param>
        /// <param name="password">访问含私钥文件的密码</param>
        /// <returns></returns>
        public RSACryptoServiceProvider X509CertFileCreatePrivateKeyRSA(string keyfile, string password)
        {
            try
            {
                X509Certificate2 x509Cert = new X509Certificate2(keyfile, password);
                RSACryptoServiceProvider RSA = (RSACryptoServiceProvider)x509Cert.PrivateKey;
                return RSA;
            }
            catch (CryptographicException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据安全证书创建加密RSA
        /// </summary>
        /// <param name="certName">证书名</param>
        /// <returns></returns>
        public static RSACryptoServiceProvider X509CertCreatePublicKeyRSA(string certName)
        {
            X509Certificate2 x509Cert = GetCertificate(certName);
            if (x509Cert == null)
            {
                throw new Exception("证书不存在！");
            }
            RSACryptoServiceProvider RSA = (RSACryptoServiceProvider)x509Cert.PublicKey.Key;
            return RSA;

        }

        /// <summary>
        /// 根据安全证书创建私钥RSA
        /// </summary>
        /// <param name="certName">证书名</param>
        /// <returns></returns>
        public static RSACryptoServiceProvider X509CertCreatePrivateKeyRSA(string certName)
        {

            X509Certificate2 x509Cert = GetCertificate(certName);
            if (x509Cert == null)
            {
                throw new Exception("证书不存在！");
            }
            RSACryptoServiceProvider RSA = (RSACryptoServiceProvider)x509Cert.PrivateKey;
            return RSA;

        }

        private static X509Certificate2 GetCertificate(string CertName)
        {
            //声明X509Store对象，指定存储区的名称和存储区的类型
            //StoreName中定义了系统默认的一些存储区的逻辑名称
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            //以只读的方式打开这个存储区，OpenFlags定义的打开的方式
            store.Open(OpenFlags.ReadOnly);
            //获取这个存储区中的数字证书的集合
            X509Certificate2Collection certCol = store.Certificates;
            //查找满足证书名称的证书并返回
            foreach (X509Certificate2 cert in certCol)
            {
                if (cert.SubjectName.Name == "CN=" + CertName)
                {
                    store.Close();
                    return cert;
                }
            }
            store.Close();
            return null;
        }

        #endregion
    }
}
