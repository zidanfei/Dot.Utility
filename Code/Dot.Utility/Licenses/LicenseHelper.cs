/**
 * 许可以文件License.lic存信息
 * 
 * **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dot.Utility.Cryptography;
using Dot.Utility.Xml;

namespace Dot.Utility.Licenses
{
    /// <summary>
    /// 生成license
    /// </summary>
    public class LicenseHelper
    {
        public static string _pubKey = "<BitStrength>1024</BitStrength><RSAKeyValue><Modulus>qE+ix5HNyTHtTx33L391LnMY1YiWjx02Ef1RSoMDHKjFvojjkF5kQIdLa8wXtmV4rpxxp+YIWynsJKIEWUs1y8pKEqjNVqsQbHNuCLqiBGHqdceM6s/4Q+x1TbNOHKo/B/8RblMu5xHhNPzm0gwMN3fL3Ghl254ap6AI5VAAzFc=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        /// <summary>
        /// 是否有效License
        /// 此方法校验License是否有效，是否过期；
        /// 版本，人数第三系统自己校验
        /// 
        /// path默认路径dll文件夹下 config/License.lic
        /// pubKey 使用内置key
        /// </summary>
        /// <returns></returns>
        public static bool IsValidityLicense(out string message)
        {
            string path = DotEnvironment.MapDllPath("config/License.lic");
            return IsValidityLicense(path, _pubKey, out message);
        }
        /// <summary>
        /// 是否有效License
        /// 此方法校验License是否有效，是否过期；
        /// 版本，人数第三系统自己校验
        /// </summary>
        /// <param name="path">License路径</param>
        /// <param name="pubKey"></param>
        /// <returns></returns>
        public static bool IsValidityLicense(string path, string pubKey, out string message)
        {
            LicenseInfo info = XmlHelper.LoadConfig<LicenseInfo>(path);
            if (info == null)
                throw new Exceptions.DotException("LicenseInfo 不能为空！");
            return IsValidityLicense(info, pubKey, out message);
        }


        /// <summary>
        /// 是否有效License
        /// 此方法校验License是否有效，是否过期；是否申请的机器
        /// 版本，人数第三系统自己校验
        /// </summary>
        /// <param name="info"></param>
        /// <param name="pubKey"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool IsValidityLicense(LicenseInfo info, string pubKey, out string message)
        {
            message = string.Empty;
            //string key = iWS.Utility.Config.ConfigHelper.GetString("License_PublicKey");
            //if (!string.IsNullOrEmpty(key))
            //    pubKey = key;
            string hashData;
            info.MachineCode = MachineCode.GetMachineCode();
            string machineCode = MachineCode.GetMachineCode();           
            RSACryption.GetHash(info.ToString(), out hashData);
            if (RSACryption.SignatureDeformatter(pubKey, hashData, info.SignatureInfo))
            {
                if (info.ExpireDate < DateTime.Now)
                {
                    message = "您的 license 已过期，请联系供应商，获取新的 license！";
                    return false;
                }
                if (info.IssuedDate > DateTime.Now)
                {
                    message = "您的 license 已过期，请联系供应商，获取新的 license！";
                    return false;
                }
                return true;
            }
            message = "无效License！";
            return false;
        }
        /// <summary>
        /// 是否有效License
        /// 此方法校验License是否有效，是否过期；是否申请的机器
        /// 版本，人数第三系统自己校验
        /// </summary>
        /// <param name="info"></param>
        /// <param name="certName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool IsValidityLicenseByCert( string certName, out string message)
        {
            LicenseInfo license = XmlHelper.LoadConfig<LicenseInfo>(DotEnvironment.MapDllPath("config/license.lic"));
            return IsValidityLicenseByCert(license, certName, out message);
             

        }
        /// <summary>
        /// 是否有效License
        /// 此方法校验License是否有效，是否过期；是否申请的机器
        /// 版本，人数第三系统自己校验
        /// </summary>
        /// <param name="info"></param>
        /// <param name="certName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool IsValidityLicenseByCert(LicenseInfo info, string certName, out string message)
        {
            message = string.Empty;
            string cert = Config.ConfigHelper.GetAppSettingOrDefault("License_CertName");
            if (!string.IsNullOrEmpty(cert))
                certName = cert;
            string hashData;
            info.MachineCode = MachineCode.GetMachineCode();
            RSACryption.GetHash(info.ToString(), out hashData);
            var pubcert = RSACryption.X509CertCreatePublicKeyRSA(certName);
            if (RSACryption.SignatureDeformatter(pubcert, hashData, info.SignatureInfo))
            {
                if (info.ExpireDate < DateTime.Now)
                {
                    message = "您的 license 已过期，请联系供应商，获取新的 license！";
                    return false;
                }
                if (info.IssuedDate > DateTime.Now)
                {
                    message = "您的 license 已过期，请联系供应商，获取新的 license！";
                    return false;
                }
                return true;
            }
            message = "无效License！";
            return false;
        }
    }
}
