using Dot.Utility.Config;
using Dot.Utility.Exceptions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Security
{
    ///3.如何创建证书：
    ///makecert.exe -sr LocalMachine -ss My -a sha1 -n CN=iWSProdCert -sky exchange -pe     (服务端证书）
    ///-n 证书主题名称
    /// -pe 将生成的私钥标记为可以导出
    /// -sr location 数字证书存储位置 CurrentUser，LocalMachine
    /// -ss store：数字证书的存储区
    /// -sky ：指定秘钥类型1signature交换秘钥 2exchange 签名秘钥；
    /// 
    ///http://blog.csdn.net/jayzai/article/details/7654246
    /// <summary>
    /// 用户名密码验证
    /// </summary>
    public class CustomX509UserNamePasswordValidator : System.IdentityModel.Selectors.UserNamePasswordValidator
    {
        /// <summary>
        /// 验证连接
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        public override void Validate(string userName, string password)
        {
            // validate arguments
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");
            if (null == ConfigHelper.GetAppSettingOrDefault("WCFUserName"))
                throw new ConfigException("缺少配置WCFUserName");
            if (null == ConfigHelper.GetAppSettingOrDefault("WCFPassword"))
                throw new ConfigException("缺少配置WCFPassword");

            if (userName.Equals(ConfigHelper.GetAppSettingOrDefault("WCFUserName"), StringComparison.OrdinalIgnoreCase)
                && password == ConfigHelper.GetAppSettingOrDefault("WCFPassword"))
            {
            }
            else
            {
                throw new SecurityTokenException("用户名或者密码错误！");
            }

        }

    }
   

    /// <summary>
    /// 证书验证
    /// </summary>
    public class CustomX509CertificateValidator2 : X509CertificateValidator
    {
        /// <summary>
        /// Validates a certificate.
        /// </summary>
        /// <param name="certificate">The certificate the validate.</param>
        public override void Validate(X509Certificate2 certificate)
        {
            // validate argument
            if (certificate == null)
                throw new ArgumentNullException("X509认证证书为空！");

            // check if the name of the certifcate matches
            if (null != ConfigHelper.GetAppSettingOrDefault("CertName")
                && certificate.SubjectName.Name != ConfigHelper.GetAppSettingOrDefault("CertName"))
                throw new SecurityTokenValidationException("Certificated was not issued by thrusted issuer");
        }

    }

    /// <summary>
    /// 证书验证
    /// </summary>
    public class CustomX509CertificateValidator : X509CertificateValidator
    {
        public override void Validate(X509Certificate2 certificate)
        {
            if (certificate == null)
                throw new ArgumentNullException("X509认证证书为空！");
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadWrite);
            X509Certificate2Collection fcollection = (X509Certificate2Collection)store.Certificates;
            if (string.IsNullOrWhiteSpace( ConfigHelper.GetAppSettingOrDefault("ClientCertName")))
                throw new ConfigException("缺少配置ClientCertName");
            string ClientCertName = ConfigHelper.GetAppSettingOrDefault("ClientCertName");
            if (string.IsNullOrWhiteSpace(ClientCertName))
                throw new ConfigException("缺少配置ClientCertName");
            X509Certificate2Collection collection = fcollection.Find(X509FindType.FindBySubjectName, ClientCertName, false);
            if (collection.Count > 0)
            {
                X509Certificate2 clientCertificate = new X509Certificate2(collection[0]);
                if (!certificate.Thumbprint.Equals(clientCertificate.Thumbprint, StringComparison.CurrentCultureIgnoreCase))
                    throw new SecurityTokenException("客户端证书没有验证通过！");
            }
            else
            {
                throw new SecurityTokenException("X509认证证书为空！");
            }
        }
    }

}
