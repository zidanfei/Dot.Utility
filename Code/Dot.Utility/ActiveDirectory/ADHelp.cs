//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.DirectoryServices;
//using System.Collections;
//using System.Security.Principal;
//using System.Text.RegularExpressions;
//using System.Data;
//using System.Configuration;
//using ActiveDs;
//using System.Web;


//namespace Dot.Utility
//{
//    class ADHelp
//    {
//        //private readonly ArchiveMemberDAL amDAL = new ArchiveMemberDAL();
//        //private static readonly ArchiveMemberDAL amDALs = new ArchiveMemberDAL();
//        public string DomainName { get; set; }

//        public string DOMAIN_PATH
//        {
//            get
//            {
//                return "";// amDAL.Load(100001).MemberValue;//域LDAP地址，例：LDAP://DC=SPS,DC=IWS,DC=COM
//            }
//        }
//        public string SUFFIX_PATH
//        {
//            get
//            {
//                return ""; //amDAL.Load(100002).MemberValue;//域LDAP地址(简写)，DC=SPS,DC=IWS,DC=COM
//            }
//        }
//        public string OLD_SUFFIX_PATH
//        {
//            get
//            {
//                return ""; //amDAL.Load(100003).MemberValue;//域地址(缩写)，SPS
//            }
//        }
//        public string DOMIN_PATH
//        {
//            get
//            {
//                return ""; //amDAL.Load(100004).MemberValue;
//            }
//        }

//        public string RootPath { get; set; }
//        //public string Exchange_Server
//        //{
//        //    get
//        //    {
//        //        return amDAL.Load(500001).MemberValue;
//        //    }
//        //}
//        //public string Exchange_UserName
//        //{
//        //    get
//        //    {
//        //        return amDAL.Load(500004).MemberValue;
//        //    }
//        //}
//        //public string Exchange_UserPassWd
//        //{
//        //    get
//        //    {
//        //        return amDAL.Load(500005).MemberValue;
//        //    }
//        //}

//        public string GetDomain()
//        {
//            return "@" + DOMIN_PATH;

//        }

//        //public ADHelp()
//        //{
//        //    DOMAIN_PATH = amDAL.Load(100001).MemberValue;//域LDAP地址，例：LDAP://DC=SPS,DC=IWS,DC=COM
//        //    SUFFIX_PATH = amDAL.Load(100002).MemberValue;//域LDAP地址(简写)，DC=SPS,DC=IWS,DC=COM
//        //    OLD_SUFFIX_PATH = amDAL.Load(100003).MemberValue;//域地址(缩写)，SPS
//        //    DOMIN_PATH = amDAL.Load(100004).MemberValue;//域地址(标准)，SPS.IWS.COM
//        //}
//        #region OUEntry
//        /// <summary>
//        /// 创建组织单元
//        /// </summary>
//        /// <param name="entry"></param>
//        /// <param name="ouname"></param>
//        /// <returns></returns>
//        public static DirectoryEntry CreateOU(DirectoryEntry entry, string ouname)
//        {
//            DirectoryEntry OU = entry.Children.Add("OU=" + ouname, "organizationalUnit");
//            OU.CommitChanges();
//            return OU;
//        }
//        /// <summary>
//        /// 返回DirectoryEntry
//        /// </summary>
//        /// <param name="path">域地址</param>
//        /// <returns></returns>
//        public static DirectoryEntry GetDirectoryEntry(string path)
//        {
//            DirectoryEntry de = new DirectoryEntry();
//            de.Path = "LDAP://" + path;
//            return de;
//        }

//        /// <summary>
//        /// 返回DirectoryEntry
//        /// </summary>
//        /// <returns></returns>
//        public DirectoryEntry GetRootEntry()
//        {
//            DirectoryEntry de = new DirectoryEntry();
//            de.Path = RootPath;// amDAL.Load(100001).MemberValue;
//            return de;
//        }
//        /// <summary>
//        /// 返回查找到的第一个OU
//        /// </summary>
//        /// <param name="entryName"></param>
//        /// <returns></returns>
//        public DirectoryEntry GetOUEntry(string entryName)
//        {
//            DirectoryEntry de = GetRootEntry();
//            DirectorySearcher ds = new DirectorySearcher(de);
//            ds.Filter = "(&(objectclass=organizationalUnit)(OU=" + entryName + "))";
//            SearchResult sr = ds.FindOne();
//            if (sr != null)
//            {
//                return sr.GetDirectoryEntry();
//            }
//            return null;
//        }
//        public static DirectoryEntry GetOUEntry(DirectoryEntry rootEntry, string entryName)
//        {
//            DirectorySearcher ds = new DirectorySearcher(rootEntry);
//            ds.Filter = "(&(objectclass=organizationalUnit)(OU=" + entryName + "))";
//            SearchResult sr = ds.FindOne();
//            if (sr != null)
//            {
//                return sr.GetDirectoryEntry();
//            }
//            return null;
//        }
//        /// <summary>
//        /// 返回查找到的所有子OU
//        /// </summary>
//        /// <param name="entry"></param>
//        /// <returns></returns>
//        public static DataTable GetAllOUName(DirectoryEntry entry)
//        {
//            DataTable dt = new DataTable();
//            dt.Columns.Add("name", typeof(string));
//            dt.Columns.Add("oupath", typeof(string));
//            foreach (DirectoryEntry Dir in entry.Children)
//            {
//                if (Dir.SchemaClassName == "organizationalUnit")
//                {
//                    DataRow dr = dt.NewRow();
//                    dr["name"] = Dir.Properties["name"].Value.ToString();
//                    dr["oupath"] = Dir.Properties["distinguishedName"].Value.ToString();
//                    dt.Rows.Add(dr);
//                }
//            }
//            return dt;
//        }

//        /// <summary>
//        /// 根据查询条件查询指定OU数据的DataTable
//        /// </summary>
//        /// <param name="Filter">LDAP语法的查询语句，外层必须包括一层括号</param>
//        /// <param name="rootEntry">根</param>
//        /// <returns>包括查询数据的DataTable</returns>
//        public DataTable SearchAllOU(string Filter, DirectoryEntry rootEntry)
//        {
//            DataTable dtable = new DataTable();
//            dtable.Columns.Add("name", typeof(string));
//            dtable.Columns.Add("distinguishedName", typeof(string));
//            dtable.Columns.Add("objectGUID", typeof(Guid));

//            //DirectoryEntry de = GetRootEntry();
//            DirectorySearcher ds = new DirectorySearcher(rootEntry);
//            ds.SearchScope = SearchScope.Subtree;
//            ds.CacheResults = false;
//            ds.PropertiesToLoad.Add("name");
//            ds.PropertiesToLoad.Add("distinguishedName");
//            ds.PropertiesToLoad.Add("objectGUID");
//            ds.Filter = "(&(objectclass=organizationalUnit)" + Filter + ")";
//            ds.PageSize = 500;
//            ds.Sort = new SortOption("name", SortDirection.Ascending);
//            SearchResultCollection con = ds.FindAll();
//            for (int i = 0; i < con.Count; i++)
//            {
//                DataRow dr = dtable.NewRow();
//                dr["name"] = con[i].Properties["name"][0].ToString();

//                //IADs iu = (IADs)con[i].GetDirectoryEntry().NativeObject;
//                //iu.GetInfoEx(new object[]{ "canonicalName" }, 0);

//                string distinguishedName = con[i].Properties["distinguishedName"][0].ToString();
//                //distinguishedName = distinguishedName.Replace("DC=", "").Replace("OU=", "").Replace(amDAL.Load(100004).MemberValue.ToLower().Replace(".", ","), "");
//                //string[] strDNArr = distinguishedName.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
//                //string canonicalName = amDAL.Load(100004).MemberValue.ToLower() + "/";
//                //for (int j = strDNArr.Length - 1; j >= 0; j--)
//                //{
//                //    canonicalName += strDNArr[j] + "/";
//                //}
//                //canonicalName = canonicalName.Substring(0, canonicalName.Length - 1);

//                //string canonicalName = con[i].GetDirectoryEntry().Properties["canonicalName"].Value.ToString();

//                //dr["canonicalName"] = canonicalName.Replace("\\", string.Empty);
//                dr["distinguishedName"] = distinguishedName;
//                dr["objectGUID"] = new Guid((Byte[])con[i].Properties["objectGUID"][0]);
//                dtable.Rows.Add(dr);
//            }
//            return dtable;
//        }
//        #endregion

//        #region Group
//        ///// <summary>
//        ///// 返回查找到的所有Group
//        ///// </summary>
//        ///// <param name="rootEntry"></param>
//        ///// <param name="entryName"></param>
//        ///// <returns></returns>
//        //public static SearchResultCollection GetAllGroupName(DirectoryEntry Entry)
//        //{
//        //DataTable dt = new DataTable();
//        //dt.Columns.Add("name", typeof(string));
//        //dt.Columns.Add("oupath", typeof(string));
//        //foreach (DirectoryEntry Dir in Entry.Children)
//        //{
//        //    if (Dir.SchemaClassName == "Group")
//        //    {
//        //        DataRow dr = dt.NewRow();
//        //        dr["name"] = Dir.Properties["name"].Value.ToString();
//        //        dr["oupath"] = Dir.Properties["distinguishedName"].Value.ToString();
//        //        dt.Rows.Add(dr);
//        //    }
//        //}
//        //return dt;
//        //SearchResultCollection src = null;
//        //DirectorySearcher searcher = new DirectorySearcher(entry);
//        //searcher.Filter = "(&(objectClass=group))";
//        //searcher.SearchScope = SearchScope.Subtree;
//        //src = searcher.FindAll();
//        //src
//        //return src;
//        //}

//        /// <summary>
//        /// 创建用户组
//        /// </summary>
//        /// <param name="entry"></param>
//        /// <param name="groupname"></param>
//        /// <param name="name2000"></param>
//        /// <returns></returns>
//        public static DirectoryEntry CreateGroup(DirectoryEntry entry, string groupname, string name2000)
//        {
//            DirectoryEntry group = entry.Children.Add("CN=" + ProcLDAPStr(groupname), "group");
//            group.Properties["name"].Value = groupname;
//            group.Properties["sAMAccountName"].Value = name2000;
//            group.Properties["groupType"].Value = ActiveDs.ADS_GROUP_TYPE_ENUM.ADS_GROUP_TYPE_UNIVERSAL_GROUP | ActiveDs.ADS_GROUP_TYPE_ENUM.ADS_GROUP_TYPE_SECURITY_ENABLED;
//            group.CommitChanges();
//            return group;
//        }

//        public static string ProcLDAPStr(string LDAPStr)
//        {
//            if (!string.IsNullOrEmpty(LDAPStr))
//            {
//                LDAPStr = LDAPStr.Replace("#", "\\#");
//                LDAPStr = LDAPStr.Replace("\"", "\\\"");
//                return LDAPStr;
//            }
//            else
//                return LDAPStr;
//        }
//        /// <summary>
//        /// 根据组名返回用户组
//        /// </summary>
//        /// <param name="entry"></param>
//        /// <param name="groupname"></param>
//        /// <returns></returns>
//        public static DirectoryEntry GetGroup(DirectoryEntry entry, string groupname)
//        {
//            if (string.IsNullOrEmpty(groupname))
//            {
//                return null;
//            }
//            DirectorySearcher searcher = new DirectorySearcher(entry);
//            searcher.Filter = "(&(objectClass=group)(name=" + groupname + "))";
//            searcher.CacheResults = false;
//            searcher.PropertyNamesOnly = true;
//            searcher.PropertiesToLoad.Add("cn");
//            searcher.PropertiesToLoad.Add("distinguishedName");
//            searcher.PropertiesToLoad.Add("Description");
//            searcher.PropertiesToLoad.Add("memberOf");

//            SearchResult result = searcher.FindOne();
//            DirectoryEntry group = null;
//            if (result != null)
//            {
//                group = result.GetDirectoryEntry();
//            }
//            entry.Dispose();
//            searcher.Dispose();
//            return group;
//        }
//        /// <summary>
//        /// 当前用户是否在用户组内
//        /// </summary>
//        /// <param name="userEntry"></param>
//        /// <param name="groupDistinguishedName"></param>
//        /// <returns></returns>
//        public static bool IsUserInGroup(DirectoryEntry userEntry, string groupDistinguishedName)
//        {
//            PropertyValueCollection pvc = userEntry.Properties["memberOf"];
//            int count = pvc.Count;
//            if (count < 1)
//            {
//                return false;
//            }
//            groupDistinguishedName = groupDistinguishedName.ToLower();
//            for (int i = 0; i < count; i++)
//            {
//                string member = pvc[i].ToString();
//                if (member.ToLower() == groupDistinguishedName)
//                {
//                    return true;
//                }
//            }
//            return false;
//        }
//        /// <summary>
//        /// 添加对象到组
//        /// </summary>
//        /// <param name="groupEntry">组</param>
//        /// <param name="objectEntry">对象</param>
//        public static void AddObjectToGroup(DirectoryEntry groupEntry, DirectoryEntry objectEntry)
//        {
//            try
//            {

//                string groupDistinguishedName = GetProperty(groupEntry, "distinguishedName");
//                string objectDistinguishedName = GetProperty(objectEntry, "distinguishedName");
//                if (string.IsNullOrEmpty(groupDistinguishedName) || string.IsNullOrEmpty(objectDistinguishedName))
//                {
//                    return;
//                }
//                if (IsUserInGroup(objectEntry, groupDistinguishedName))
//                {
//                    return;
//                }
//                SetPropertyGroup(groupEntry, "member", objectDistinguishedName);
//                groupEntry.CommitChanges();
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        /// <summary>
//        /// 将用户添加至用户组
//        /// </summary>
//        /// <param name="OUEntry"></param>
//        /// <param name="userEntry"></param>
//        /// <param name="groupName"></param>
//        public static void AddUserToGroup(DirectoryEntry OUEntry, DirectoryEntry userEntry, string groupName)
//        {
//            if (string.IsNullOrEmpty(groupName))
//            {
//                return;
//            }
//            DirectoryEntry groupEntry = GetGroup(OUEntry, groupName);
//            //if (groupEntry == null)
//            //{
//            //    groupEntry = CreateGroup(OUEntry, groupName);
//            //}
//            string groupDistinguishedName = GetProperty(groupEntry, "distinguishedName");
//            string userDistinguishedName = GetProperty(userEntry, "distinguishedName");
//            if (string.IsNullOrEmpty(groupDistinguishedName) || string.IsNullOrEmpty(userDistinguishedName))
//            {
//                return;
//            }
//            if (IsUserInGroup(userEntry, groupDistinguishedName))
//            {
//                return;
//            }
//            SetPropertyGroup(groupEntry, "member", userDistinguishedName);
//            groupEntry.CommitChanges();
//        }

//        public static void AddUserToGroup(DirectoryEntry entry, string username, string groupName)
//        {
//            DirectorySearcher searcher = new DirectorySearcher(entry);
//            searcher.Filter = "(&(objectClass=group) (cn=" + groupName + "))";
//            SearchResultCollection results = searcher.FindAll();
//            if (results.Count != 0)
//            {
//                DirectorySearcher searcherUser = new DirectorySearcher(entry);
//                searcherUser.Filter = "(&(objectClass=user)(cn=" + username + "))";
//                SearchResultCollection resultsUser = searcherUser.FindAll();
//                if (resultsUser.Count != 0)
//                {
//                    DirectoryEntry group = results[0].GetDirectoryEntry();
//                    group.Invoke("Add", new object[] { resultsUser[0].Path });
//                    group.CommitChanges();
//                    group.Close();
//                }
//            }
//            return;
//        }

//        public static void DeleteUserToGroup(DirectoryEntry entry, string username, string groupName)
//        {
//            DirectorySearcher searcher = new DirectorySearcher(entry);
//            searcher.Filter = "(&(objectClass=group) (cn=" + groupName + "))";
//            SearchResultCollection results = searcher.FindAll();
//            if (results.Count != 0)
//            {
//                DirectorySearcher searcherUser = new DirectorySearcher(entry);
//                searcherUser.Filter = "(&(objectClass=user)(cn=" + username + "))";
//                SearchResultCollection resultsUser = searcherUser.FindAll();
//                if (resultsUser.Count != 0)
//                {
//                    DirectoryEntry group = results[0].GetDirectoryEntry();
//                    try
//                    {
//                        group.Invoke("Remove", new object[] { resultsUser[0].Path });
//                    }
//                    catch
//                    { }
//                    group.CommitChanges();
//                    group.Close();
//                }
//            }
//        }

//        /// <summary>
//        /// 修改组
//        /// </summary>
//        /// <param name="entry"></param>
//        public static void UpdateGroup(DirectoryEntry entry, string desc)
//        {
//            if (!string.IsNullOrEmpty(desc))
//            {
//                entry.Properties["Description"].Value = desc;
//                entry.CommitChanges();
//            }
//            else
//            {
//                entry.Properties["Description"].Value = null;
//                entry.CommitChanges();
//            }
//        }

//        /// <summary>
//        /// 判断当前给定组名是否在所属与给定OUPath对象下
//        ///     例如：给定组名：0011_lyy_23_33
//        ///           给定OUPath：OU=L3DT,DC=addom,DC=xinaogroup,DC=com
//        ///     当前方法判断组名：0011_lyy_23_33 是否所属与L3DT的OU为下
//        /// </summary>
//        /// <param name="ouPath"></param>
//        /// <param name="groupName"></param>
//        /// <returns></returns>
//        public static bool IsGroupInOUPath(string ouPath, string groupName)
//        {
//            using (DirectoryEntry parent = GetDirectoryEntry(ouPath))
//            {
//                using (DirectorySearcher searcher = new DirectorySearcher())
//                {
//                    searcher.SearchRoot = parent;
//                    searcher.CacheResults = false;
//                    searcher.SearchScope = SearchScope.Subtree;
//                    searcher.Filter = "(&(objectClass=group)(name=" + groupName + "))";
//                    SearchResult result = searcher.FindOne();
//                    if (result != null)
//                    {
//                        return true;
//                    }
//                    else
//                        return false;
//                }
//            }
//        }

//        /// <summary>
//        /// 获取组类型的int值
//        /// </summary>
//        /// <returns></returns>
//        public static int GetGroupTypeValue()
//        {
//            int val = (int)(ActiveDs.ADS_GROUP_TYPE_ENUM.ADS_GROUP_TYPE_GLOBAL_GROUP
//            | ActiveDs.ADS_GROUP_TYPE_ENUM.ADS_GROUP_TYPE_SECURITY_ENABLED);
//            return val;

//        }
//        #endregion

//        #region GetUserEntry
//        /// <summary>
//        /// 根据用户登录名查询用户
//        /// </summary>
//        /// <param name="entry"></param>
//        /// <param name="account"></param>
//        /// <returns></returns>
//        public static DirectoryEntry GetUserEntryByAccount(DirectoryEntry entry, string account)
//        {
//            DirectorySearcher searcher = new DirectorySearcher(entry);
//            searcher.Filter = "(&(objectClass=user)(SAMAccountName=" + account + "))";
//            SearchResult result = searcher.FindOne();
//            entry.Close();
//            if (result != null)
//            {
//                return result.GetDirectoryEntry();
//            }
//            return null;
//        }
//        /// <summary>
//        /// 根据用户UniqueID查询用户
//        /// </summary>
//        /// <param name="entry"></param>
//        /// <param name="UniqueID"></param>
//        /// <returns></returns>
//        public static DirectoryEntry GetUserEntryByUniqueID(DirectoryEntry entry, string UniqueID)
//        {
//            DirectorySearcher searcher = new DirectorySearcher(entry);
//            searcher.Filter = "(&(objectClass=user)(title=" + UniqueID + "))";
//            SearchResult result = searcher.FindOne();
//            entry.Close();
//            if (result != null)
//            {
//                return result.GetDirectoryEntry();
//            }
//            return null;
//        }
//        #endregion

//        #region Get/Set Property
//        public static void SetProperty(DirectoryEntry entry, string propertyName, string propertyValue)
//        {
//            if (entry.Properties.Contains(propertyName))
//            {
//                if (string.IsNullOrEmpty(propertyValue))
//                {
//                    //object o = entry.Properties[propertyName].Value;
//                    //entry.Properties[propertyName].Remove(o);
//                    entry.Properties[propertyName][0] = " ";
//                }
//                else
//                {
//                    entry.Properties[propertyName][0] = propertyValue;
//                }
//            }
//            else
//            {
//                if (string.IsNullOrEmpty(propertyValue))
//                {
//                    return;
//                }
//                entry.Properties[propertyName].Add(propertyValue);
//            }
//            //entry.CommitChanges();

//        }
//        public static void UpdateProperty(DirectoryEntry entry, string propertyName, bool propertyValue)
//        {
//            if (entry.Properties.Contains(propertyName))
//            {
//                entry.Properties[propertyName][0] = propertyValue;
//            }
//        }
//        public static void UpdateProperty(DirectoryEntry entry, string propertyName, string propertyValue)
//        {
//            if (entry.Properties.Contains(propertyName))
//            {
//                if (string.IsNullOrEmpty(propertyValue))
//                {
//                    entry.Properties[propertyName][0] = " ";
//                }
//                else
//                {
//                    entry.Properties[propertyName][0] = propertyValue;
//                }
//            }
//        }
//        public static void SetPropertyGroup(DirectoryEntry entry, string propertyName, string propertyValue)
//        {
//            if (entry.Properties.Contains(propertyName))
//            {
//                if (string.IsNullOrEmpty(propertyValue))
//                {
//                    object o = entry.Properties[propertyName].Value;
//                    entry.Properties[propertyName].Remove(o);
//                }
//                else
//                {
//                    entry.Properties[propertyName].Add(propertyValue);
//                }
//            }
//            else
//            {
//                if (string.IsNullOrEmpty(propertyValue))
//                {
//                    return;
//                }
//                entry.Properties[propertyName].Add(propertyValue);
//            }
//        }

//        public static string GetProperty(DirectoryEntry entry, string propertyName)
//        {
//            if (entry.Properties.Contains(propertyName))
//            {
//                return entry.Properties[propertyName].Value.ToString();
//            }
//            else
//            {
//                return string.Empty;
//            }
//        }
//        #endregion

//        #region IsAccountActive
//        public static bool IsAccountActive(DirectoryEntry user)
//        {
//            int nIsAlive = Convert.ToInt32(user.Properties["userAccountControl"].Value);
//            return IsAccountActive(nIsAlive);
//        }

//        public static bool IsAccountActive(int userAccountControl)
//        {
//            int userAccountControl_Disabled = 0X0002;
//            int flagExists = userAccountControl & userAccountControl_Disabled;
//            return flagExists <= 0;
//        }
//        /// <summary>
//        /// 密码加密是否可逆
//        /// </summary>
//        /// <param name="userAccountControl"></param>
//        /// <returns></returns>
//        public static bool IsIsReversible(int userAccountControl)
//        {
//            bool flag = true;
//            int userAccountControl_Disabled = 0X0080;
//            int flagExists = userAccountControl & userAccountControl_Disabled;
//            if (flagExists == 0)
//            {
//                flag = false;
//            }

//            return flag;
//            //return flagExists <= 128;
//        }

//        public static bool IsSafeGroup(int groupType)
//        {
//            bool flag = true;
//            int SECURITY_ENABLED = (int)ADS_GROUP_TYPE_ENUM.ADS_GROUP_TYPE_SECURITY_ENABLED;
//            int flagExists = groupType & SECURITY_ENABLED;
//            if (flagExists == 0)
//            {
//                flag = false;
//            }

//            return flag;
//            //return flagExists <= 128;
//        }

//        #endregion

//        #region EnableAccount
//        public static void EnableUserAccount(DirectoryEntry userEntry)
//        {
//            int exp = (int)userEntry.Properties["userAccountControl"].Value;
//            userEntry.Properties["userAccountControl"].Value = exp | 0x10000;
//            userEntry.CommitChanges();
//            int val = (int)userEntry.Properties["userAccountControl"].Value;
//            userEntry.Properties["userAccountControl"].Value = val & ~0x2;
//            userEntry.CommitChanges();
//        }
//        public static void EnableComputerAccount(DirectoryEntry userEntry)
//        {
//            EnableAccount(userEntry);
//        }


//        public static void EnableAccount(DirectoryEntry userEntry)
//        {
//            int val = (int)userEntry.Properties["userAccountControl"].Value;
//            userEntry.Properties["userAccountControl"].Value = val & ~0x2;
//            userEntry.CommitChanges();
//        }
//        #endregion
//        //密码永不过期
//        public static bool IsExpried(int userAccountControl)
//        {
//            bool flag = true;
//            int userAccountControl_Disabled = 0x10000;
//            int flagExists = userAccountControl & userAccountControl_Disabled;
//            if (flagExists == 0)
//            {
//                flag = false;
//            }

//            return flag;//flagExists <= 65536;
//        }

//        #region DisableAccount
//        public static void DisableAccount(DirectoryEntry rootEntry, string userID)
//        {
//            DirectorySearcher searcher = new DirectorySearcher(rootEntry);
//            searcher.Filter = "(&(objectCategory=Person)(objectClass=user)(userID=" + userID + "))";
//            searcher.SearchScope = SearchScope.Subtree;
//            SearchResult result = searcher.FindOne();
//            if (result != null)
//            {
//                DirectoryEntry entry = result.GetDirectoryEntry();
//                int val = (int)entry.Properties["userAccountControl"].Value;
//                entry.Properties["userAccountControl"].Value = val | 0x2;
//                entry.Properties["msExchHideFromAddressLists"].Value = "TRUE";
//                entry.CommitChanges();
//                entry.Close();
//            }
//            rootEntry.Close();
//        }

//        public static void DisableAccount(DirectoryEntry userEntry)
//        {
//            userEntry.Properties["userAccountControl"][0] = 0X0200 | 0X10000 | 0X0002;
//            userEntry.CommitChanges();
//            userEntry.Close();
//        }
//        public static void DisableComputerAccount(DirectoryEntry entry)
//        {
//            entry.Properties["userAccountControl"][0] = 0x1000 | 0X0002;
//            entry.CommitChanges();
//            entry.Close();
//        }
//        #endregion

//        #region Password
//        public static void SetPassword(DirectoryEntry userEntry, string password)
//        {
//            try
//            {
//                object obj = userEntry.Invoke("SetPassword", new object[] { password });
//            }
//            catch //(Exception ex)
//            {
//                //Common.WriteLog(ex.ToString());
//            }
//        }

//        public static void SetPassword(string path, string password)
//        {
//            DirectoryEntry user = new DirectoryEntry(path);
//            user.Path = path;
//            user.AuthenticationType = AuthenticationTypes.Secure;
//            object[] pw = new object[] { password };
//            user.Invoke("SetPassword", pw);
//            user.CommitChanges();
//            user.Close();
//        }

//        public static void ChangePassword(DirectoryEntry userEntry, string password)
//        {
//            try
//            {
//                object[] pw = new object[] { "", password };
//                userEntry.Invoke("ChangePassword", pw);
//            }
//            catch //(Exception ex)
//            {
//                //Common.WriteLog(ex.ToString());
//            }
//        }
//        #endregion

//        #region User
//        /// <summary>
//        /// 创建用户
//        /// </summary>
//        /// <param name="entry"></param>
//        /// <param name="name"></param>
//        /// <param name="loginName"></param>
//        /// <param name="loginName2000"></param>
//        /// <param name="password"></param>
//        /// <param name="principalDomain">登录域</param>
//        /// <param name="isExpried"></param>
//        /// <returns></returns>
//        public DirectoryEntry CreateNewUser(DirectoryEntry entry, string name, string loginName, string loginName2000, string password, string principalDomain, bool isExpried)
//        {
//            try
//            {
//                DirectoryEntries users = entry.Children;
//                DirectoryEntry userEntry = users.Add("CN=" + ProcLDAPStr(name), "user");
//                SetProperty(userEntry, "SAMAccountName", loginName2000);
//                //userEntry.CommitChanges();
//                string userPrincipalName = loginName + "@" + principalDomain;
//                SetProperty(userEntry, "userPrincipalName", userPrincipalName);
//                userEntry.CommitChanges();
//                SetPassword(userEntry, password);
//                if (!(IsAccountActive(userEntry)))
//                {
//                    EnableUserAccount(userEntry);
//                }

//                if (isExpried == false)
//                {
//                    object ti = userEntry.Properties["userAccountControl"].Value;
//                    userEntry.Properties["userAccountControl"].Value = (int.Parse(ti.ToString()) - 65536);
//                    userEntry.CommitChanges();
//                }
//                //userEntry.CommitChanges();
//                userEntry.Close();
//                entry.Close();
//                return userEntry;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }


//        }
//        /// <summary>
//        /// 创建帐户禁用的用户
//        /// </summary>
//        /// <param name="entry"></param>
//        /// <param name="account"></param>
//        /// <param name="password"></param>
//        /// <param name="principalDomain"></param>
//        /// <returns></returns>
//        public DirectoryEntry CreateDisabledUser(DirectoryEntry entry, string account, string password, string principalDomain)
//        {
//            DirectoryEntries users = entry.Children;
//            DirectoryEntry userEntry = users.Add("CN=" + account, "user");
//            SetProperty(userEntry, "SAMAccountName", account);
//            string userPrincipalName = account + "@" + principalDomain;
//            SetProperty(userEntry, "userPrincipalName", userPrincipalName);
//            SetProperty(userEntry, "userAccountControl", "514");
//            userEntry.CommitChanges();
//            SetPassword(userEntry, password);
//            userEntry.Close();
//            entry.Close();
//            return userEntry;
//        }
//        /// <summary>
//        /// 删除path下的User
//        /// </summary>
//        /// <param name="path"></param>
//        /// <param name="Username">要删除的用户</param>
//        /// <returns></returns>
//        public static bool DeleteUser(DirectoryEntry entry, string Username)
//        {
//            if (!string.IsNullOrEmpty(Username))
//            {
//                DirectoryEntry AD = entry;
//                DirectoryEntry NewUser = AD.Children.Find("CN=" + Username + "", "User");
//                AD.Children.Remove(NewUser);
//                AD.CommitChanges();
//                AD.Close();
//                return true;
//            }
//            return false;
//        }

//        public DirectoryEntry GetUserBySAMAccountName(string sAMAccountName)
//        {
//            DirectorySearcher search = new DirectorySearcher(GetRootEntry());
//            sAMAccountName = ADHelp.ProcSearchFilter(sAMAccountName);
//            search.Filter = ("(&(objectClass=user)(!(objectClass=computer))(sAMAccountname=" + sAMAccountName + "))");
//            SearchResult sr = search.FindOne();
//            return sr.GetDirectoryEntry();
//        }
//        #endregion

//        #region Computer
//        public static DirectoryEntry CreateCompoter(DirectoryEntry entry, string computerName, string sAMAccountName, bool isEnable)
//        {
//            DirectoryEntry computer = entry.Children.Add("CN=" + computerName, "computer");
//            computer.Properties["cn"].Value = computerName;
//            computer.Properties["name"].Value = computerName;
//            if (sAMAccountName.Length > 15)
//            {
//                computer.Properties["sAMAccountName"].Value = sAMAccountName.Substring(0, 14);
//            }
//            else
//            {
//                computer.Properties["sAMAccountName"].Value = sAMAccountName;
//            }
//            computer.CommitChanges();
//            if (isEnable)
//            {
//                if (!(IsAccountActive(computer)))
//                {
//                    EnableAccount(computer);
//                }
//            }
//            return computer;
//        }
//        /// <summary>
//        /// 修改计算机
//        /// </summary>
//        /// <param name="entry"></param>
//        public static void UpdateCompoter(DirectoryEntry entry, bool isEnable)
//        {

//            bool state = (IsAccountActive(entry));
//            if (isEnable != state)
//            {
//                if (!state)
//                {
//                    EnableComputerAccount(entry);
//                }
//                else
//                {
//                    DisableComputerAccount(entry);
//                }
//            }
//            //entry.CommitChanges();
//        }
//        /// <summary>
//        /// 根据计算机名查询计算机
//        /// </summary>
//        /// <param name="entry"></param>
//        /// <param name="account"></param>
//        /// <returns></returns>
//        public static DirectoryEntry GetComputerEntryByAccount(DirectoryEntry entry, string account)
//        {
//            DirectorySearcher searcher = new DirectorySearcher(entry);
//            searcher.Filter = "(&(objectClass=computer)(CN=" + account + "))";
//            searcher.CacheResults = false;
//            searcher.PropertyNamesOnly = true;
//            searcher.PropertiesToLoad.Add("cn");
//            searcher.PropertiesToLoad.Add("distinguishedName");
//            searcher.PropertiesToLoad.Add("Description");
//            searcher.PropertiesToLoad.Add("memberOf");
//            searcher.PropertiesToLoad.Add("userAccountControl");

//            SearchResult result = searcher.FindOne();
//            DirectoryEntry returnEntry = null;
//            entry.Close();
//            if (result != null)
//            {
//                returnEntry = result.GetDirectoryEntry();
//            }
//            entry.Dispose();
//            searcher.Dispose();
//            return returnEntry;

//        }

//        public static void RemoveGroup(DirectoryEntry entry, string OU)
//        {
//            PropertyValueCollection pvc = entry.Properties["member"];
//            if (pvc.IndexOf(OU) >= 0)
//                pvc.Remove(OU);
//            entry.CommitChanges();
//        }

//        public static void AddGroup(DirectoryEntry entry, string OU)
//        {
//            PropertyValueCollection pvc = entry.Properties["member"];
//            if (pvc.IndexOf(OU) >= 0)
//                pvc.Add(OU);
//            entry.CommitChanges();
//        }


//        public void AddComputerToGroup(string group, string computername)
//        {
//            if (!string.IsNullOrEmpty(group) && !string.IsNullOrEmpty(computername))
//            {
//                DirectoryEntry groupEntry = null;
//                DirectoryEntry computerEntry = null;
//                DirectorySearcher seacrch = new DirectorySearcher(GetRootEntry());
//                seacrch.Filter = "(&(objectClass=group)(CN=" + group + "))";
//                seacrch.SearchScope = SearchScope.Subtree;
//                SearchResult result = seacrch.FindOne();
//                if (result != null)
//                {


//                    groupEntry = result.GetDirectoryEntry();
//                    seacrch.Filter = "(&(objectClass=computer)(CN=" + computername + "))";
//                    result = seacrch.FindOne();
//                    if (result != null)
//                    {
//                        computerEntry = result.GetDirectoryEntry();
//                        ADHelp.AddUserToGroup(groupEntry, computerEntry, group);
//                        SetPrimaryGroup(group, computername);
//                        //groupEntry.Dispose();
//                        //computerEntry.Dispose();
//                    }
//                }
//                seacrch.Dispose();
//            }
//        }
//        public void SetPrimaryGroup(string groupname, string computername)
//        {
//            DirectoryEntry groupEntry = null;
//            DirectoryEntry computerEntry = null;
//            DirectorySearcher seacrch = new DirectorySearcher(GetRootEntry());
//            seacrch.Filter = "(&(objectClass=group)(CN=" + groupname + "))";
//            seacrch.SearchScope = SearchScope.Subtree;
//            SearchResult result = seacrch.FindOne();
//            if (result != null)
//            {

//                groupEntry = result.GetDirectoryEntry();
//                seacrch.Filter = "(&(objectClass=computer)(CN=" + computername + "))";
//                result = seacrch.FindOne();
//                if (result != null)
//                {
//                    groupEntry.RefreshCache(new string[] { "primaryGroupToken" });
//                    int groupID = Convert.ToInt32(ADHelp.GetProperty(groupEntry, "primaryGroupToken"));

//                    computerEntry = result.GetDirectoryEntry();
//                    computerEntry.Properties["primaryGroupID"].Value = groupID;
//                    computerEntry.CommitChanges();
//                }
//            }
//        }

//        public void SetPrimaryGroupById(DirectoryEntry groupEntry, DirectoryEntry objectEntry)
//        {
//            //DirectoryEntry groupEntry = ADHelp.GetDirectoryEntry(DOMIN_PATH + "/<GUID=" + groupID + ">");//ADHelp.GetDirectoryEntry(DOMAIN_PATH + "/<GUID=" + groupID + ">"); ;
//            //DirectoryEntry objectEntry = ADHelp.GetDirectoryEntry(DOMAIN_PATH + "/<GUID=" + objcetID + ">");
//            if (groupEntry != null && objectEntry != null)
//            {
//                string Sid = string.Empty;
//                groupEntry.RefreshCache(new string[] { "primaryGroupToken" });

//                string primaryGroupToken = ADHelp.GetProperty(groupEntry, "primaryGroupToken");

//                if (primaryGroupToken != string.Empty)
//                {
//                    int groupId = Convert.ToInt32(primaryGroupToken);
//                    objectEntry.Properties["primaryGroupID"].Value = groupId;
//                    objectEntry.CommitChanges();
//                }
//            }

//        }

//        public void RemoveComputerGroup(string groupname, string computername)
//        {
//            if (!string.IsNullOrEmpty(groupname) && !string.IsNullOrEmpty(computername))
//            {
//                DirectoryEntry groupEntry = null;
//                DirectoryEntry computerEntry = null;
//                DirectorySearcher seacrch = new DirectorySearcher(GetRootEntry());
//                seacrch.Filter = "(&(objectClass=group)(CN=" + groupname + "))";
//                seacrch.SearchScope = SearchScope.Subtree;
//                SearchResult result = seacrch.FindOne();
//                if (result != null)
//                {
//                    groupEntry = result.GetDirectoryEntry();
//                    seacrch.Filter = "(&(objectClass=computer)(CN=" + computername + "))";
//                    result = seacrch.FindOne();
//                    if (result != null)
//                    {
//                        computerEntry = result.GetDirectoryEntry();
//                        ADHelp.RemoveGroup(groupEntry, computerEntry.Path.Replace("LDAP://", ""));
//                        groupEntry.Dispose();
//                        computerEntry.Dispose();
//                    }
//                }
//                seacrch.Dispose();
//            }
//        }
//        #endregion

//        #region functions

//        public string GetDomainName()
//        {
//            return DomainName;// amDAL.Load(100003).MemberValue.ToUpper().Replace("DC=", "").Replace(",", ".");//SUFFIX_PATH.ToUpper().Replace("DC=", "").Replace(",", ".");
//        }
//        public string GetUniqueAccountName(string account)
//        {
//            string preName = account;
//            int namecount = 0;
//            Regex r = new Regex("\\d+$");
//            Match m = r.Match(account);
//            if (m.Success)
//            {
//                preName = r.Split(account)[0];
//                int.TryParse(m.Value, out namecount);
//            }
//            namecount++;
//            account = preName + namecount.ToString();

//            DirectoryEntry deRoot = GetRootEntry();
//            DirectoryEntry userEntry = GetUserEntryByAccount(deRoot, account);
//            if (userEntry != null)
//            {
//                account = GetUniqueAccountName(account);
//            }
//            return account;
//        }

//        public static bool IsAccountIDSame(string userid, DirectoryEntry userEntry)
//        {
//            string desc = GetProperty(userEntry, "title");
//            if (string.IsNullOrEmpty(desc))
//            {
//                return false;
//            }
//            return (userid.ToLower() == desc.ToLower());
//        }

//        public static string ShowSid(string strSid)
//        {
//            //return SidShow.HexStrToDecStr(strSid);


//            int strLen = strSid.Length;
//            if (strLen == 0)
//            {
//                return string.Empty;
//            }
//            int[] arrbySid = new int[strLen / 2];
//            int sidLen = arrbySid.Length;
//            StringBuilder sb = new StringBuilder();

//            for (int j = 0; j < sidLen; j++)
//            {
//                //arrbySid[j] = Convert.ToInt32(strSid.Substring(2 * j + 1, 2),16);
//                arrbySid[j] = Convert.ToInt32(strSid.Substring(2 * j, 2), 16);
//            }

//            sb.AppendFormat("S-{0}-{1}-{2}", arrbySid[0], arrbySid[1], arrbySid[8]);

//            long lngTemp = arrbySid[15];
//            lngTemp = lngTemp * 256 + arrbySid[14];
//            lngTemp = lngTemp * 256 + arrbySid[13];
//            lngTemp = lngTemp * 256 + arrbySid[12];

//            sb.Append("-");
//            sb.Append(lngTemp.ToString());

//            lngTemp = arrbySid[19];
//            lngTemp = lngTemp * 256 + arrbySid[18];
//            lngTemp = lngTemp * 256 + arrbySid[17];
//            lngTemp = lngTemp * 256 + arrbySid[16];

//            sb.Append("-");
//            sb.Append(lngTemp.ToString());

//            lngTemp = arrbySid[23];
//            lngTemp = lngTemp * 256 + arrbySid[22];
//            lngTemp = lngTemp * 256 + arrbySid[21];
//            lngTemp = lngTemp * 256 + arrbySid[20];

//            sb.Append("-");
//            sb.Append(lngTemp.ToString());

//            lngTemp = arrbySid[25];
//            lngTemp = lngTemp * 256 + arrbySid[24];

//            sb.Append("-");
//            sb.Append(lngTemp.ToString());

//            return sb.ToString();

//        }

//        public static IADsLargeInteger GetLargeIntegerFromDateTime(DateTime dateTimeValue)
//        {
//            //
//            // Convert DateTime value to utc file time
//            //
//            Int64 int64Value = 0;
//            try
//            {
//                int64Value = dateTimeValue.ToFileTime();
//            }
//            catch
//            {
//                int64Value = dateTimeValue.ToFileTimeUtc();
//            }

//            //
//            // convert to large integer
//            //
//            IADsLargeInteger largeIntValue = (IADsLargeInteger)new LargeInteger();
//            largeIntValue.HighPart = (int)(int64Value >> 32);
//            largeIntValue.LowPart = (int)(int64Value & 0xFFFFFFFF);

//            return largeIntValue;
//        }
//        public static DateTime GetDateTimeFromLargeInteger(IADsLargeInteger largeIntValue)
//        {
//            //
//            // Convert large integer to int64 value
//            //
//            long int64Value = (long)((uint)largeIntValue.LowPart +
//                     (((long)largeIntValue.HighPart) << 32));

//            //
//            // Return the DateTime in utc
//            //
//            return DateTime.FromFileTime(int64Value);
//        }

//        /// <summary>
//        /// 处理LDAP的SearchFilter
//        /// </summary>
//        /// <param name="srcString">原始字符串</param>
//        /// <returns>处理后的字符串</returns>
//        public static string ProcSearchFilter(string srcString)
//        {
//            srcString = srcString.Replace(@"\", @"\5c");
//            srcString = srcString.Replace(@"*", @"\2a");
//            srcString = srcString.Replace(@"(", @"\28");
//            srcString = srcString.Replace(@")", @"\29");
//            srcString = srcString.Replace(@"/", @"\2f");
//            return srcString;
//            //srcString = srcString.Replace(@"\", @"\5c");
//        }

//        /// <summary>
//        /// 逆处理LDAP的SearchFilter
//        /// </summary>
//        /// <param name="srcString">处理后的字符串</param>
//        /// <returns>逆处理后的字符串</returns>
//        public static string UNProcSearchFilter(string srcString)
//        {
//            srcString = srcString.Replace(@"\5c", @"\");
//            srcString = srcString.Replace(@"\2a", @"*");
//            srcString = srcString.Replace(@"\28", @"(");
//            srcString = srcString.Replace(@"\29", @")");
//            srcString = srcString.Replace(@"\2f", @"/");
//            return srcString;
//            //srcString = srcString.Replace(@"\", @"\5c");
//        }

//        public string GetCanonicalName(string distinguishedName)
//        {
//            string canonicalName = DOMIN_PATH.ToLower() + "/";
//            string[] strDNArr;
//            if (distinguishedName.IndexOf("ldap://").Equals(0))
//            {
//                string _Domain_Path = DOMAIN_PATH.ToLower();
//                strDNArr = SUFFIX_PATH.ToLower().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
//                string temp = "";
//                for (int j = strDNArr.Length - 1; j >= 0; j--)
//                {
//                    temp += "/" + strDNArr[j].Replace("dc=", "");
//                }
//                canonicalName += distinguishedName.Replace(DOMAIN_PATH.ToLower() + temp + "/", "");
//            }
//            else
//            {
//                distinguishedName = distinguishedName.Replace("DC=", "").Replace("OU=", "").Replace("CN=", "").Replace("\\#", "#").Replace(DOMIN_PATH.ToLower().Replace(".", ","), "");
//                strDNArr = distinguishedName.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
//                for (int j = strDNArr.Length - 1; j >= 0; j--)
//                {
//                    canonicalName += strDNArr[j] + "/";
//                }
//                canonicalName = canonicalName.Substring(0, canonicalName.Length - 1);
//            }

//            return canonicalName;
//        }
//        #endregion

//        #region Convert
//        /// <summary>
//        /// 汉字转换成全拼的拼音
//        /// </summary>
//        /// <param name="Chstr">汉字字符串</param>
//        /// <returns>转换后的拼音字符串</returns>
//        public static string ChineseCharacter2QuanPinYin(string Chstr)
//        {
//            //定义拼音区编码数组
//            int[] getValue = new int[]
//      {
//          -20319,-20317,-20304,-20295,-20292,-20283,-20265,-20257,-20242,-20230,-20051,-20036,
//          -20032,-20026,-20002,-19990,-19986,-19982,-19976,-19805,-19784,-19775,-19774,-19763,
//          -19756,-19751,-19746,-19741,-19739,-19728,-19725,-19715,-19540,-19531,-19525,-19515,
//          -19500,-19484,-19479,-19467,-19289,-19288,-19281,-19275,-19270,-19263,-19261,-19249,
//          -19243,-19242,-19238,-19235,-19227,-19224,-19218,-19212,-19038,-19023,-19018,-19006,
//          -19003,-18996,-18977,-18961,-18952,-18783,-18774,-18773,-18763,-18756,-18741,-18735,
//          -18731,-18722,-18710,-18697,-18696,-18526,-18518,-18501,-18490,-18478,-18463,-18448,
//          -18447,-18446,-18239,-18237,-18231,-18220,-18211,-18201,-18184,-18183, -18181,-18012,
//          -17997,-17988,-17970,-17964,-17961,-17950,-17947,-17931,-17928,-17922,-17759,-17752,
//          -17733,-17730,-17721,-17703,-17701,-17697,-17692,-17683,-17676,-17496,-17487,-17482,
//          -17468,-17454,-17433,-17427,-17417,-17202,-17185,-16983,-16970,-16942,-16915,-16733,
//          -16708,-16706,-16689,-16664,-16657,-16647,-16474,-16470,-16465,-16459,-16452,-16448,
//          -16433,-16429,-16427,-16423,-16419,-16412,-16407,-16403,-16401,-16393,-16220,-16216,
//          -16212,-16205,-16202,-16187,-16180,-16171,-16169,-16158,-16155,-15959,-15958,-15944,
//          -15933,-15920,-15915,-15903,-15889,-15878,-15707,-15701,-15681,-15667,-15661,-15659,
//          -15652,-15640,-15631,-15625,-15454,-15448,-15436,-15435,-15419,-15416,-15408,-15394,
//          -15385,-15377,-15375,-15369,-15363,-15362,-15183,-15180,-15165,-15158,-15153,-15150,
//          -15149,-15144,-15143,-15141,-15140,-15139,-15128,-15121,-15119,-15117,-15110,-15109,
//          -14941,-14937,-14933,-14930,-14929,-14928,-14926,-14922,-14921,-14914,-14908,-14902,
//          -14894,-14889,-14882,-14873,-14871,-14857,-14678,-14674,-14670,-14668,-14663,-14654,
//          -14645,-14630,-14594,-14429,-14407,-14399,-14384,-14379,-14368,-14355,-14353,-14345,
//          -14170,-14159,-14151,-14149,-14145,-14140,-14137,-14135,-14125,-14123,-14122,-14112,
//          -14109,-14099,-14097,-14094,-14092,-14090,-14087,-14083,-13917,-13914,-13910,-13907,
//          -13906,-13905,-13896,-13894,-13878,-13870,-13859,-13847,-13831,-13658,-13611,-13601,
//          -13406,-13404,-13400,-13398,-13395,-13391,-13387,-13383,-13367,-13359,-13356,-13343,
//          -13340,-13329,-13326,-13318,-13147,-13138,-13120,-13107,-13096,-13095,-13091,-13076,
//          -13068,-13063,-13060,-12888,-12875,-12871,-12860,-12858,-12852,-12849,-12838,-12831,
//          -12829,-12812,-12802,-12607,-12597,-12594,-12585,-12556,-12359,-12346,-12320,-12300,
//          -12120,-12099,-12089,-12074,-12067,-12058,-12039,-11867,-11861,-11847,-11831,-11798,
//          -11781,-11604,-11589,-11536,-11358,-11340,-11339,-11324,-11303,-11097,-11077,-11067,
//          -11055,-11052,-11045,-11041,-11038,-11024,-11020,-11019,-11018,-11014,-10838,-10832,
//          -10815,-10800,-10790,-10780,-10764,-10587,-10544,-10533,-10519,-10331,-10329,-10328,
//          -10322,-10315,-10309,-10307,-10296,-10281,-10274,-10270,-10262,-10260,-10256,-10254
//      };
//            //定义拼音数组
//            string[] getName = new string[]
//      {
//          "A","Ai","An","Ang","Ao","Ba","Bai","Ban","Bang","Bao","Bei","Ben",
//          "Beng","Bi","Bian","Biao","Bie","Bin","Bing","Bo","Bu","Ba","Cai","Can",
//          "Cang","Cao","Ce","Ceng","Cha","Chai","Chan","Chang","Chao","Che","Chen","Cheng",
//          "Chi","Chong","Chou","Chu","Chuai","Chuan","Chuang","Chui","Chun","Chuo","Ci","Cong",
//          "Cou","Cu","Cuan","Cui","Cun","Cuo","Da","Dai","Dan","Dang","Dao","De",
//          "Deng","Di","Dian","Diao","Die","Ding","Diu","Dong","Dou","Du","Duan","Dui",
//          "Dun","Duo","E","En","Er","Fa","Fan","Fang","Fei","Fen","Feng","Fo",
//          "Fou","Fu","Ga","Gai","Gan","Gang","Gao","Ge","Gei","Gen","Geng","Gong",
//          "Gou","Gu","Gua","Guai","Guan","Guang","Gui","Gun","Guo","Ha","Hai","Han",
//          "Hang","Hao","He","Hei","Hen","Heng","Hong","Hou","Hu","Hua","Huai","Huan",
//          "Huang","Hui","Hun","Huo","Ji","Jia","Jian","Jiang","Jiao","Jie","Jin","Jing",
//          "Jiong","Jiu","Ju","Juan","Jue","Jun","Ka","Kai","Kan","Kang","Kao","Ke",
//          "Ken","Keng","Kong","Kou","Ku","Kua","Kuai","Kuan","Kuang","Kui","Kun","Kuo",
//          "La","Lai","Lan","Lang","Lao","Le","Lei","Leng","Li","Lia","Lian","Liang",
//          "Liao","Lie","Lin","Ling","Liu","Long","Lou","Lu","Lv","Luan","Lue","Lun",
//          "Luo","Ma","Mai","Man","Mang","Mao","Me","Mei","Men","Meng","Mi","Mian",
//          "Miao","Mie","Min","Ming","Miu","Mo","Mou","Mu","Na","Nai","Nan","Nang",
//          "Nao","Ne","Nei","Nen","Neng","Ni","Nian","Niang","Niao","Nie","Nin","Ning",
//          "Niu","Nong","Nu","Nv","Nuan","Nue","Nuo","O","Ou","Pa","Pai","Pan",
//          "Pang","Pao","Pei","Pen","Peng","Pi","Pian","Piao","Pie","Pin","Ping","Po",
//          "Pu","Qi","Qia","Qian","Qiang","Qiao","Qie","Qin","Qing","Qiong","Qiu","Qu",
//          "Quan","Que","Qun","Ran","Rang","Rao","Re","Ren","Reng","Ri","Rong","Rou",
//          "Ru","Ruan","Rui","Run","Ruo","Sa","Sai","San","Sang","Sao","Se","Sen",
//          "Seng","Sha","Shai","Shan","Shang","Shao","She","Shen","Sheng","Shi","Shou","Shu",
//          "Shua","Shuai","Shuan","Shuang","Shui","Shun","Shuo","Si","Song","Sou","Su","Suan",
//          "Sui","Sun","Suo","Ta","Tai","Tan","Tang","Tao","Te","Teng","Ti","Tian",
//          "Tiao","Tie","Ting","Tong","Tou","Tu","Tuan","Tui","Tun","Tuo","Wa","Wai",
//          "Wan","Wang","Wei","Wen","Weng","Wo","Wu","Xi","Xia","Xian","Xiang","Xiao",
//          "Xie","Xin","Xing","Xiong","Xiu","Xu","Xuan","Xue","Xun","Ya","Yan","Yang",
//          "Yao","Ye","Yi","Yin","Ying","Yo","Yong","You","Yu","Yuan","Yue","Yun",
//          "Za", "Zai","Zan","Zang","Zao","Ze","Zei","Zen","Zeng","Zha","Zhai","Zhan",
//          "Zhang","Zhao","Zhe","Zhen","Zheng","Zhi","Zhong","Zhou","Zhu","Zhua","Zhuai","Zhuan",
//          "Zhuang","Zhui","Zhun","Zhuo","Zi","Zong","Zou","Zu","Zuan","Zui","Zun","Zuo"
//     };

//            Regex reg = new Regex("^[\u4e00-\u9fa5]$");//验证是否输入汉字
//            byte[] arr = new byte[2];
//            string pystr = "";
//            int asc = 0, M1 = 0, M2 = 0;
//            char[] mChar = Chstr.ToCharArray();//获取汉字对应的字符数组
//            for (int j = 0; j < mChar.Length; j++)
//            {
//                //如果输入的是汉字
//                if (reg.IsMatch(mChar[j].ToString()))
//                {
//                    arr = System.Text.Encoding.Default.GetBytes(mChar[j].ToString());
//                    M1 = (short)(arr[0]);
//                    M2 = (short)(arr[1]);
//                    asc = M1 * 256 + M2 - 65536;
//                    if (asc > 0 && asc < 160)
//                    {
//                        pystr += mChar[j];
//                    }
//                    else
//                    {
//                        switch (asc)
//                        {
//                            case -9254:
//                                pystr += "Zhen"; break;
//                            case -8985:
//                                pystr += "Qian"; break;
//                            case -5463:
//                                pystr += "Jia"; break;
//                            case -8274:
//                                pystr += "Ge"; break;
//                            case -5448:
//                                pystr += "Ga"; break;
//                            case -5447:
//                                pystr += "La"; break;
//                            case -4649:
//                                pystr += "Chen"; break;
//                            case -5436:
//                                pystr += "Mao"; break;
//                            case -5213:
//                                pystr += "Mao"; break;
//                            case -3597:
//                                pystr += "Die"; break;
//                            case -5659:
//                                pystr += "Tian"; break;
//                            default:
//                                for (int i = (getValue.Length - 1); i >= 0; i--)
//                                {
//                                    if (getValue[i] <= asc)//判断汉字的拼音区编码是否在指定范围内
//                                    {
//                                        pystr += getName[i];//如果不超出范围则获取对应的拼音
//                                        break;
//                                    }
//                                }
//                                break;
//                        }
//                    }
//                }
//                else//如果不是汉字
//                {
//                    pystr += mChar[j].ToString();//如果不是汉字则返回
//                }
//            }
//            return pystr;//返回获取到的汉字拼音
//        }
//        /// <summary>
//        /// 汉字转拼音缩写
//        /// 字母和符号不转换
//        /// </summary>
//        /// <param name="str">要转换的汉字字符串</param>
//        /// <returns>拼音缩写</returns>
//        public static string ChineseCharacter2PinYinAbbreviation(string str)
//        {
//            string tempStr = string.Empty;
//            foreach (char c in str)
//            {
//                if ((int)c >= 33 && (int)c <= 126)
//                {
//                    tempStr += c.ToString();
//                }
//                else
//                {
//                    tempStr += GetPYChar(c.ToString());
//                }
//            }
//            return tempStr;
//        }
//        /// <summary>
//        /// 取单个字符的拼音声母
//        /// </summary>
//        /// <param name="c">要转换的单个汉字</param>
//        /// <returns>拼音声母</returns>
//        public static string GetPYChar(string c)
//        {
//            byte[] array = new byte[2];
//            array = System.Text.Encoding.Default.GetBytes(c);
//            int i = (short)(array[0] - '\0') * 256 + ((short)(array[1] - '\0'));
//            if (i < 0xB0A1) return "*";
//            if (i < 0xB0C5) return "a";
//            if (i < 0xB2C1) return "b";
//            if (i < 0xB4EE) return "c";
//            if (i < 0xB6EA) return "d";
//            if (i < 0xB7A2) return "e";
//            if (i < 0xB8C1) return "f";
//            if (i < 0xB9FE) return "g";
//            if (i < 0xBBF7) return "h";
//            if (i < 0xBFA6) return "g";
//            if (i < 0xC0AC) return "k";
//            if (i < 0xC2E8) return "l";
//            if (i < 0xC4C3) return "m";
//            if (i < 0xC5B6) return "n";
//            if (i < 0xC5BE) return "o";
//            if (i < 0xC6DA) return "p";
//            if (i < 0xC8BB) return "q";
//            if (i < 0xC8F6) return "r";
//            if (i < 0xCBFA) return "s";
//            if (i < 0xCDDA) return "t";
//            if (i < 0xCEF4) return "w";
//            if (i < 0xD1B9) return "x";
//            if (i < 0xD4D1) return "y";
//            if (i < 0xD7FA) return "z";
//            return "*";
//        }
//        #endregion Convert
//    }

//    /// <summary>
//    /// 操作AD
//    /// </summary>
//    class AD
//    {
//        private string _domainADsPath;
//        private string _username;
//        private string _password;

//        public static string TYPE_ORGANIZATIONALUNIT = "organizationalUnit";
//        public static string TYPE_GROUP = "group";
//        public static string TYPE_USER = "user";

//        /// <summary>
//        /// 构造
//        /// </summary>
//        /// <param name="server"></param>
//        /// <param name="path"></param>
//        public AD(string domainADsPath, string username, string password)
//        {
//            this._domainADsPath = domainADsPath;
//            this._username = username;
//            this._password = password;
//        }

//        /// <summary>
//        /// 读取用户
//        /// </summary>
//        /// <param name="domainADsPath"></param>
//        /// <param name="username"></param>
//        /// <param name="password"></param>
//        /// <param name="schemaClassNameToSearch"></param>
//        /// <returns></returns>
//        public DataTable GetUserList(string schemaClassNameToSearch)
//        {
//            SearchResultCollection results = ExecuteAD(schemaClassNameToSearch);
//            DataTable dt = GetUserList(results);
//            results.Dispose();
//            return dt;
//        }

//        /// <summary>
//        /// 给用户绑定RFID
//        /// </summary>
//        /// <param name="username"></param>
//        /// <param name="rfid"></param>
//        public void BindRfIdToADUser(string username, string rfid)
//        {
//            DirectoryEntry entry = ExecuteAD(TYPE_USER, username);
//            if (entry != null)
//            {
//                //需要判断卡号是否存在
//                SearchResultCollection results = ExecuteAD(TYPE_USER);
//                foreach (SearchResult result in results)
//                {
//                    string adPath = result.Path;
//                    if (adPath.IndexOf("/") < 0)
//                        continue;
//                    DirectoryEntry tmpEntry = result.GetDirectoryEntry();
//                    if (tmpEntry.Properties["Comment"].Count > 0 && tmpEntry.Properties["Comment"][0].ToString() == rfid)
//                    {
//                        //卡号已经存在
//                        throw new Exception("此卡号已经绑定到员工[" + tmpEntry.Properties["name"][0].ToString() + "]");
//                    }
//                }

//                //更新
//                SetProperty(entry, "Comment", rfid); //Comment 值作为RFID卡号
//                entry.CommitChanges();

//            }
//        }

//        /// <summary>
//        /// 通过rfid读取AD用户信息
//        /// </summary>
//        /// <param name="rfid"></param>
//        /// <returns></returns>
//        public DirectoryEntry GetDirectoryEntryByRFID(string rfid)
//        {
//            SearchResultCollection results = ExecuteAD(TYPE_USER);
//            foreach (SearchResult result in results)
//            {
//                string adPath = result.Path;
//                if (adPath.IndexOf("/") < 0)
//                    continue;
//                DirectoryEntry tmpEntry = result.GetDirectoryEntry();
//                if (tmpEntry.Properties["Comment"].Count > 0 && tmpEntry.Properties["Comment"][0].ToString() == rfid)
//                {
//                    return result.GetDirectoryEntry();
//                }
//            }
//            return null;
//        }

//        /// <summary>
//        /// 读取用户
//        /// </summary>
//        /// <param name="results"></param>
//        /// <returns></returns>
//        public DataTable GetUserList(SearchResultCollection results)
//        {
//            DataTable dt = new DataTable();
//            dt.Columns.Add("rfid", typeof(string));
//            dt.Columns.Add("username", typeof(string));
//            dt.Columns.Add("password", typeof(string));
//            dt.Columns.Add("path", typeof(string));
//            dt.Columns.Add("displayname", typeof(string));
//            dt.Columns.Add("samaccountname", typeof(string));
//            dt.Columns.Add("mail", typeof(string));


//            if (results.Count == 0)
//                throw new Exception("域中没有任何用户");
//            else
//            {
//                foreach (SearchResult result in results)
//                {
//                    string adPath = result.Path;
//                    if (adPath.IndexOf("/") < 0)
//                        continue;
//                    DirectoryEntry entry = result.GetDirectoryEntry();
//                    if (entry != null)
//                    {
//                        DataRow dr = dt.NewRow();
//                        if (entry.Properties["name"].Count > 0)
//                            dr["username"] = entry.Properties["name"][0].ToString();
//                        if (entry.Properties["samaccountname"].Count > 0)
//                            dr["samaccountname"] = entry.Properties["sAMAccountName"][0].ToString();
//                        if (entry.Properties["Comment"].Count > 0)
//                            dr["rfid"] = entry.Properties["Comment"][0].ToString();//暂时用这个来作为RFID
//                        if (entry.Properties["displayname"].Count > 0)
//                            dr["displayname"] = entry.Properties["displayname"][0].ToString();
//                        if (entry.Properties["mail"].Count > 0)
//                            dr["mail"] = entry.Properties["mail"][0].ToString();
//                        dt.Rows.Add(dr);
//                    }
//                }
//            }
//            return dt;
//        }

//        /// <summary>
//        /// 读取组
//        /// </summary>
//        /// <param name="domainADsPath"></param>
//        /// <param name="username"></param>
//        /// <param name="password"></param>
//        /// <param name="schemaClassNameToSearch"></param>
//        /// <returns></returns>
//        public DataTable GetGroupList(string schemaClassNameToSearch)
//        {
//            SearchResultCollection results = ExecuteAD(schemaClassNameToSearch);
//            DataTable dt = GetGroupList(results);
//            results.Dispose();
//            return dt;
//        }



//        /// <summary>
//        /// 读取组
//        /// </summary>
//        /// <param name="results"></param>
//        /// <returns></returns>
//        public DataTable GetGroupList(SearchResultCollection results)
//        {
//            DataTable dt = new DataTable();
//            dt.Columns.Add("rfid", typeof(string));
//            dt.Columns.Add("username", typeof(string));
//            dt.Columns.Add("password", typeof(string));
//            dt.Columns.Add("path", typeof(string));
//            dt.Columns.Add("displayname", typeof(string));
//            dt.Columns.Add("samaccountname", typeof(string));
//            dt.Columns.Add("mail", typeof(string));
//            if (results.Count == 0)
//                throw new Exception("域中没有任何组织结构");
//            else
//            {
//                foreach (SearchResult result in results)
//                {
//                    if (result.Path.IndexOf("OU=用户") < 0)
//                        continue;
//                    ResultPropertyCollection propColl = result.Properties;
//                    DataRow dr = dt.NewRow();
//                    dr["name"] = propColl["name"][0].ToString();
//                    //TODO
//                }
//            }
//            return dt;
//        }

//        /// <summary>
//        /// 从AD中读取数据
//        /// </summary>
//        /// <param name="schemaClassNameToSearch"></param>
//        /// <returns></returns>
//        public SearchResultCollection ExecuteAD(string schemaClassNameToSearch)
//        {
//            DirectorySearcher searcher = new DirectorySearcher();
//            searcher.SearchRoot = new DirectoryEntry(_domainADsPath, _username, _password);
//            searcher.Filter = "(objectClass=" + schemaClassNameToSearch + ")";
//            searcher.SearchScope = SearchScope.Subtree;
//            searcher.Sort = new SortOption("name", SortDirection.Ascending);
//            searcher.PageSize = 512;

//            //指对范围内的属性进行加载，以提高效率
//            searcher.PropertiesToLoad.AddRange(new string[] { "name", "Path", "displayname", "samaccountname", "mail", "Comment" });
//            SearchResultCollection results = searcher.FindAll();
//            return results;
//        }


//        /// <summary>
//        /// 从AD中读取数据
//        /// </summary>
//        /// <returns></returns>
//        public DirectoryEntry ExecuteAD(string schemaClassNameToSearch, string cn)
//        {
//            DirectorySearcher searcher = new DirectorySearcher();
//            searcher.SearchRoot = new DirectoryEntry(_domainADsPath, _username, _password, AuthenticationTypes.Delegation);
//            searcher.Filter = "(&(objectClass=" + schemaClassNameToSearch + ")(cn=" + cn + "))";
//            searcher.SearchScope = SearchScope.Subtree;
//            searcher.Sort = new SortOption("name", SortDirection.Ascending);
//            searcher.PageSize = 512;

//            //指对范围内的属性进行加载，以提高效率
//            searcher.PropertiesToLoad.AddRange(new string[] { "name", "Path", "displayname", "samaccountname", "mail", "Comment" });

//            SearchResult result = searcher.FindOne();
//            DirectoryEntry entry = result.GetDirectoryEntry();
//            return entry;
//        }

//        /// <summary>
//        /// 设置属性，如果不存在此属性，可以创建
//        /// </summary>
//        /// <param name="entry"></param>
//        /// <param name="propertyName"></param>
//        /// <param name="propertyValue"></param>
//        public static void SetProperty(DirectoryEntry entry, string propertyName, string propertyValue)
//        {
//            if (!string.IsNullOrEmpty(propertyValue))
//            {
//                if (entry.Properties.Contains(propertyName))
//                {
//                    entry.Properties[propertyName][0] = propertyValue;
//                }
//                else
//                {
//                    entry.Properties[propertyName].Add(propertyValue);
//                }
//            }
//        }

//        /// <summary>
//        /// 读取属性
//        /// </summary>
//        /// <param name="entry"></param>
//        /// <param name="propertyName"></param>
//        /// <returns></returns>
//        public static string GetProperty(DirectoryEntry entry, string propertyName)
//        {
//            if (entry.Properties.Contains(propertyName))
//                return entry.Properties[propertyName][0].ToString();
//            else
//                return String.Empty;
//        }
//    }
//}