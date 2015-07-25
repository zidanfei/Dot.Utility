using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Dot.Utility.ActiveDirectory
{
    [Serializable]
    [DataContract]
    public class Computer
    {
        public Computer() { }
        //CanonicalName

        private string canonicalName;
        [DataMember]
        public string CanonicalName
        {
            get { return canonicalName; }
            set { canonicalName = value; }
        }

        private string ouID;
        [DataMember]
        public string OuID
        {
            get { return ouID; }
            set { ouID = value; }
        }

        private string id;
        [DataMember]
        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        private string name;
        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string name2000;
        [DataMember]
        public string Name2000
        {
            get { return name2000; }
            set { name2000 = value; }
        }

        private string ouName;
        [DataMember]
        public string OuName
        {
            get { return ouName; }
            set { ouName = value; }
        }

        private string desc;
        [DataMember]
        public string Desc
        {
            get { return desc; }
            set { desc = value; }
        }

        private bool state;
        [DataMember]
        public bool State
        {
            get { return state; }
            set { state = value; }
        }

        private string beforeName;
        [DataMember]
        public string BeforeName
        {
            get { return beforeName; }
            set { beforeName = value; }
        }

        private string newOU;
        [DataMember]
        public string NewOU
        {
            get { return newOU; }
            set { newOU = value; }
        }

        private List<string> memberOf;
        [DataMember]
        public List<string> MemberOf
        {
            get { return memberOf; }
            set { memberOf = value; }
        }

        private List<string> print;
        [DataMember]
        public List<string> Print
        {
            get { return print; }
            set { print = value; }
        }

        private string dns;
        [DataMember]
        public string DNS
        {
            get { return dns; }
            set { dns = value; }

        }

        private string site;
        [DataMember]
        public string Site
        {
            get { return site; }
            set { site = value; }

        }

        //操作系统
        private string system;
        [DataMember]
        public string System
        {
            get { return system; }
            set { system = value; }

        }

        //版本
        private string version;
        [DataMember]
        public string Version
        {
            get { return version; }
            set { version = value; }

        }

        private string pack;
        [DataMember]
        public string Pack
        {
            get { return pack; }
            set { pack = value; }

        }

        private string modifyDate;
        [DataMember]
        public string ModifyDate
        {
            get { return modifyDate; }
            set { modifyDate = value; }

        }

        private string createDate;
        [DataMember]
        public string CreateDate
        {
            get { return createDate; }
            set { createDate = value; }

        }

        private string oldName;
        [DataMember]
        public string OldName
        {
            get { return oldName; }
            set { oldName = value; }

        }

        private string oldName2000;
        [DataMember]
        public string OldName2000
        {
            get { return oldName2000; }
            set { oldName2000 = value; }

        }

        private string oldDNS;
        [DataMember]
        public string OldDNS
        {
            get { return oldDNS; }
            set { oldDNS = value; }

        }

        private string oldDesc;
        [DataMember]
        public string OldDesc
        {
            get { return oldDesc; }
            set { oldDesc = value; }

        }
        private bool oldState;
        [DataMember]
        public bool OldState
        {
            get { return oldState; }
            set { oldState = value; }
        }
    }
}
