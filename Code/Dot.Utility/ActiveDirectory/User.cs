using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Dot.Utility.ActiveDirectory
{
    [DataContract]
    [Serializable]
    public class User
    {
        private string dbName;
        /// <summary>
        /// 1 Exchange用户邮箱数据库名称
        /// </summary>
        [DataMember]
        public string DBName
        {
            get { return dbName; }
            set { dbName = value; }
        }


        private byte[] photo;
        /// <summary>
        /// 2
        /// </summary>
        [DataMember]
        public byte[] Photo
        {
            get { return photo; }
            set { photo = value; }
        }

        private string photoPath;
        /// <summary>
        /// 3
        /// </summary>
        [DataMember]
        public string PhotoPath
        {
            get { return photoPath; }
            set { photoPath = value; }
        }


        private string nickName;
        /// <summary>
        /// 4 xchange用户邮箱别名
        /// </summary>
        [DataMember]
        public string NickName
        {
            get { return nickName; }
            set { nickName = value; }
        }

        private string oldLoginName;
        /// <summary>
        /// 5
        /// </summary>
        [DataMember]
        public string OldLoginName
        {
            get { return oldLoginName; }
            set { oldLoginName = value; }
        }

        private string oldLoginName2000;
        /// <summary>
        /// 6
        /// </summary>
        [DataMember]
        public string OldLoginName2000
        {
            get { return oldLoginName2000; }
            set { oldLoginName2000 = value; }
        }


        private bool isCanUpdatePWD;
        /// <summary>
        /// 7
        /// </summary>
        [DataMember]
        public bool IsCanUpdatePWD
        {
            get { return isCanUpdatePWD; }
            set { isCanUpdatePWD = value; }
        }

        //cbCreateMail
        private bool isCreateMail;
        /// <summary>
        /// 8
        /// </summary>
        [DataMember]
        public bool IsCreateMail
        {
            get { return isCreateMail; }
            set { isCreateMail = value; }
        }

        private List<string> otherIpTel = new List<string>();
        /// <summary>
        /// 9
        /// </summary>
        [DataMember]
        public List<string> OtherIpTel
        {
            get { return otherIpTel; }
            set { otherIpTel = value; }
        }

        private List<string> otherFax = new List<string>();
        /// <summary>
        /// 10
        /// </summary>
        [DataMember]
        public List<string> OtherFax
        {
            get { return otherFax; }
            set { otherFax = value; }
        }

        private List<string> otherCellPhone = new List<string>();
        /// <summary>
        /// 11
        /// </summary>
        [DataMember]
        public List<string> OtherCellPhone
        {
            get { return otherCellPhone; }
            set { otherCellPhone = value; }
        }

        private List<string> otherCall = new List<string>();
        /// <summary>
        /// 12
        /// </summary>
        [DataMember]
        public List<string> OtherCall
        {
            get { return otherCall; }
            set { otherCall = value; }
        }

        private List<string> otherHomePhone = new List<string>();
        /// <summary>
        /// 13
        /// </summary>
        [DataMember]
        public List<string> OtherHomePhone
        {
            get { return otherHomePhone; }
            set { otherHomePhone = value; }
        }
        //pager
        private string call;
        /// <summary>
        /// 14
        /// </summary>
        [DataMember]
        public string Call
        {
            get { return call; }
            set { call = value; }
        }

        private string profile;
        /// <summary>
        /// 15
        /// </summary>
        [DataMember]
        public string Profile
        {
            get { return profile; }
            set { profile = value; }
        }

        private string loginScript;
        /// <summary>
        /// 16
        /// </summary>
        [DataMember]
        public string LoginScript
        {
            get { return loginScript; }
            set { loginScript = value; }
        }


        private bool isAccountExpried;
        /// <summary>
        /// 17
        /// </summary>
        [DataMember]
        public bool IsAccountExpried
        {
            get { return isAccountExpried; }
            set { isAccountExpried = value; }
        }


        private bool isReversible;
        /// <summary>
        /// 18
        /// </summary>
        [DataMember]
        public bool IsReversible
        {
            get { return isReversible; }
            set { isReversible = value; }
        }

        private string badPasswordTime;
        /// <summary>
        /// 19
        /// </summary>
        [DataMember]
        public string BadPasswordTime
        {
            get { return badPasswordTime; }
            set { badPasswordTime = value; }
        }

        private string badPasswordCount;
        /// <summary>
        /// 20
        /// </summary>
        [DataMember]
        public string BadPasswordCount
        {
            get { return badPasswordCount; }
            set { badPasswordCount = value; }
        }

        private string canonicalName;
        /// <summary>
        /// 21
        /// </summary>
        [DataMember]
        public string CanonicalName
        {
            get { return canonicalName; }
            set { canonicalName = value; }
        }

        private string distinguishedName;
        [DataMember]
        public string DistinguishedName
        {
            get { return distinguishedName; }
            set { distinguishedName = value; }
        }
        private string postOffice;
        /// <summary>
        /// 22
        /// </summary>
        [DataMember]
        public string PostOffice
        {
            get { return postOffice; }
            set { postOffice = value; }
        }



        private string id;
        /// <summary>
        /// 23
        /// </summary>
        [DataMember]
        public string ID
        {
            get { return id; }
            set { id = value; }
        }


        private string employeeID;
        /// <summary>
        /// 24
        /// </summary>
        [DataMember]
        public string EmployeeID
        {
            get { return employeeID; }
            set { employeeID = value; }
        }
        //登录名
        private string itcode;
        /// <summary>
        /// 25 登录名
        /// </summary>
        [DataMember]
        public string Itcode
        {
            get { return itcode; }
            set { itcode = value; }
        }
        //2000登录名
        private string loginName2000;
        /// <summary>
        /// 26
        /// </summary>
        [DataMember]
        public string LoginName2000
        {
            get { return loginName2000; }
            set { loginName2000 = value; }
        }

        private string name;
        /// <summary>
        /// 27
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string company;
        /// <summary>
        /// 28
        /// </summary>
        [DataMember]
        public string Company
        {
            get { return company; }
            set { company = value; }
        }

        private string password;
        /// <summary>
        /// 29
        /// </summary>
        [DataMember]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        private string passwordConfirm;
        /// <summary>
        /// 30
        /// </summary>
        [DataMember]
        public string PasswordConfirm
        {
            get { return passwordConfirm; }
            set { passwordConfirm = value; }
        }

        private string tel;
        /// <summary>
        /// 31
        /// </summary>
        [DataMember]
        public string Tel
        {
            get { return tel; }
            set { tel = value; }
        }

        private string manager;
        /// <summary>
        /// 32
        /// </summary>
        [DataMember]
        public string Manager
        {
            get { return manager; }
            set { manager = value; }
        }

        private string desciption;
        /// <summary>
        /// 33
        /// </summary>
        [DataMember]
        public string Desciption
        {
            get { return desciption; }
            set { desciption = value; }
        }

        private string outDate;
        /// <summary>
        /// 34
        /// </summary>
        [DataMember]
        public string OutDate
        {
            get { return outDate; }
            set { outDate = value; }
        }

        private bool isLock;
        /// <summary>
        /// 35
        /// </summary>
        [DataMember]
        public bool IsLock
        {
            get { return isLock; }
            set { isLock = value; }
        }

        private bool isUpdatePwd;
        /// <summary>
        /// 36
        /// </summary>
        public bool IsEsitPWD = false;
        /// <summary>
        /// 37
        /// </summary>
        [DataMember]
        public bool IsUpdatePwd
        {
            get { return isUpdatePwd; }
            set { isUpdatePwd = value; }
        }

        private string email;
        /// <summary>
        /// 38
        /// </summary>
        [DataMember]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        private bool isDiai;
        /// <summary>
        /// 39
        /// </summary>
        [DataMember]
        public bool IsDiai
        {
            get { return isDiai; }
            set { isDiai = value; }
        }

        private bool accountState;
        /// <summary>
        /// 40
        /// </summary>
        public bool IsEsitSTATE = false;
        /// <summary>
        /// 41
        /// </summary>
        [DataMember]
        public bool AccountState
        {
            get { return accountState; }
            set { accountState = value; }
        }


        private bool isExpried;
        /// <summary>
        /// 42
        /// </summary>
        [DataMember]
        public bool IsExpried
        {
            get { return isExpried; }
            set { isExpried = value; }
        }

        private List<string> memberof;
        /// <summary>
        /// 43
        /// </summary>
        [DataMember]
        public List<string> Memberof
        {
            get { return memberof; }
            set { memberof = value; }
        }

        private string department;
        /// <summary>
        /// 44
        /// </summary>
        [DataMember]
        public string Department
        {
            get { return department; }
            set { department = value; }
        }

        private string accountCreatTime;
        /// <summary>
        /// 45
        /// </summary>
        [DataMember]
        public string AccountCreatTime
        {
            get { return accountCreatTime; }
            set { accountCreatTime = value; }
        }

        private string loginCount;
        /// <summary>
        /// 46
        /// </summary>
        [DataMember]
        public string LoginCount
        {
            get { return loginCount; }
            set { loginCount = value; }
        }

        private string lastLoginTime;
        /// <summary>
        /// 47 最后登录时间
        /// </summary>
        [DataMember]
        public string LastLoginTime
        {
            get { return lastLoginTime; }
            set { lastLoginTime = value; }
        }

        private string lastSetPwd;
        /// <summary>
        /// 48
        /// </summary>
        [DataMember]
        public string LastSetPwd
        {
            get { return lastSetPwd; }
            set { lastSetPwd = value; }
        }

        private string homeDirectory;
        /// <summary>
        /// 49
        /// </summary>
        [DataMember]
        public string HomeDirectory
        {
            get { return homeDirectory; }
            set { homeDirectory = value; }
        }

        private string homeDrive;
        /// <summary>
        /// 50
        /// </summary>
        [DataMember]
        public string HomeDrive
        {
            get { return homeDrive; }
            set { homeDrive = value; }
        }

        private string cN;
        /// <summary>
        /// 51
        /// </summary>
        [DataMember]
        public string CN
        {
            get { return cN; }
            set { cN = value; }
        }

        //姓
        private string familyName;
        /// <summary>
        /// 52
        /// </summary>
        [DataMember]
        public string FamilyName
        {
            get { return familyName; }
            set { familyName = value; }
        }

        //名
        private string lastName;
        /// <summary>
        /// 53
        /// </summary>
        [DataMember]
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }


        //英文缩写
        private string enName;
        /// <summary>
        /// 54
        /// </summary>
        [DataMember]
        public string EnName
        {
            get { return enName; }
            set { enName = value; }
        }
        //显示名称
        private string showName;
        /// <summary>
        /// 55
        /// </summary>
        [DataMember]
        public string ShowName
        {
            get { return showName; }
            set { showName = value; }
        }

        //显示名称
        private string office;
        /// <summary>
        /// 56
        /// </summary>
        [DataMember]
        public string Office
        {
            get { return office; }
            set { office = value; }
        }

        //职务
        private string duty;
        /// <summary>
        /// 57
        /// </summary>
        [DataMember]
        public string Duty
        {
            get { return duty; }
            set { duty = value; }
        }

        //网页
        private string page;
        /// <summary>
        /// 58
        /// </summary>
        [DataMember]
        public string Page
        {
            get { return page; }
            set { page = value; }
        }

        //家庭电话
        private string famliyTel;
        /// <summary>
        /// 59 家庭电话
        /// </summary>
        [DataMember]
        public string FamliyTel
        {
            get { return famliyTel; }
            set { famliyTel = value; }
        }

        //移动电话
        private string cellphone;
        /// <summary>
        /// 60
        /// </summary>
        [DataMember]
        public string Cellphone
        {
            get { return cellphone; }
            set { cellphone = value; }
        }

        //传真
        private string fax;
        /// <summary>
        /// 61
        /// </summary>
        [DataMember]
        public string Fax
        {
            get { return fax; }
            set { fax = value; }
        }

        //IP电话
        private string ipTel;
        /// <summary>
        /// 62
        /// </summary>
        [DataMember]
        public string IPTel
        {
            get { return ipTel; }
            set { ipTel = value; }
        }
        //修改时间
        private string modifyDate;
        /// <summary>
        /// 63
        /// </summary>
        [DataMember]
        public string ModifyDate
        {
            get { return modifyDate; }
            set { modifyDate = value; }
        }

        //OU名称
        private string ouName;
        /// <summary>
        /// 64
        /// </summary>
        [DataMember]
        public string OUName
        {
            get { return ouName; }
            set { ouName = value; }
        }
        //街道
        private string street;
        /// <summary>
        /// 65
        /// </summary>
        [DataMember]
        public string Street
        {
            get { return street; }
            set { street = value; }
        }
        //城市
        private string city;
        /// <summary>
        /// 66
        /// </summary>
        [DataMember]
        public string City
        {
            get { return city; }
            set { city = value; }
        }
        //省
        private string province;
        /// <summary>
        /// 67
        /// </summary>
        [DataMember]
        public string Province
        {
            get { return province; }
            set { province = value; }
        }
        //邮编
        private string zip;
        /// <summary>
        /// 68
        /// </summary>
        [DataMember]
        public string ZIP
        {
            get { return zip; }
            set { zip = value; }
        }
        //邮编
        private string memo;
        /// <summary>
        /// 69
        /// </summary>
        [DataMember]
        public string Memo
        {
            get { return memo; }
            set { memo = value; }
        }
    }
}
