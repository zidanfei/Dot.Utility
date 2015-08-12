using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Threading;
using Dot.Utility.Log;


namespace Dot.Utility.Net
{
    /// <summary>
    /// HttpHelper class
    /// </summary>
    public class HttpHelper
    {
        public HttpHelper()
        {
            ChatSet = "UTF8";
        }

        public HttpHelper(string chatset = "UTF8")
        {
            ChatSet = chatset;
        }

        public string ChatSet { get; set; }

        // 无 referer 的 POST  // by hook.hu@gmail.com
        public string Post(string url, string content)
        {
            return Post(url, content, "");
        }

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
        public string Get(string url, string referer, string userAgent = null, CookieCollection cookies = null)
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
                    req.CookieContainer = new CookieContainer();
                    req.CookieContainer.Add(cookies);
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



        #region Member Fields
        private CookieContainer _cc = new CookieContainer();
        private WebProxy _proxy;

        private int _delayTime;
        private int _timeout = 120000; // The default is 120000 milliseconds (120 seconds).
        private int _tryTimes = 3; // 默认重试3次
        #endregion
    }
}
