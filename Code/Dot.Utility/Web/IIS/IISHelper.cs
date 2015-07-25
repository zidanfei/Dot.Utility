using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.DirectoryServices;
using System.Collections;

namespace Dot.Utility.Web.IIS
{
    public class IISHelper
    {

        private string _HostName = "localhost";
        public string HostName
        {
            get
            {
                return this._HostName;
            }
            set
            {
                this._HostName = value;
            }
        }

        private string _UserName;
        public string UserName
        {
            get
            {
                return this._UserName;
            }
            set
            {
                this._UserName = value;
            }
        }

        private string _Password;
        public string Password
        {
            get
            {
                return this._Password;
            }
            set
            {
                this._Password = value;
            }
        }

        public IISHelper()
        {
        }

        public IISHelper(string HostName, string UserName, string Password)
        {
            this._HostName = HostName;
            this._UserName = UserName;
            this._Password = Password;
        }




        public DirectoryEntry GetDirectoryEntry(string EntryPath)
        {
            DirectoryEntry entry;
            if (UserName == null)
            {
                entry = new DirectoryEntry(EntryPath);
            }
            else
            {
                entry = new DirectoryEntry(
    EntryPath,
    this.UserName,
    this.Password,
    AuthenticationTypes.Secure);
            }
            return entry;
        }




        public void CreateWebSite(WebSiteInfo siteInfo)
        {
            if (!EnsureNewSiteEnavaible(siteInfo.BindString))
            {
                throw new Exception("已经有了这样的网站了。" + Environment.NewLine + siteInfo.BindString);
            }
            string entPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry rootEntry = GetDirectoryEntry(entPath);
            string newSiteNum = GetNewWebSiteID();
            DirectoryEntry newSiteEntry = rootEntry.Children.Add(newSiteNum, "IIsWebServer");
            newSiteEntry.CommitChanges();
            newSiteEntry.Properties["ServerBindings"].Value = siteInfo.BindString;
            newSiteEntry.Properties["ServerComment"].Value = siteInfo.CommentOfWebSite;
            newSiteEntry.Properties["AccessRead"][0] = true;
            newSiteEntry.CommitChanges();
            DirectoryEntry vdEntry = newSiteEntry.Children.Add("root", "IIsWebVirtualDir");
            vdEntry.CommitChanges();
            vdEntry.Properties["Path"].Value = siteInfo.WebPath;
            vdEntry.CommitChanges();
        }

        public void DeleteWebSiteByName(string siteName)
        {
            WebSiteAdmin site = this.GetWebSite(siteName);
            if (site != null)
            {
                string siteEntPath = String.Format("IIS://{0}/w3svc/{1}", HostName, site.WebSiteNumber);
                DirectoryEntry siteEntry = GetDirectoryEntry(siteEntPath);
                string rootPath = String.Format("IIS://{0}/w3svc", HostName);
                DirectoryEntry rootEntry = GetDirectoryEntry(rootPath);
                rootEntry.Children.Remove(siteEntry);
                rootEntry.CommitChanges();
            }
        }




        public bool EnsureNewSiteEnavaible(string bindStr)
        {
            string entPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry ent = GetDirectoryEntry(entPath);
            foreach (DirectoryEntry child in ent.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    if (child.Properties["ServerBindings"].Value != null)
                    {
                        if (child.Properties["ServerBindings"].Value.ToString() == bindStr)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }




        public WebSiteAdmin GetWebSite(string SiteName)
        {
            Regex regex = new Regex(SiteName);
            string entPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry ent = GetDirectoryEntry(entPath);
            foreach (DirectoryEntry child in ent.Children)
            {
                if (
                    child.SchemaClassName == "IIsWebServer" &&
                    child.Properties["ServerComment"].Value != null &&
                    regex.Match(child.Properties["ServerComment"].Value.ToString()).Success)
                {
                    return new WebSiteAdmin(this.HostName, child.Name);
                }
            }
            return null;
        }

        public WebSiteAdmin GetWebSite(int Port)
        {
            Regex regex = new Regex(Port.ToString());
            string entPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry ent = GetDirectoryEntry(entPath);
            foreach (DirectoryEntry child in ent.Children)
            {
                if (
                    child.SchemaClassName == "IIsWebServer" &&
                    child.Properties["ServerBindings"].Value != null &&
                    regex.Match(child.Properties["ServerBindings"].Value.ToString()).Success)
                {
                    return new WebSiteAdmin(this.HostName, child.Name);
                }
            }
            return null;
        }




        public string GetNewWebSiteID()
        {
            ArrayList list = new ArrayList();
            string tmpStr;
            string entPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry ent = GetDirectoryEntry(entPath);
            foreach (DirectoryEntry child in ent.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    tmpStr = child.Name.ToString();
                    list.Add(Convert.ToInt32(tmpStr));
                }
            }
            list.Sort();
            int i = 1;
            foreach (int j in list)
            {
                if (i == j)
                {
                    i++;
                }
            }
            return i.ToString();
        }


    }
}
