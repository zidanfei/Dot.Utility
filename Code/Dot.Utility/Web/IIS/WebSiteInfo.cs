using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Utility.Web.IIS
{
    public class WebSiteInfo
    {
        private string hostIP; private int portNum; private string descOfWebSite; private string commentOfWebSite; private string webPath;
        public WebSiteInfo(string hostIP, int portNum, string descOfWebSite, string commentOfWebSite, string webPath)
        {
            this.hostIP = hostIP;
            this.portNum = portNum;
            this.descOfWebSite = descOfWebSite;
            this.commentOfWebSite = commentOfWebSite;
            this.webPath = webPath;
        }

        public string BindString
        {
            get
            {
                return String.Format("{0}:{1}:{2}", hostIP, portNum, descOfWebSite);
            }
        }

        public string CommentOfWebSite
        {
            get
            {
                return commentOfWebSite;
            }
        }

        public string WebPath
        {
            get
            {
                return webPath;
            }
        }
    }
}
