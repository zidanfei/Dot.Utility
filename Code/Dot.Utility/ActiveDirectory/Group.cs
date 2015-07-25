using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Dot.Utility.ActiveDirectory
{
    [Serializable]
    [DataContract]
    public class Group
    {
        private string type;
        [DataMember]
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private string scope;
        [DataMember]
        public string Scope
        {
            get { return scope; }
            set { scope = value; }
        }

        private string canonicalName;
        [DataMember]
        public string CanonicalName
        {
            get { return canonicalName; }
            set { canonicalName = value; }
        }

        private string id;
        [DataMember]
        public string ID
        {
            get { return id; }
            set { id = value; }
        }
        private string ouId;
        [DataMember]
        public string OuID
        {
            get { return ouId; }
            set { ouId = value; }
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

        private List<string> memberOf;
        [DataMember]
        public List<string> MemberOf
        {
            get { return memberOf; }
            set { memberOf = value; }
        }

        private string desc;
        [DataMember]
        public string Desc
        {
            get { return desc; }
            set { desc = value; }
        }


        private string ouName;
        [DataMember]
        public string OuName
        {
            get { return ouName; }
            set { ouName = value; }
        }


        private string newName;
        [DataMember]
        public string NewName
        {
            get { return newName; }
            set { newName = value; }
        }

        private string email;
        [DataMember]
        public string Email
        {
            get { return email; }
            set { email = value; }

        }

        private string info;
        [DataMember]
        public string Info
        {
            get { return info; }
            set { info = value; }

        }

    }
}
