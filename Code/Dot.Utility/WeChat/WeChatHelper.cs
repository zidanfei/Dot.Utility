//using Dot.Utility.Net;
//using Dot.Utility.Web;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web;

//namespace Dot.Utility.WeChat
//{
//    public class WeChatHelper
//    {
//        public static string AppId
//        {
//            get
//            {
//                return Config.ConfigHelper.GetAppSettingOrDefault("appId", null);

//            }
//        }


//        public static string AppSecret
//        {
//            get
//            {
//                return Config.ConfigHelper.GetAppSettingOrDefault("appSecret", null);
//            }
//        }

//        public static string GetToken()
//        {
//            HttpHelper helper = new HttpHelper();
//            string url = string.Format(@"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", AppId, AppSecret);
//            string result = helper.Get(url, null);
//            return result;
//        }

//        public static string GetTickect(string acessToken)
//        {
//            HttpHelper helper = new HttpHelper();
//            string url = string.Format(@"https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", acessToken);
//            string result = helper.Get(url, null);
//            return result;
//        }

//        public static void SetToken()
//        {
//            JObject joToken = (JObject)JsonConvert.DeserializeObject(GetToken());
//            HttpContext.Current.Application["access_token"] = joToken["access_token"];
//            HttpContext.Current.Application["TokenExpire"] = joToken["expires_in"];
//            HttpContext.Current.Application["TokenBeginTime"] = DateTime.Now;
//        }

//        public static void SetTicket()
//        {

//            if (HttpContext.Current.Application["ticket"] == null || (HttpContext.Current.Application["ticket"] != null
//           && ((DateTime)HttpContext.Current.Application["ticketBeginTime"]).AddSeconds(
//           double.Parse(HttpContext.Current.Application["ticketExpire"].ToString()) - 10).CompareTo(DateTime.Now) < 1))
//            {

//                JObject joTickect = (JObject)JsonConvert.DeserializeObject(GetTickect(HttpContext.Current.Application["access_token"].ToString()));

//                HttpContext.Current.Application["ticket"] = joTickect["ticket"];
//                HttpContext.Current.Application["ticketExpire"] = joTickect["expires_in"];
//                HttpContext.Current.Application["ticketBeginTime"] = DateTime.Now;

//            }

//        }

//        /// <summary>
//        /// 从微信服务器下载文件
//        /// </summary>
//        /// <param name="media_id">The media_id.</param>
//        /// <param name="saveDirPath">保存文件夹.</param>
//        /// <param name="downloadUrlBase">http相对路径.</param>
//        /// <returns></returns>
//        public string UpLoadImage(string media_id, string saveDirPath, string downloadUrlBase)
//        {
//            JObject resultJobject = new JObject();
//            try
//            {
//                SetToken();
//                string access_token = HttpContext.Current.Application["access_token"].ToString();
//                string content = string.Empty;
//                string strpath = string.Empty;
//                string savepath = string.Empty;
//                string stUrl = string.Format("http://file.api.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}", access_token, media_id);

//                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(stUrl);
//                req.Method = "GET";
//                using (WebResponse wr = req.GetResponse())
//                {
//                    HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();
//                    strpath = myResponse.ResponseUri.ToString();
//                    WebClient mywebclient = new WebClient();
//                    savepath = saveDirPath + media_id + ".jpg";

//                    mywebclient.DownloadFile(strpath, savepath);
//                    resultJobject["result"] = true;
//                    resultJobject["file"] = downloadUrlBase + media_id + ".jpg";
//                    resultJobject["reason"] = "";
//                }
//            }
//            catch (Exception ex)
//            {
//                resultJobject["result"] = false;
//                resultJobject["reason"] = ex.ToString();
//            }
//            return resultJobject.ToString();

//        }
//    }
//}
