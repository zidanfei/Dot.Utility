using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Dot.Utility.ActiveDirectory
{
    [Serializable]
    [DataContract]
    public class OU
    {
        #region 构造函数

        public OU(object name, object importId, object importParentId, DataState state)
            : this(name, importId, importParentId)
        {
            State = state;
        }

        public OU(object name, object importId, object importParentId)
            : this(importId, importParentId)
        {
            if (name != null)
            {
                Name = name.ToString();
            }
        }

        public OU(object importId, object importParentId)
        {
            if (null == importId)
                throw new ArgumentNullException("importId");
            Name = importId.ToString();
            ImportId = importId.ToString();
            ImportParentId = importParentId.ToString();
        }
        #endregion

        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string Name { set; get; }
        [DataMember]
        public string CanonicalName { set; get; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Country { set; get; }
        [DataMember]
        public string Province { set; get; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string Street { set; get; }
        [DataMember]
        public string Zip { set; get; }
        [DataMember]
        public string OUpath { set; get; }

        public DataState State { get; set; }

        public string ImportId { get; set; }

        public string ImportParentId { get; set; }


        public Dictionary<string, string> propertys = new Dictionary<string, string>();
        public string this[string prop]
        {
            get
            {
                if (propertys.ContainsKey(prop))
                {
                    return propertys[prop];
                }
                return null;
            }
            set
            {
                if (propertys.ContainsKey(prop))
                {
                    propertys[prop] = value;
                }
                propertys.Add(prop, value);
            }
        }
    }
}
