using Dot.Log;
using Dot.Utility.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Mail
{
    /*          var logo = new Attachment(fileStream, "onedocLogo.png");
                 logo.ContentId = "onedocLogo.png";
                 logo.ContentDisposition.Inline = true;
     */

    public interface IEmailHelper
    {
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        bool SendEmail(string body);

        /// <summary>
        /// Gets from.
        /// </summary>
        /// <value>
        /// From.
        /// </value>
        string From { get; }

        /// <summary>
        /// Gets to.
        /// </summary>
        /// <value>
        /// To.
        /// </value>
        string To { get; }
        /// <summary>
        /// Gets the cc.
        /// </summary>
        /// <value>
        /// The cc.
        /// </value>
        string Cc { get; }
        /// <summary>
        /// Gets the BCC.
        /// </summary>
        /// <value>
        /// The BCC.
        /// </value>
        string Bcc { get; }
        /// <summary>
        /// Gets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        string Subject { get; }
        /// <summary>
        /// Gets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        string Body { get; }

        /// <summary>
        /// Gets the attach file path list.
        /// </summary>
        /// <value>
        /// The attach file path list.
        /// </value>
        List<FileInfo> AttachFilePathList { get; }

        /// <summary>
        /// Gets the attach file stream list.
        /// </summary>
        /// <value>
        /// The attach file stream list.
        /// </value>
        //List<AttachFile> AttachFileStreamList { get; }

        /// <summary>
        /// Gets the attach file list.
        /// </summary>
        /// <value>
        /// The attach file list.
        /// </value>
        List<Attachment> AttachFileList { get; }

    }

    [DebuggerDisplay("From: {From},To: {To},Cc: {Cc},Bcc: {Bcc},Subject: {Subject},Body: {Body}")]
    public class EmailHelper : IEmailHelper
    {
        public EmailHelper(string subject, string to)
        {
            AttachFilePathList = new List<FileInfo>();
            AttachFileList = new List<Attachment>();
            //AttachFileStreamList = new List<AttachFile>();
            Subject = subject;
            To = to.Replace("; ", ",").Replace(" ", "");
        }

        public EmailHelper(string subject, string from, string to)
            : this(subject, to)
        {
            From = from;
        }

        public EmailHelper(string subject, string from, string to, string cc, string bcc = null)
            : this(subject, from, to)
        {
            Cc = cc.Replace("; ", ",").Replace(" ", "");
            if (!string.IsNullOrEmpty(bcc))
            {
                Bcc = bcc.Replace("; ", ",").Replace(" ", "");
            }
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public bool SendEmail(string body)
        {
            Body = body;
            return SendEmail();
        }

        public void AddAttachFile(FileInfo file)
        {
            AttachFilePathList.Add(file);
        }

        //public void AddAttachFile(string name, string body)
        //{
        //    AttachFileStreamList.Add(new AttachFile(name, body));
        //}

        //public void AddAttachFile(string name, FileStream fs)
        //{
        //    AttachFileStreamList.Add(new AttachFile(name, fs));
        //}

        public void SmtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {

            }
            else
            {
                if (e.Error == null)
                {

                }
                else
                {
                    throw new Exception("发送邮件失败" + e.Error.Message);
                }
            }
        }

        /// <summary>
        /// Separator, using to separate the To/CC/BCC/Attachment.
        /// </summary>
        char[] separator = { ',', ';' };

        bool SendEmail()
        {
            try
            {
                ValidateParameters();
                using (MailMessage mailMessage = new MailMessage())
                {
                    using (SmtpClient client = new SmtpClient())
                    {
                        client.Host = Host;
                        client.UseDefaultCredentials = false;
                        if (string.IsNullOrEmpty(CredentialUserName))
                        {
                            client.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                        }
                        else if (string.IsNullOrEmpty(CredentialDomain))
                        {
                            client.Credentials = new NetworkCredential(CredentialUserName, CredentialPassword);
                        }
                        else
                        {
                            client.Credentials = new NetworkCredential(CredentialUserName, CredentialPassword, CredentialDomain);
                        }

                        mailMessage.From = new MailAddress(From);
                        RefineReceiver(To, r => mailMessage.To.Add(r));
                        RefineReceiver(Cc, r => mailMessage.CC.Add(r));
                        RefineReceiver(Bcc, r => mailMessage.Bcc.Add(r));

                        AddAttachment(mailMessage, AttachFilePathList);

                        //AddAttachment(mailMessage, AttachFileStreamList);
                        foreach (Attachment item in AttachFileList)
                        {
                            mailMessage.Attachments.Add(item);
                        }

                        if (!PublishEnvironment.Equals("Dev", StringComparison.OrdinalIgnoreCase))
                        {
                            mailMessage.Subject = Subject;
                            mailMessage.Body = Body;
                        }
                        else
                        {
                            mailMessage.Body = "*** This email is being sent as a test. If you receive this e-mail, please safely ignore and sorry for the inconvenience! ***\r\n\r\n<br/>" + Body;
                            mailMessage.Subject = Subject + " (Test - please ignore)";
                        }

                        mailMessage.BodyEncoding = Encoding.UTF8;
                        mailMessage.IsBodyHtml = true;
                        mailMessage.Priority = MailPriority.Normal;
                        //#if !DEBUG
                        client.Send(mailMessage);
                        client.SendCompleted += SmtpClient_SendCompleted;
                        //#endif
                        LogFactory.BusinessLog.Info("Send Email [To:" + mailMessage.To.ToString() + "] [Subject: " + mailMessage.Subject.ToString() + "] [Body: " + mailMessage.Body.ToString() + "]");

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //throw ex;
                LogFactory.ExceptionLog.Error("Send Email", ex);
                return false;
            }
        }

        void RefineReceiver(string receiver, Action<string> addReceiverFunc)
        {
            if (!string.IsNullOrEmpty(receiver) && receiver.Length > 0)
            {
                try
                {
                    addReceiverFunc(string.Join(",", receiver.Split(separator, StringSplitOptions.RemoveEmptyEntries)));
                }
                catch
                {
                }
            }
        }

        void AddAttachment(MailMessage mailMessage, List<FileInfo> list)
        {
            if (null != list)
            {
                foreach (var file in list)
                {
                    if (file != null)
                    {

                        mailMessage.Attachments.Add(new Attachment(file.Open(FileMode.Open), file.Name));
                    }
                }
            }
        }

        //void AddAttachment(MailMessage mailMessage, List<Attachment> list)
        //{
        //    if (null != list)
        //    {
        //        foreach (var item in list)
        //        {
        //            if (!string.IsNullOrEmpty(item.Body) && !string.IsNullOrEmpty(item.Name))
        //            {
        //                AddAttachment(mailMessage, item.Body, item.Name);
        //            }
        //            else if (null != item.Content && !string.IsNullOrEmpty(item.Name))
        //            {
        //                AddAttachment(mailMessage, item.Content, item.Name);
        //            }
        //        }
        //    }
        //}

        //void AddAttachment(MailMessage mailMessage, FileStream content, string name)
        //{
        //    if (content != null && !string.IsNullOrEmpty(name))
        //    {
        //        mailMessage.Attachments.Add(new Attachment(content, name));
        //    }
        //}

        //void AddAttachment(MailMessage mailMessage, string body, string name)
        //{
        //    if (!string.IsNullOrEmpty(body) && !string.IsNullOrEmpty(name))
        //    {
        //        byte[] buf = new System.Text.UTF8Encoding().GetBytes(body);
        //        MemoryStream ms = new MemoryStream(buf);
        //        System.Net.Mime.ContentType contentType = new System.Net.Mime.ContentType("text/html");
        //        contentType.Name = name;
        //        Attachment fileAttachment = new Attachment(ms, contentType);
        //        mailMessage.Attachments.Add(fileAttachment);
        //    }
        //}

        void ValidateParameters()
        {
            if (string.IsNullOrEmpty(From))
            {
                throw new Exception("没有发送邮箱，可以通过配置项 EmailFrom 设置！");
            }
            if (string.IsNullOrEmpty(Host))
            {
                throw new Exception("没有 SMTP 事务的主机的名称或 IP 地址，可以通过配置项 EmailHost 设置！");
            }
            if (string.IsNullOrEmpty(To))
            {
                throw new Exception("没有收件人！");
            }
            if (string.IsNullOrEmpty(Subject))
            {
                throw new Exception("没有标题！");
            }


        }

        #region Properties

        private string emailFrom = ConfigHelper.GetValueOrDefault("EmailFrom");
        public string From
        {
            get
            {
                return emailFrom;
            }
            private set
            {
                emailFrom = value;
            }
        }

        /// <summary>
        /// 获取或设置接受邮件
        /// </summary>
        /// <value>
        /// To.
        /// </value>
        public string To
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取或设置抄送邮件
        /// </summary>
        /// <value>
        /// The cc.
        /// </value>
        public string Cc
        {
            get;
            private set;
        }
        /// <summary>
        /// 获取或设置秘密抄送邮件
        /// </summary>
        /// <value>
        /// The BCC.
        /// </value>
        public string Bcc
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取或设置接受邮件标题
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        public string Subject
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取或设置邮件主体
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public string Body
        {
            get;
            private set;
        }
        /// <summary>
        /// 获取或设置附件地址列表
        /// </summary>
        /// <value>
        /// The attach file path list.
        /// </value>
        public List<FileInfo> AttachFilePathList
        {
            get;
            private set;
        }

        //public List<AttachFile> AttachFileStreamList
        //{
        //    get;
        //    private set;
        //}
        /// <summary>
        /// 获取或设置附件列表
        /// </summary>
        /// <value>
        /// The attach file list.
        /// </value>
        public List<Attachment> AttachFileList
        {
            get;
            private set;
        }

        private string userName = ConfigHelper.GetValueOrDefault("CredentialUserName");

        /// <summary>
        /// 获取或设置发送邮件
        /// 可以使用应用程序配置项 CredentialUserName 进行配置。
        /// </summary>
        /// <value>
        /// The name of the credential user.
        /// </value>
        public virtual string CredentialUserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
            }
        }

        private string pwd = ConfigHelper.GetValueOrDefault("CredentialPassword");
        /// <summary>
        /// 获取或设置发送邮件密码.
        /// 可以使用应用程序配置项 CredentialPassword 进行配置。
        /// </summary>
        /// <value>
        /// The credential password.
        /// </value>
        public virtual string CredentialPassword
        {
            get
            {
                return pwd;
            }
            set
            {
                pwd = value;
            }
        }

        private string domain = ConfigHelper.GetValueOrDefault("CredentialDomain");

        /// <summary>
        /// Gets or sets the credential domain.
        /// 可以使用应用程序配置项 CredentialDomain 进行配置。
        /// </summary>
        /// <value>
        /// The credential domain.
        /// </value>
        public virtual string CredentialDomain
        {
            get
            {
                return domain;
            }
            set
            {
                domain = value;
            }
        }

        private string env = ConfigHelper.GetValueOrDefault("PublishEnvironment");

        /// <summary>
        ///  获取或设置部署环境，用于提醒是否测试邮件
        ///  可以使用应用程序配置项 PublishEnvironment 进行配置。
        /// </summary>
        /// <value>
        /// The publish environment.
        /// </value>
        public virtual string PublishEnvironment
        {
            get
            {
                return env;
            }
            set
            {
                env = value;
            }
        }

        private string host = ConfigHelper.GetValueOrDefault("EmailHost");
        /// <summary>
        /// 获取或设置用于 SMTP 事务的主机的名称或 IP 地址
        /// 可以使用应用程序配置项 PublishEnvironment 进行配置。
        /// 例如： SMTP.126.COM
        /// </summary>
        public virtual string Host
        {
            get
            {
                return host;
            }
            set
            {
                host = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// 附件
    /// </summary>
    //public class AttachFile
    //{
    //    public AttachFile(string name, string body)
    //    {
    //        Name = name;
    //        Body = body;
    //    }

    //    public AttachFile(string name, FileStream content)
    //    {
    //        Name = name;
    //        Content = content;
    //    }
    //    /// <summary>
    //    /// 附件名
    //    /// </summary>
    //    public string Name
    //    {
    //        get;
    //        set;
    //    }
    //    /// <summary>
    //    /// 附件内容
    //    /// </summary>
    //    public string Body
    //    {
    //        get;
    //        set;
    //    }
    //    /// <summary>
    //    /// 附件内容
    //    /// </summary>
    //    public FileStream Content
    //    {
    //        get;
    //        set;
    //    }

    //}
}
