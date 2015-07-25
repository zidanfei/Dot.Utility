using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Utility.Web.IIS
{
    public class VirtualDirectory
    {
        private bool _indexed, _endirbrow, _endefaultdoc;
        private string _ausername, _auserpass, _name, _path;
        private int _flag;
        private string _defaultdoc;
        public VirtualDirectory(string strVirDirName)
        {
            _name = strVirDirName;
            _indexed = false; _endirbrow = false; _endefaultdoc = false;
            _flag = 1;
            _defaultdoc = "default.htm,default.aspx,default.asp,index.htm";
            _path = "C:\\";
            _ausername = null; _auserpass = null;
        }

        public int flag
        {
            get { return _flag; }
            set { _flag = value; }
        }

        public bool ContentIndexed
        {
            get { return _indexed; }
            set { _indexed = value; }
        }
        public bool EnableDirBrowsing
        {
            get { return _endirbrow; }
            set { _endirbrow = value; }
        }
        public bool EnableDefaultDoc
        {
            get { return _endefaultdoc; }
            set { _endefaultdoc = value; }
        }
        public string Name
        {
            get { return _name; }
        }
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
        public string DefaultDoc
        {
            get { return _defaultdoc; }
            set { _defaultdoc = value; }
        }
        public string AnonymousUserName
        {
            get { return _ausername; }
            set { _ausername = value; }
        }
        public string AnonymousUserPass
        {
            get { return _auserpass; }
            set { _auserpass = value; }
        }

        private string _DefaultLogonDomain;
        public string DefaultLogonDomain
        {
            get
            {
                return this._DefaultLogonDomain;
            }
            set
            {
                this._DefaultLogonDomain = value;
            }
        }

        public const int MD_AUTH_ANONYMOUS = 0x00000001;
        public const int MD_AUTH_BASIC = 0x00000002;
        public const int MD_AUTH_NT = 0x00000004;
        private int _AuthFlags = 0x00000001;
        public int AuthFlags
        {
            get
            {
                return this._AuthFlags;
            }
            set
            {
                this._AuthFlags = value;
            }
        }


        private int _AccessFlags = 0x00000205;
        public const int MD_ACCESS_READ = 0x00000001;
        public const int MD_ACCESS_WRITE = 0x00000002;
        public const int MD_ACCESS_EXECUTE = 0x00000004;
        public const int MD_ACCESS_SOURCE = 0x00000010;
        public const int MD_ACCESS_SCRIPT = 0x00000200;
        public const int MD_ACCESS_NO_REMOTE_WRITE = 0x00000400;
        public const int MD_ACCESS_NO_REMOTE_READ = 0x00001000;
        public const int MD_ACCESS_NO_REMOTE_EXECUTE = 0x00002000;
        public const int MD_ACCESS_NO_REMOTE_SCRIPT = 0x00004000;
        public int AccessFlags
        {
            get
            {
                return this._AccessFlags;
            }
            set
            {
                this._AccessFlags = value;
            }
        }

        private int _AccessSSLFlagValue = 0x00000000;
        public int AccessSSLFlagValue
        {
            get
            {
                return this._AccessSSLFlagValue;
            }
            set
            {
                this._AccessSSLFlagValue = value;
            }
        }

        private int _DirBrowseFlagValue = 0;
        public const int MD_DIRBROW_SHOW_DATE = 0x00000002;
        public const int MD_DIRBROW_SHOW_TIME = 0x00000004;
        public const int MD_DIRBROW_SHOW_SIZE = 0x00000008;
        public const int MD_DIRBROW_SHOW_EXTENSION = 0x00000010;
        public const int MD_DIRBROW_LONG_DATE = 0x00000020;
        public const int MD_DIRBROW_LOADDEFAULT = 0x40000000;
        public int DirBrowseFlagValue
        {
            get
            {
                return this._DirBrowseFlagValue;
            }
            set
            {
                this._DirBrowseFlagValue = value;
            }
        }
    }

    public class VirtualDirectories : System.Collections.Hashtable
    {
        public VirtualDirectories()
        {
        }
        //添加新的方法 
        public VirtualDirectory Find(string strName)
        {
            return (VirtualDirectory)this[strName];
        }
    }
}
