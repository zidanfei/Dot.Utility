using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Threading;
using Dot.Utility.Log;
using System.Net.Http;
using System.Linq;

namespace Dot.Utility.Net
{
    /// <summary>
    /// HttpHelper class
    /// </summary>
    public class HttpHelper
    {
        public HttpHelper()
        {
            ChatSet = "UTF-8";
        }

        public HttpHelper(string chatset = "UTF-8")
        {
            ChatSet = chatset;
        }

        public string ChatSet { get; set; }


        public static CookieContainer LoginCookies = new CookieContainer();

        /// <summary>
        /// Send post data to server
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns>Return content</returns>
        public string Post(string url, string content, string referer = null, string contentType = "application/x-www-form-urlencoded", CookieContainer cookies = null)
        {
            int failedTimes = _tryTimes;
            while (failedTimes-- > 0)
            {
                try
                {
                    if (_delayTime > 0)
                    {
                        Thread.Sleep(_delayTime * 1000);
                    }
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(new Uri(url));
                    if (cookies != null)
                        req.CookieContainer = cookies;
                    else
                        req.CookieContainer = _cc;
                    req.Referer = referer;
                    byte[] buff = Encoding.GetEncoding(ChatSet).GetBytes(content);
                    req.Method = "POST";
                    req.Timeout = _timeout;
                    req.ContentType = contentType;
                    req.ContentLength = buff.Length;
                    if (null != _proxy && null != _proxy.Credentials)
                    {
                        req.UseDefaultCredentials = true;
                    }
                    req.Proxy = _proxy;
                    //req.Connection = "Keep-Alive";
                    Stream reqStream = req.GetRequestStream();
                    reqStream.Write(buff, 0, buff.Length);
                    reqStream.Close();

                    //接收返回字串
                    HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                    StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding(ChatSet));
                    return sr.ReadToEnd();
                }
                catch (Exception e)
                {
                    LogFactory.ExceptionLog.Error("HTTP POST Error: " + e.Message);
                    LogFactory.ExceptionLog.Error("Url: " + url);
                    LogFactory.ExceptionLog.Error("Data: " + content);
                }
            }

            return string.Empty;
        }

        public string Login(string url, IDictionary<string, string> parameters, out CookieContainer credentials)
        {
            StringBuilder buffer = new StringBuilder();
            if (parameters != null && parameters.Count > 0)
            {
                int i = 0;
                foreach (var item in parameters)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", item.Key, item.Value);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", item.Key, item.Value);
                    }
                    i++; 
                }
            }
            return Login(url, buffer.ToString(), out credentials);
        }

        /// <summary>
        /// Send post data to server
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="credentials">cookie容器</param>
        /// <returns>Return content</returns>
        public string Login(string url, string content, out CookieContainer credentials)
        {
            string contentType = "application/x-www-form-urlencoded";
            credentials = new CookieContainer();
            //contentType = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            int failedTimes = _tryTimes;
            while (failedTimes-- > 0)
            {
                try
                {
                    if (_delayTime > 0)
                    {
                        Thread.Sleep(_delayTime * 1000);
                    }
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(new Uri(url));
                    req.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; Maxthon; .NET CLR 1.1.4322)";
                    byte[] buff = Encoding.GetEncoding(ChatSet).GetBytes(content);
                    req.KeepAlive = true;
                    req.Method = "POST";
                    req.Timeout = _timeout;
                    req.ContentType = contentType;
                    req.ContentLength = buff.Length;
                    req.AllowAutoRedirect = false;
                    if (null != _proxy && null != _proxy.Credentials)
                    {
                        req.UseDefaultCredentials = true;
                    }
                    else
                    {
                        req.Credentials = CredentialCache.DefaultCredentials;
                    }
                    req.Proxy = _proxy;
                    req.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                    //req.Connection = "Keep-Alive";
                    Stream reqStream = req.GetRequestStream();
                    reqStream.Write(buff, 0, buff.Length);
                    reqStream.Close();

                    //接收返回字串
                    HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                    var co = res.GetResponseHeader("Set-Cookie");
                    if (!string.IsNullOrEmpty(co))
                    {
                        credentials.SetCookies(new Uri(url), co);
                    }

                    StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding(ChatSet));
                    var result = sr.ReadToEnd();
                    try
                    {
                        res.Close();
                        //string _APSNET_SessionValue = res.Cookies["ASP.NET_SessionId"].Value;

                    }
                    catch (WebException ex)
                    {
                        throw ex;
                    }

                    return result;
                }
                catch (Exception e)
                {
                    LogFactory.ExceptionLog.Error("HTTP POST Error: " + e.Message);
                    LogFactory.ExceptionLog.Error("Url: " + url);
                    LogFactory.ExceptionLog.Error("Data: " + content);
                }
            }

            return string.Empty;
        }

        // 无 referer 的 POST  // by hook.hu@gmail.com
        public string Get(string url)
        {
            return Get(url, "");
        }

        /// <summary>
        /// Get data from server
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Return content</returns>
        public string Get(string url, string referer, string userAgent = null, CookieContainer cookies = null)
        {
            int failedTimes = _tryTimes;

            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(new Uri(url));
                req.Referer = referer;

                req.Method = "GET";
                req.Timeout = _timeout;
                if (null != _proxy && null != _proxy.Credentials)
                {
                    req.UseDefaultCredentials = true;
                }
                req.Proxy = _proxy;
                if (!string.IsNullOrEmpty(userAgent))
                {
                    req.UserAgent = userAgent;
                }
                if (cookies != null)
                {
                    //req.CookieContainer = new CookieContainer();
                    req.CookieContainer = (cookies);
                }

                //接收返回字串
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding(ChatSet));
                return sr.ReadToEnd();
            }
            catch (Exception e)
            {
                return string.Format("[Exception]\r\nHTTP GET Error:{0} \r\nUrl:{1}", e.Message, url);
                //TraceLog.Error("HTTP GET Error: " + e.Message);
                //TraceLog.Error("Url: " + url);
            }
        }

        /// <summary>
        /// Set Proxy
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        public void SetProxy(string server, int port, string username, string password)
        {
            if (null != server && port > 0)
            {
                _proxy = new WebProxy(server, port);
                if (null != username && null != password)
                {
                    _proxy.Credentials = new NetworkCredential(username, password);
                    _proxy.BypassProxyOnLocal = true;
                }
            }
        }

        /// <summary>
        /// Set delay connect time
        /// </summary>
        /// <param name="delayTime"></param>
        public void SetDelayConnect(int delayTime)
        {
            _delayTime = delayTime;
        }

        /// <summary>
        /// Set the timeout for each http request
        /// </summary>
        /// <param name="timeout"></param>
        public void SetTimeOut(int timeout)
        {
            if (timeout > 0)
            {
                _timeout = timeout;
            }
        }

        /// <summary>
        /// Set the try times for each http request
        /// </summary>
        /// <param name="timeout"></param>
        public void SetTryTimes(int times)
        {
            if (times > 0)
            {
                _tryTimes = times;
            }
        }



        public void Download(string url, string localfile)
        {
            //WebClient client = new WebClient();
            //client.DownloadFile(url, localfile);

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "GET";
            using (WebResponse wr = req.GetResponse())
            {
                HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();
                var strpath = myResponse.ResponseUri.ToString();
                WebClient client = new WebClient();
                client.DownloadFile(strpath, localfile);
            }
        }

        #region Form认证

        /// <summary>
        /// 获取凭据
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="url"></param>
        /// <param name="cookieName"></param>
        /// <returns></returns>
        public string GetSecurityToken(string userName, string password, string url, string cookieName)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                Dictionary<string, string> credential = new Dictionary<string, string>();
                credential.Add("UserName", userName);
                credential.Add("Password", password);
                HttpResponseMessage response = httpClient.PostAsync(url, new FormUrlEncodedContent(credential)).Result;
                IEnumerable<string> cookies;
                if (response.Headers.TryGetValues("Set-Cookie", out cookies))
                {
                    string token = cookies.FirstOrDefault(value => value.StartsWith(cookieName));
                    if (null != token)
                    {
                        return token.Split(';')[0].Substring(cookieName.Length + 1);
                    }
                }
                return null;
            }
        }

        public string Get(string userName, string password, string loginUrl, string url, string cookieName = ".ASPXAUTH")
        {
            string token = GetSecurityToken(userName, password, loginUrl, cookieName);
            string address = url;
            if (!string.IsNullOrEmpty(token))
            {
                HttpClientHandler handler = new HttpClientHandler { CookieContainer = new CookieContainer() };
                handler.CookieContainer.Add(new Uri(url), new Cookie(cookieName, token));
                using (HttpClient httpClient = new HttpClient(handler))
                {
                    HttpResponseMessage response = httpClient.GetAsync(address).Result;
                    IEnumerable<string> userNames = response.Content.ReadAsAsync<IEnumerable<string>>().Result;
                    foreach (string un in userNames)
                    {
                        return un;
                    }
                }
            }
            return null;

        }
        #endregion


        #region Member Fields
        private CookieContainer _cc = new CookieContainer();
        private WebProxy _proxy;

        private int _delayTime;
        private int _timeout = 120000; // The default is 120000 milliseconds (120 seconds).
        private int _tryTimes = 3; // 默认重试3次
        #endregion
    }
}
