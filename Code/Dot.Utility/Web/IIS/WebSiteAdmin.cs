using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Utility.Web.IIS
{
    public class WebSiteAdmin
    {
        protected System.DirectoryServices.DirectoryEntry RootFolder;
        public WebSiteAdmin(string Server, string WebSiteNumber)
        {
            _Server = Server;
            _WebSiteNumber = WebSiteNumber;

            string strPath = "IIS://" + _Server + "/W3SVC/" + this.WebSiteNumber + "/ROOT";
            try
            {
                this.RootFolder = new System.DirectoryServices.DirectoryEntry(strPath);
            }
            catch (Exception e)
            {
                throw new Exception("Can't connect to the server [" + _Server + "] ...", e);
            }
        }


        //定义需要使用的
        private string _Server;
        //Server属性定义访问机器的名字，可以是IP与计算名
        public string Server
        {
            get
            {
                return _Server;
            }
        }




        //定义需要使用的
        private string _WebSiteNumber;
        //WebSite属性定义，为一数字，为方便，使用string 
        //一般来说第一台主机为1,第二台主机为2，依次类推
        public string WebSiteNumber
        {
            get
            {
                return _WebSiteNumber;
            }
        }




        private VirtualDirectories _VirtualDirectories;
        public VirtualDirectories VirDirs
        {
            get
            {
                if (this._VirtualDirectories == null)
                {
                    this._VirtualDirectories = new VirtualDirectories();
                    foreach (System.DirectoryServices.DirectoryEntry de in this.RootFolder.Children)
                    {
                        if (de.SchemaClassName == "IIsWebVirtualDir")
                        {
                            VirtualDirectory vd = new VirtualDirectory(de.Name);

                            vd.AuthFlags = (int)de.Properties["AuthFlags"][0];
                            vd.DefaultLogonDomain = (string)de.Properties["DefaultLogonDomain"].Value;
                            vd.AccessFlags = (int)de.Properties["AccessFlags"].Value;
                            vd.AccessSSLFlagValue = (int)de.Properties["AccessSSLFlags"].Value;
                            vd.DirBrowseFlagValue = (int)de.Properties["DirBrowseFlags"].Value;

                            vd.AnonymousUserName = (string)de.Properties["AnonymousUserName"][0];
                            vd.AnonymousUserPass = (string)de.Properties["AnonymousUserName"][0];
                            vd.ContentIndexed = (bool)de.Properties["ContentIndexed"][0];
                            vd.EnableDefaultDoc = (bool)de.Properties["EnableDefaultDoc"][0];
                            vd.EnableDirBrowsing = (bool)de.Properties["EnableDirBrowsing"][0];
                            vd.Path = (string)de.Properties["Path"][0];
                            vd.flag = 0;
                            vd.DefaultDoc = (string)de.Properties["DefaultDoc"][0];
                            this._VirtualDirectories.Add(vd.Name, vd);
                        }
                    }
                }
                return this._VirtualDirectories;
            }
            set
            {
                this._VirtualDirectories = value;
            }
        }




        public void Start()
        {
            this.RootFolder.Invoke("Start", new object[] { });
        }

        public void Stop()
        {
            this.RootFolder.Invoke("Stop", new object[] { });
        }



        //判断是否存这个虚拟目录
        public bool Exists(string strVirdir)
        {
            return this.VirDirs.Contains(strVirdir);
        }

        //添加一个虚拟目录
        public void Create(VirtualDirectory newdir)
        {
            string strPath = "IIS://" + _Server + "/W3SVC/" + this.WebSiteNumber + "/ROOT/" + newdir.Name;
            if (!this.VirDirs.Contains(newdir.Name))
            {
                //加入到ROOT的Children集合中去
                System.DirectoryServices.DirectoryEntry newVirDir = this.RootFolder.Children.Add(newdir.Name, "IIsWebVirtualDir");
                newVirDir.Invoke("AppCreate", true);
                newVirDir.CommitChanges();
                this.RootFolder.CommitChanges();
                //然后更新数据
                UpdateDirInfo(newVirDir, newdir);
            }
            else
            {
                throw new Exception("This virtual directory is already exist.");
            }
        }

        //得到一个虚拟目录
        public VirtualDirectory GetVirDir(string strVirdir)
        {
            VirtualDirectory tmp = null;
            if (this.VirDirs.Contains(strVirdir))
            {
                tmp = this.VirDirs.Find(strVirdir);
                ((VirtualDirectory)this.VirDirs[strVirdir]).flag = 2;
            }
            return tmp;
        }

        //更新一个虚拟目录
        public void Update(VirtualDirectory dir)
        {
            //判断需要更改的虚拟目录是否存在
            if (this.VirDirs.Contains(dir.Name))
            {
                System.DirectoryServices.DirectoryEntry ode = this.RootFolder.Children.Find(dir.Name, "IIsWebVirtualDir");
                UpdateDirInfo(ode, dir);
            }
            else
            {
                throw new Exception("This virtual directory is not exists.");
            }
        }

        //删除一个虚拟目录
        public void Delete(string strVirdir)
        {
            if (this.VirDirs.Contains(strVirdir))
            {
                object[] paras = new object[2];
                paras[0] = "IIsWebVirtualDir"; //表示操作的是虚拟目录
                paras[1] = strVirdir;
                this.RootFolder.Invoke("Delete", paras);
                this.RootFolder.CommitChanges();
            }
            else
            {
                throw new Exception("Can't delete " + strVirdir + ",because it isn't exists.");
            }
        }

        //批量更新
        public void UpdateBatch()
        {
            foreach (object item in this.VirDirs.Values)
            {
                VirtualDirectory vd = (VirtualDirectory)item;
                switch (vd.flag)
                {
                    case 0:
                        break;
                    case 1:
                        Create(vd);
                        break;
                    case 2:
                        Update(vd);
                        break;
                }
            }
        }

        //更新东东
        private void UpdateDirInfo(System.DirectoryServices.DirectoryEntry de, VirtualDirectory vd)
        {
            de.Properties["AuthFlags"][0] = vd.AuthFlags;
            de.Properties["DefaultLogonDomain"].Value = vd.DefaultLogonDomain;
            de.Properties["AccessFlags"].Value = vd.AccessFlags;
            de.Properties["AccessSSLFlags"].Value = vd.AccessSSLFlagValue;
            de.Properties["DirBrowseFlags"].Value = vd.DirBrowseFlagValue;

            if (vd.AnonymousUserName != null)
            {
                de.Properties["AnonymousUserName"][0] = vd.AnonymousUserName;
            }
            if (vd.AnonymousUserPass != null)
            {
                de.Properties["AnonymousUserPass"][0] = vd.AnonymousUserPass;
            }
            de.Properties["ContentIndexed"][0] = vd.ContentIndexed;
            de.Properties["EnableDefaultDoc"][0] = vd.EnableDefaultDoc;
            de.Properties["EnableDirBrowsing"][0] = vd.EnableDirBrowsing;
            de.Properties["DefaultDoc"][0] = vd.DefaultDoc;
            de.Properties["Path"][0] = vd.Path;

            de.CommitChanges();
        }
    }
}
