using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.DirectoryServices;
using System.Data;
using Dot.Utility.Log;


namespace Dot.Utility.ActiveDirectory
{
    public class ADHelper
    {
        public string LDAPPath = "LDAP://";
        public string Domain;
        public string DomainUser;
        public string DomainPass;
        public DirectoryEntry RootEntry
        {
            get;
            set;
        }

        public const string Type_OrganizationalUnit = "organizationalUnit";
        public const string Type_Group = "group";
        public const string Type_User = "user";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        public ADHelper(string domain)
        {
            Domain = domain;
            if (RootEntry == null)
            {
                RootEntry = new DirectoryEntry(LDAPPath + Domain);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="admin"></param>
        /// <param name="password"></param>
        public ADHelper(string domain, string admin, string password)
        {
            Domain = domain;
            DomainUser = admin;
            DomainPass = password;
            if (RootEntry == null)
            {
                RootEntry = new DirectoryEntry(LDAPPath + Domain, DomainUser, DomainPass);
            }
        }
        /// <summary>
        /// 验证AD用户是否登陆成功
        /// </summary>
        /// <param name="domain">域名称</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>返回登陆状态</returns>
        public bool TryAuthenticate(string Account, string Password)
        {
            bool isLogin = false;
            //try
            //{
            DirectoryEntry entry = new DirectoryEntry(LDAPPath + Domain, Account, Password);
            entry.RefreshCache();
            isLogin = true;
            //}
            //catch
            //{
            //    isLogin = false;
            //}
            return isLogin;
        }

        /// <summary>
        /// 获取AD根
        /// </summary>
        /// <param name="domain">域名称</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>返回登陆状态</returns>
        public DirectoryEntry GetRootEntry()
        {
            return GetRootEntry(Domain, DomainUser, DomainPass);
        }
        /// <summary>
        /// 获取AD根
        /// </summary>
        /// <param name="domain">域名称</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>返回登陆状态</returns>
        public DirectoryEntry GetRootEntry(string Domain, string Account, string Password)
        {
            if (RootEntry == null)
            {
                RootEntry = new DirectoryEntry(LDAPPath + Domain, Account, Password);
            }
            return RootEntry;
        }

        /// <summary>
        /// 返回DirectoryEntry
        /// </summary>
        /// <param name="path">域地址</param>
        /// <returns></returns>
        public static DirectoryEntry GetDirectoryEntry(string path)
        {
            DirectoryEntry de = new DirectoryEntry();
            de.Path = "LDAP://" + path;
            return de;
        }

        #region 操作OU

        /// <summary>
        /// 获取根OU
        /// </summary>
        /// <param name="orgName">OU名称</param>
        /// <returns></returns>
        public DirectoryEntry GetRootOU(string nameOrPath)
        {
            //try
            //{
            if (nameOrPath.IndexOf(",DC=", StringComparison.OrdinalIgnoreCase) > -1)
                return GetOUByPath(nameOrPath);
            else
                return GetOU(RootEntry, nameOrPath);
            //}
            //catch (Exception ex)
            //{
            //    LogFactory.ExceptionLog.Error(ex);
            //    return null;
            //}
        }

        /// <summary>
        /// 获取或创建机构根节点
        /// </summary>
        /// <param name="org"></param>
        /// <param name="codeProperty">编号保存到那个属性</param>
        /// <returns></returns>
        public DirectoryEntry GetOrCreateRootOU(OU org, string codeProperty)
        {
            DirectoryEntry root = GetRootEntry(Domain, DomainUser, DomainPass);

            DirectoryEntry rootOU = GetOU(root, org.Name);
            if (null != rootOU)
            {
                return rootOU;
            }
            else
            {
                rootOU = ADDOU(root, org.Name, codeProperty, org.ImportId);

                return rootOU;
            }

        }


        /// <summary>
        /// 通过path获取机构
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public DirectoryEntry GetOUByPath(string path)
        {
            //try
            //{
            DirectorySearcher search = new DirectorySearcher(RootEntry);
            search.Filter = "(&(objectClass=" + Type_OrganizationalUnit + ")(distinguishedName=" + path + "))";// "(SAMAccountName=qiu.fangbing)";
            //search.SearchScope = scope;
            SearchResult result = search.FindOne();
            if (null == result)
                return null;
            DirectoryEntry org = result.GetDirectoryEntry();
            return org;
            //}
            //catch (Exception ex)
            //{
            //    LogFactory.ExceptionLog.Error(ex);
            //    return null;
            //}
        }


        /// <summary>
        /// 通过编号获取机构
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="orgCodeColumnName">对应属性</param>
        /// <param name="orgCode"></param>
        /// <returns></returns>
        public static DirectoryEntry GetOUByOrgCode(DirectoryEntry parent, string orgCodeColumnName, string orgCode, SearchScope scope = SearchScope.OneLevel)
        {
            //try
            //{
            DirectorySearcher search = new DirectorySearcher(parent);
            search.Filter = "(&(objectClass=" + Type_OrganizationalUnit + ")(" + orgCodeColumnName + "=" + orgCode + "))";// "(SAMAccountName=qiu.fangbing)";
            search.SearchScope = scope;
            SearchResult result = search.FindOne();
            if (null == result)
                return null;
            DirectoryEntry org = result.GetDirectoryEntry();
            return org;
            //}
            //catch (Exception ex)
            //{
            //    LogFactory.ExceptionLog.Error(ex);
            //    return null;
            //}
        }


        /// <summary>
        /// 通过名称获取机构
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="orgName"></param>
        /// <returns></returns>
        public static DirectoryEntry GetOU(DirectoryEntry parent, string orgName)
        {
            DirectorySearcher search = new DirectorySearcher(parent);
            search.Filter = "(&(objectClass=" + Type_OrganizationalUnit + ")(name=" + orgName + "))";// "(SAMAccountName=qiu.fangbing)";
            //search.SearchScope = scope;
            SearchResult result = search.FindOne();
            if (null == result)
                return null;
            DirectoryEntry org = result.GetDirectoryEntry();
            return org;

            //try
            //{
            //DirectoryEntry ouEntry = parent.Children.Find("OU=" + orgName);
            //return ouEntry;
            //}
            //catch (Exception ex)
            //{
            //    LogFactory.ExceptionLog.Error(ex);
            //    return null;
            //}
        }

        /// <summary>
        /// 获取所有子OU
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static SearchResultCollection GetAllSubOUs(DirectoryEntry parent)
        {
            return GetAllSubDirectoryEntrys(parent, Type_OrganizationalUnit);
        }

        /// <summary>
        /// 获取所有子OU
        /// </summary>
        /// <param name="orgName"></param>
        /// <returns></returns>
        public SearchResultCollection GetAllSubOUs(string orgName)
        {
            return GetAllSubDirectoryEntrys(orgName, Type_OrganizationalUnit);
        }

        /// <summary>
        /// 获取某个时间段内的OU
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static SearchResultCollection GetSubOUs(DirectoryEntry parent, DateTime startTime, DateTime? endTime = null, SearchScope scope = SearchScope.Subtree, string datetimePropterty = "whenChanged")
        {
            return GetSubDirectoryEntrys(parent, Type_OrganizationalUnit, startTime, endTime, scope, datetimePropterty);
        }

        /// <summary>
        /// 获得OU的Path
        /// </summary>
        /// <param name="organizeUnit">OU名</param>
        /// <returns></returns>
        public string GetOrganizeNamePath(string organizeUnit)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(LDAPPath);
            sb.Append(Domain);
            sb.Append("/");
            return sb.Append(SplitOrganizeNameToDN(organizeUnit)).ToString();
        }

        static string SplitOrganizeNameToDN(string organizeUnit)
        {
            return "OU=" + organizeUnit;
        }

        /// <summary>
        /// 修改OU
        /// </summary>
        /// <param name="ouEntry"></param>
        /// <param name="newOUName"></param>
        /// <returns></returns>
        public static bool ChangeOU(DirectoryEntry ouEntry, string newOUName)
        {
            //try
            //{
            ouEntry.Rename("OU=" + newOUName);

            ouEntry.CommitChanges();
            return true;
            //}
            //catch (Exception ex)
            //{
            //    LogFactory.ExceptionLog.Error(ex);
            //    return false;
            //}
        }

        /// <summary>
        /// 删除OU
        /// </summary>
        /// <param name="ouEntry"></param>
        /// <param name="newOUName"></param>
        /// <returns></returns>
        public static bool DeleteOU(DirectoryEntry ouEntry)
        {
            //try
            //{

            //DirectoryEntry OUParent = ouEntry.Parent;
            ouEntry.DeleteTree();
            //OUParent.Children.Remove(ouEntry);
            ouEntry.CommitChanges();
            //OUParent.CommitChanges();
            return true;
            //}
            //catch (Exception ex)
            //{
            //    LogFactory.ExceptionLog.Error(ex);
            //    return false;
            //}
        }

        /// <summary>
        /// 添加OU
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="ouName">OU名称</param>
        /// <param name="ouPropertyName">OU属性名</param>
        /// <param name="ouCode">OU编码</param>
        /// <returns></returns>
        public static DirectoryEntry ADDOU(DirectoryEntry parent, string ouName, string ouPropertyName, string ouCode)
        {
            DirectoryEntry newou = parent.Children.Add("OU=" + ouName, "organizationalUnit");
            ADHelper.SetProperty(newou, ouPropertyName, ouCode);
            newou.CommitChanges();
            return newou;
        }



        #endregion

        #region 操作group

        /// <summary>
        /// 获取所有组
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static SearchResultCollection GetAllSubGroups(DirectoryEntry parent)
        {
            return GetAllSubDirectoryEntrys(parent, Type_Group);             
        }

        /// <summary>
        /// 获取AD组
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="organizeUnit"></param>
        /// <returns></returns>
        public DirectoryEntry GetADGroupInOU(string groupName, string organizeUnit)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                DirectoryEntry de = new DirectoryEntry(GetOrganizeNamePath(organizeUnit), DomainUser, DomainPass, AuthenticationTypes.Secure);
                DirectorySearcher deSearch = new DirectorySearcher(de);
                deSearch.Filter = "(&(objectClass=group)(cn=" + groupName.Replace("\\", "") + "))";
                deSearch.SearchScope = SearchScope.Subtree;
                try
                {
                    SearchResult result = deSearch.FindOne();
                    if (result != null)
                    {
                        de = new DirectoryEntry(result.Path, DomainUser, DomainPass);
                    }
                    else
                    {
                        return null;
                    }
                    return de;
                }
                catch (Exception ex)
                {
                    LogFactory.ExceptionLog.Error(ex);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }



        ///// <summary>
        ///// 创建用户组
        ///// </summary>
        ///// <param name="entry"></param>
        ///// <param name="groupname"></param>
        ///// <param name="name2000"></param>
        ///// <returns></returns>
        //public static DirectoryEntry CreateGroup(DirectoryEntry entry, string groupname, string name2000)
        //{
        //    DirectoryEntry group = entry.Children.Add("CN=" + ProcLDAPStr(groupname), "group");
        //    group.Properties["name"].Value = groupname;
        //    group.Properties["sAMAccountName"].Value = name2000;
        //    group.Properties["groupType"].Value = ActiveDs.ADS_GROUP_TYPE_ENUM.ADS_GROUP_TYPE_UNIVERSAL_GROUP | ActiveDs.ADS_GROUP_TYPE_ENUM.ADS_GROUP_TYPE_SECURITY_ENABLED;
        //    group.CommitChanges();
        //    return group;
        //}

        public static string ProcLDAPStr(string LDAPStr)
        {
            if (!string.IsNullOrEmpty(LDAPStr))
            {
                LDAPStr = LDAPStr.Replace("#", "\\#");
                LDAPStr = LDAPStr.Replace("\"", "\\\"");
                return LDAPStr;
            }
            else
                return LDAPStr;
        }
        /// <summary>
        /// 根据组名返回用户组
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="groupname"></param>
        /// <returns></returns>
        public static DirectoryEntry GetGroup(DirectoryEntry entry, string groupname)
        {
            if (string.IsNullOrEmpty(groupname))
            {
                return null;
            }
            DirectorySearcher searcher = new DirectorySearcher(entry);
            searcher.Filter = "(&(objectClass=group)(name=" + groupname + "))";
            searcher.CacheResults = false;
            searcher.PropertyNamesOnly = true;
            searcher.PropertiesToLoad.Add("cn");
            searcher.PropertiesToLoad.Add("distinguishedName");
            searcher.PropertiesToLoad.Add("Description");
            searcher.PropertiesToLoad.Add("memberOf");

            SearchResult result = searcher.FindOne();
            DirectoryEntry group = null;
            if (result != null)
            {
                group = result.GetDirectoryEntry();
            }
            entry.Dispose();
            searcher.Dispose();
            return group;
        }
        /// <summary>
        /// 当前用户是否在用户组内
        /// </summary>
        /// <param name="userEntry"></param>
        /// <param name="groupDistinguishedName"></param>
        /// <returns></returns>
        public static bool IsUserInGroup(DirectoryEntry userEntry, string groupDistinguishedName)
        {
            PropertyValueCollection pvc = userEntry.Properties["memberOf"];
            int count = pvc.Count;
            if (count < 1)
            {
                return false;
            }
            groupDistinguishedName = groupDistinguishedName.ToLower();
            for (int i = 0; i < count; i++)
            {
                string member = pvc[i].ToString();
                if (member.ToLower() == groupDistinguishedName)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 添加对象到组
        /// </summary>
        /// <param name="groupEntry">组</param>
        /// <param name="objectEntry">对象</param>
        public static void AddObjectToGroup(DirectoryEntry groupEntry, DirectoryEntry objectEntry)
        {
            try
            {

                string groupDistinguishedName = GetProperty(groupEntry, "distinguishedName");
                string objectDistinguishedName = GetProperty(objectEntry, "distinguishedName");
                if (string.IsNullOrEmpty(groupDistinguishedName) || string.IsNullOrEmpty(objectDistinguishedName))
                {
                    return;
                }
                if (IsUserInGroup(objectEntry, groupDistinguishedName))
                {
                    return;
                }
                SetPropertyGroup(groupEntry, "member", objectDistinguishedName);
                groupEntry.CommitChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 将用户添加至用户组
        /// </summary>
        /// <param name="OUEntry"></param>
        /// <param name="userEntry"></param>
        /// <param name="groupName"></param>
        public static void AddUserToGroup(DirectoryEntry OUEntry, DirectoryEntry userEntry, string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                return;
            }
            DirectoryEntry groupEntry = GetGroup(OUEntry, groupName);
            //if (groupEntry == null)
            //{
            //    groupEntry = CreateGroup(OUEntry, groupName);
            //}
            string groupDistinguishedName = GetProperty(groupEntry, "distinguishedName");
            string userDistinguishedName = GetProperty(userEntry, "distinguishedName");
            if (string.IsNullOrEmpty(groupDistinguishedName) || string.IsNullOrEmpty(userDistinguishedName))
            {
                return;
            }
            if (IsUserInGroup(userEntry, groupDistinguishedName))
            {
                return;
            }
            SetPropertyGroup(groupEntry, "member", userDistinguishedName);
            groupEntry.CommitChanges();
        }

        public static void AddUserToGroup(DirectoryEntry entry, string username, string groupName)
        {
            DirectorySearcher searcher = new DirectorySearcher(entry);
            searcher.Filter = "(&(objectClass=group) (cn=" + groupName + "))";
            SearchResultCollection results = searcher.FindAll();
            if (results.Count != 0)
            {
                DirectorySearcher searcherUser = new DirectorySearcher(entry);
                searcherUser.Filter = "(&(objectClass=user)(cn=" + username + "))";
                SearchResultCollection resultsUser = searcherUser.FindAll();
                if (resultsUser.Count != 0)
                {
                    DirectoryEntry group = results[0].GetDirectoryEntry();
                    group.Invoke("Add", new object[] { resultsUser[0].Path });
                    group.CommitChanges();
                    group.Close();
                }
            }
            return;
        }

        public static void DeleteUserToGroup(DirectoryEntry entry, string username, string groupName)
        {
            DirectorySearcher searcher = new DirectorySearcher(entry);
            searcher.Filter = "(&(objectClass=group) (cn=" + groupName + "))";
            SearchResultCollection results = searcher.FindAll();
            if (results.Count != 0)
            {
                DirectorySearcher searcherUser = new DirectorySearcher(entry);
                searcherUser.Filter = "(&(objectClass=user)(cn=" + username + "))";
                SearchResultCollection resultsUser = searcherUser.FindAll();
                if (resultsUser.Count != 0)
                {
                    DirectoryEntry group = results[0].GetDirectoryEntry();
                    try
                    {
                        group.Invoke("Remove", new object[] { resultsUser[0].Path });
                    }
                    catch
                    { }
                    group.CommitChanges();
                    group.Close();
                }
            }
        }

        /// <summary>
        /// 修改组
        /// </summary>
        /// <param name="entry"></param>
        public static void UpdateGroup(DirectoryEntry entry, string desc)
        {
            if (!string.IsNullOrEmpty(desc))
            {
                entry.Properties["Description"].Value = desc;
                entry.CommitChanges();
            }
            else
            {
                entry.Properties["Description"].Value = null;
                entry.CommitChanges();
            }
        }

        /// <summary>
        /// 判断当前给定组名是否在所属与给定OUPath对象下
        ///     例如：给定组名：0011_lyy_23_33
        ///           给定OUPath：OU=L3DT,DC=addom,DC=xinaogroup,DC=com
        ///     当前方法判断组名：0011_lyy_23_33 是否所属与L3DT的OU为下
        /// </summary>
        /// <param name="ouPath"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static bool IsGroupInOUPath(string ouPath, string groupName)
        {
            using (DirectoryEntry parent = GetDirectoryEntry(ouPath))
            {
                using (DirectorySearcher searcher = new DirectorySearcher())
                {
                    searcher.SearchRoot = parent;
                    searcher.CacheResults = false;
                    searcher.SearchScope = SearchScope.Subtree;
                    searcher.Filter = "(&(objectClass=group)(name=" + groupName + "))";
                    SearchResult result = searcher.FindOne();
                    if (result != null)
                    {
                        return true;
                    }
                    else
                        return false;
                }
            }
        }

        /// <summary>
        /// 获取组类型的int值
        /// </summary>
        /// <returns></returns>
        public static int GetGroupTypeValue()
        {
            int val = (int)(ActiveDs.ADS_GROUP_TYPE_ENUM.ADS_GROUP_TYPE_GLOBAL_GROUP
            | ActiveDs.ADS_GROUP_TYPE_ENUM.ADS_GROUP_TYPE_SECURITY_ENABLED);
            return val;

        }

        #endregion

        #region 用户操作



        #region 获取单个用户

        /// <summary>
        /// 根据用户登录名查询用户
        /// </summary>
        /// <param name="ouDE"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public static DirectoryEntry GetUserEntryByAccount(DirectoryEntry ouDE, string account, SearchScope scope = SearchScope.Subtree)
        {
            if (null == ouDE || string.IsNullOrWhiteSpace(account))
                return null;
            DirectorySearcher searcher = new DirectorySearcher(ouDE);
            searcher.Filter = "(&(objectClass=user)(sAMAccountName=" + account + "))";
            searcher.SearchScope = scope;
            SearchResult result = searcher.FindOne();
            ouDE.Close();
            if (result != null)
            {
                return result.GetDirectoryEntry();
            }
            return null;
        }


        /// <summary>
        /// 根据用户公共名称取得用户的 对象
        /// </summary>
        /// <param name="ouDE">部门</param>
        /// <param name="realName">用户公共名称</param>
        /// <returns>如果找到该用户则返回用户的对象,否则返回 null</returns>
        public static DirectoryEntry GetSubUser(DirectoryEntry ouDE, string realName, SearchScope scope = SearchScope.Subtree)
        {
            DirectorySearcher deSearch = new DirectorySearcher(ouDE);
            deSearch.Filter = "(&(objectClass=user)(CN=" + realName.Replace("\\", "").Replace("CN=", "") + "))";
            deSearch.SearchScope = scope;
            //try
            //{
            SearchResult result = deSearch.FindOne();
            if (result == null)
                return null;
            var de = result.GetDirectoryEntry();
            return de;
            //}
            //catch (Exception ex)
            //{
            //    LogFactory.ExceptionLog.Error(ex);
            //    return null;
            //}
        }

        /// <summary>
        /// 通过用户编码获取用户
        /// 用户名区分是否带a
        /// </summary>
        /// <param name="ouDE">所在OU</param>
        /// <param name="realName">用户姓名</param>
        /// <param name="userCode">用户编号</param>
        /// <returns></returns>
        public static DirectoryEntry GetSubUserByNameAndCode(DirectoryEntry ouDE, string realName, string userCode, string userCodePropertyName, SearchScope scope = SearchScope.Subtree)
        {
            DirectorySearcher deSearch = new DirectorySearcher(ouDE);
            deSearch.Filter = "(&(objectClass=user)(" + userCodePropertyName + "=" + userCode + "))";
            deSearch.SearchScope = scope;
            //try
            //{
            if (string.IsNullOrEmpty(realName))
            {
                SearchResult sr = deSearch.FindOne();
                if (sr != null)
                    return sr.GetDirectoryEntry();
                return null;
            }
            else
            {
                SearchResultCollection resList = deSearch.FindAll();
                if (resList == null)
                    return null;
                foreach (SearchResult sr in resList)
                {
                    var de = sr.GetDirectoryEntry();

                    if ((realName.IndexOf("a") > -1 && de.Name.IndexOf("a") > -1)
                        || (realName.IndexOf("a") == -1 && de.Name.IndexOf("a") == -1))
                    {
                        return de;
                    }
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    LogFactory.ExceptionLog.Error(ex);

            //}
            return null;
        }

        /// <summary>
        /// 通过用户编码获取用户
        /// </summary>
        /// <param name="ouDE">所在OU</param>
        /// <param name="userCodePropertyName">用户编号所在属性名</param>
        /// <param name="userCode">用户编号</param>
        /// <returns></returns>
        public static DirectoryEntry GetSubUserByCode(DirectoryEntry ouDE, string userCodePropertyName, string userCode, SearchScope scope = SearchScope.Subtree)
        {
            DirectorySearcher deSearch = new DirectorySearcher(ouDE);
            deSearch.Filter = "(&(objectClass=user)(" + userCodePropertyName + "=" + userCode + "))";
            deSearch.SearchScope = scope;
            //try
            //{
            var sr = deSearch.FindOne();
            var de = sr.GetDirectoryEntry();
            return de;
            //}
            //catch (Exception ex)
            //{
            //    LogFactory.ExceptionLog.Error(ex);

            //}
            return null;
        }

        /// <summary>
        /// 通过用户名获取直接OU下的用户
        /// </summary>
        /// <param name="ouDE"></param>
        /// <param name="realName">用户公共名称</param>
        /// <returns></returns>
        public static DirectoryEntry GetDirectSubUser(DirectoryEntry ouDE, string realName)
        {
            return GetSubUser(ouDE, realName, SearchScope.OneLevel);
        }


        /// <summary>
        /// 通过用户编码获取直接OU下的用户
        /// 用户名区分是否带a
        /// </summary>
        /// <param name="ouDE">所在OU</param>
        /// <param name="realName">用户姓名</param>
        /// <param name="userCode">用户编号</param>
        /// <returns></returns>
        public static DirectoryEntry GetDirectSubUserByNameAndCode(DirectoryEntry ouDE, string realName, string userCode, string userCodePropertyName)
        {
            return GetSubUserByNameAndCode(ouDE, realName, userCode, userCodePropertyName, SearchScope.OneLevel);
        }

        #endregion


        #region 获取多个用户


        public static SearchResultCollection GetSubUsers(DirectoryEntry parent, DateTime startTime, DateTime? endTime = null, SearchScope scope = SearchScope.Subtree, string datetimePropterty = "whenChanged")
        {
            return GetSubDirectoryEntrys(parent, Type_User, startTime, endTime, scope, datetimePropterty);
        }


        /// <summary>
        /// 获取所有子User
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static SearchResultCollection GetAllSubUsers(DirectoryEntry parent)
        {
            DirectorySearcher deSearch = new DirectorySearcher(parent);
            deSearch.Filter = "(&(objectCategory=person)(objectClass=user))";
            deSearch.SearchScope = SearchScope.Subtree;
            SearchResultCollection srList = deSearch.FindAll();
            return srList;
        }

        /// <summary>
        /// 获取所有子User
        /// </summary>
        /// <param name="orgName"></param>
        /// <returns></returns>
        public SearchResultCollection GetAllSubUsers(string orgName)
        {
            return GetAllSubDirectoryEntrys(orgName, Type_User);
        }


        /// <summary>
        /// 获取直接OU下的用户
        /// </summary>
        /// <param name="parent">部门</param>
        /// <returns>如果找到该用户则返回用户的对象,否则返回 null</returns>
        public static SearchResultCollection GetDirectSubUsers(DirectoryEntry parent)
        {
            DirectorySearcher deSearch = new DirectorySearcher(parent);
            deSearch.Filter = "(&(objectCategory=person)(objectClass=user))";
            deSearch.SearchScope = SearchScope.OneLevel;
            //try
            //{
            SearchResultCollection srList = deSearch.FindAll();
            return srList;
            //}
            //catch (Exception ex)
            //{
            //    LogFactory.ExceptionLog.Error(ex);
            //    return null;
            //}
        }


        #endregion

        /// <summary>
        /// 当前用户已加入的组
        /// </summary>
        /// <param name="userEntry"></param>
        /// <returns></returns>
        public static HashSet<string> UserInGroup(DirectoryEntry userEntry)
        {
            //PropertyValueCollection pvc = userEntry.Properties["memberOf"].Value;
            Object[] stringArray = (Object[])userEntry.Properties["memberOf"].Value;
            int count = stringArray.Count();
            if (count < 1)
            {
                return new HashSet<string>();
            }
            HashSet<string> groups = new HashSet<string>();
            for (int i = 0; i < count; i++)
            {
                string member = GetDirectoryEntry(EscapeFilterLiteral( stringArray[i].ToString(),false)).Guid.ToString();
                if (!groups.Contains(member))
                    groups.Add(member);

            }
            return groups;
        }

        public static string EscapeFilterLiteral(string literal, bool escapeWildcards)
        {
            if (literal == null) return string.Empty;

            //literal = literal.Replace("\",@"\5c");
            literal = literal.Replace("(", @"\28");
            literal = literal.Replace(")", @"\29");
            literal = literal.Replace("\0", @"\00");
            literal = literal.Replace("/", @"\2f");
            if (escapeWildcards) literal = literal.Replace("*", @"\2a");
            return literal;
        }
      

        /// <summary>
        /// 根据员工ID获取对应AD域账号
        /// </summary>
        /// <param name="EmployeeID"></param>
        /// <returns></returns>
        public string GetAccountByEmployeeID(string EmployeeID)
        {
            string Account = string.Empty;
            //try
            //{
            DirectoryEntry entry = new DirectoryEntry(LDAPPath, DomainUser, DomainPass, AuthenticationTypes.Secure);
            Object obj = entry.NativeObject;
            DirectorySearcher search = new DirectorySearcher(entry);
            search.Filter = "EmployeeID=" + EmployeeID;// "(SAMAccountName=qiu.fangbing)";
            search.PropertiesToLoad.Add("cn");
            SearchResult result = search.FindOne();
            DirectoryEntry user = result.GetDirectoryEntry();

            Account = Convert.ToString(user.Invoke("Get", new object[] { "SAMAccountName" }));
            //string AD = user.Properties["SAMAccountName"].Value.ToString();
            //string FullName = Convert.ToString(user.Invoke("Get", new object[] { "displayName" }));
            //string Email = Convert.ToString(user.Invoke("Get", new object[] { "mail" }));               
            //string Path = Convert.ToString(user.Invoke("Get", new object[] { "distinguishedName" }));
            //}
            //catch
            //{
            //    Account = string.Empty;
            //}
            return Account;
        }


        /// <summary>
        /// 删除path下的User
        /// </summary>
        /// <param name="path"></param>
        /// <param name="realName">要删除的用户</param>
        /// <returns></returns>
        public static bool DeleteUser(DirectoryEntry userDE, string realName)
        {
            if (!string.IsNullOrEmpty(realName))
            {
                DirectoryEntry AD = userDE;
                DirectoryEntry NewUser = AD.Children.Find("CN=" + realName + "", "User");
                AD.Children.Remove(NewUser);
                AD.CommitChanges();
                AD.Close();
                return true;
            }
            return false;
        }


        /// <summary>
        /// 添加用户到某个组
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        public static void AddUserToGroup(DirectoryEntry group, DirectoryEntry user)
        {
            group.Properties["member"].Add(user.Properties["distinguishedName"].Value);
        }

        /// <summary>
        /// 从某个组移除用户
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        public static void RemoveUserfromGroup(DirectoryEntry group, DirectoryEntry user)
        {
            group.Properties["member"].Remove(user.Properties["distinguishedName"].Value);
        }


        /// <summary>
        /// 设置密码
        /// </summary>
        /// <param name="de"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static bool SetPassword(DirectoryEntry de, string pwd)
        {
            bool isLogin = false;
            //try
            //{
            object obj = de.Invoke("SetPassword", new object[] { pwd });
            isLogin = true;
            //}
            //catch (Exception ex)
            //{
            //    LogFactory.ExceptionLog.Error(ex);
            //    //Common.WriteLog(ex.ToString());
            //}
            return isLogin;
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="ou"></param>
        /// <param name="name"></param>
        /// <param name="loginName"></param>
        /// <param name="loginName2000"></param>
        /// <param name="password"></param>
        /// <param name="principalDomain">登录域</param>
        /// <param name="userAccountControl"></param>
        /// <param name="userCodeName"></param>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public static DirectoryEntry AddUser(DirectoryEntry ou, string name, string userPrincipalName, string loginName2000,
            string password, string userAccountControl, string userCodePropertyName, string userCode)
        {
            DirectoryEntries users = ou.Children;
            DirectoryEntry userEntry = users.Add("CN=" + ProcLDAPStr(name), "user");
            SetProperty(userEntry, "SAMAccountName", loginName2000);
            //userEntry.CommitChanges();
            //string userPrincipalName = loginName + "@" + principalDomain;
            SetProperty(userEntry, "userPrincipalName", userPrincipalName);
            if (userCodePropertyName.Equals("Guid", StringComparison.OrdinalIgnoreCase))
            {

            }
            else
            {
                SetProperty(userEntry, userCodePropertyName, userCode);
            }
            //userEntry.Properties["userAccountControl"].Value = int.Parse(userAccountControl);
            userEntry.CommitChanges();
            //if (!(IsAccountActive(userEntry)))
            //{
            //    EnableUserAccount(userEntry);
            //}

            //if (isExpried == false)
            //{
            //    object ti = userEntry.Properties["userAccountControl"].Value;
            userEntry.Properties["userAccountControl"].Value = int.Parse(userAccountControl);
            SetPassword(userEntry, password);
            userEntry.CommitChanges();
            //}
            //userEntry.CommitChanges();
            userEntry.Close();
            ou.Close();
            return userEntry;


        }

        /// <summary>
        /// 创建帐户禁用的用户
        /// </summary>
        /// <param name="ou"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="principalDomain"></param>
        /// <returns></returns>
        public static DirectoryEntry AddDisabledUser(DirectoryEntry ou, string account, string password, string principalDomain)
        {
            DirectoryEntries users = ou.Children;
            DirectoryEntry userEntry = users.Add("CN=" + account, "user");
            SetProperty(userEntry, "SAMAccountName", account);
            string userPrincipalName = account + "@" + principalDomain;
            SetProperty(userEntry, "userPrincipalName", userPrincipalName);
            SetProperty(userEntry, "userAccountControl", "514");
            userEntry.CommitChanges();
            SetPassword(userEntry, password);
            userEntry.Close();
            ou.Close();
            return userEntry;
        }

        public static bool ChangeUserName(DirectoryEntry ouEntry, string newUserName)
        {
            //try
            //{
            ouEntry.Rename("CN=" + newUserName);

            ouEntry.CommitChanges();
            return true;
            //}
            //catch (Exception ex)
            //{
            //    LogFactory.ExceptionLog.Error(ex);
            //    return false;
            //}
        }


        #endregion

        #region 属性操作

        /// <summary>
        /// 设置指定的属性值
        /// </summary>
        /// <param name="de"></param>
        /// <param name="propertyName">属性名称?</param>
        /// <param name="propertyValue">属性值</param>
        public static void SetProperty(DirectoryEntry de, string propertyName, string propertyValue)
        {
            if (de.Properties.Contains(propertyName))
            {
                if (string.IsNullOrEmpty(propertyValue))
                {
                    de.Properties[propertyName].RemoveAt(0);
                }
                else
                {
                    de.Properties[propertyName][0] = propertyValue;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(propertyValue))
                {
                    de.Properties[propertyName].Add(propertyValue);
                }
            }
        }

        /// <summary>
        /// 读取属性
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string GetProperty(DirectoryEntry entry, string propertyName)
        {
            try
            {
                if (propertyName.Equals("objectGUID", StringComparison.OrdinalIgnoreCase))
                {
                    return new Guid((entry.Properties[propertyName] as PropertyValueCollection).Value as byte[]).ToString();
                }
                else if (entry.Properties.Contains(propertyName))
                    return entry.Properties[propertyName][0].ToString();
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                string msg = string.Empty;
                if (entry != null)
                {
                    msg = "Guid=" + entry.Guid + ";Name=" + entry.Name + ";propertyName" + propertyName;
                }
                LogFactory.ExceptionLog.Error(msg, ex);
            }
            return string.Empty;
        }

        public static void SetPropertyGroup(DirectoryEntry entry, string propertyName, string propertyValue)
        {
            if (entry.Properties.Contains(propertyName))
            {
                if (string.IsNullOrEmpty(propertyValue))
                {
                    object o = entry.Properties[propertyName].Value;
                    entry.Properties[propertyName].Remove(o);
                }
                else
                {
                    entry.Properties[propertyName].Add(propertyValue);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(propertyValue))
                {
                    return;
                }
                entry.Properties[propertyName].Add(propertyValue);
            }
        }


        #endregion

        #region 获取DE

        /// <summary>
        /// 获取所有子DirectoryEntry
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static SearchResultCollection GetAllSubDirectoryEntrys(DirectoryEntry parent, string type, SearchScope scope = SearchScope.Subtree)
        {
            DirectorySearcher deSearch = new DirectorySearcher(parent);
            deSearch.SearchScope = scope;
            deSearch.Filter = "(objectClass=" + type + ")";
            deSearch.PageSize = 1000;
            deSearch.SizeLimit = int.MaxValue;
            //try
            //{
            SearchResultCollection srList = deSearch.FindAll();
            return srList;
            //}
            //catch (Exception ex)
            //{
            //    LogFactory.ExceptionLog.Error(ex);
            //    return null;
            //}


        }

        /// <summary>
        ///  获取所有子DirectoryEntry
        /// </summary>
        /// <param name="orgName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public SearchResultCollection GetAllSubDirectoryEntrys(string orgName, string type)
        {
            DirectoryEntry root = GetRootEntry(Domain, DomainUser, DomainPass);
            DirectoryEntry rootOU = GetOU(root, orgName);
            return GetAllSubDirectoryEntrys(rootOU, type);

        }

        /// <summary>
        /// 获取某个时间段修改的DE
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="type"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static SearchResultCollection GetSubDirectoryEntrys(DirectoryEntry parent, string type, DateTime startTime, DateTime? endTime = null, SearchScope scope = SearchScope.Subtree, string datetimePropterty = "whenChanged")
        {

            DirectorySearcher deSearch = new DirectorySearcher(parent);
            deSearch.Filter = "(&(objectClass=" + type + ")(" + datetimePropterty + ">=" + ToADDateString(startTime) + ")" + (endTime.HasValue ? "(" + datetimePropterty + "<=" + ToADDateString(endTime.Value) + ")" : "") + ")";
            deSearch.SearchScope = scope;
            deSearch.PageSize = 1000;
            deSearch.SizeLimit = int.MaxValue;
            //try
            //{
            SearchResultCollection resList = deSearch.FindAll();
            return resList;
            //}
            //catch (Exception ex)
            //{
            //    LogFactory.ExceptionLog.Error(ex);

            //}
            return null;
        }

        #endregion

        #region IsAccountActive
        public static bool IsAccountActive(DirectoryEntry user)
        {
            int nIsAlive = Convert.ToInt32(user.Properties["userAccountControl"].Value);
            return IsAccountActive(nIsAlive);
        }

        public static bool IsAccountActive(int userAccountControl)
        {
            int userAccountControl_Disabled = 0X0002;
            int flagExists = userAccountControl & userAccountControl_Disabled;
            return flagExists <= 0;
        }
        /// <summary>
        /// 密码加密是否可逆
        /// </summary>
        /// <param name="userAccountControl"></param>
        /// <returns></returns>
        public static bool IsIsReversible(int userAccountControl)
        {
            bool flag = true;
            int userAccountControl_Disabled = 0X0080;
            int flagExists = userAccountControl & userAccountControl_Disabled;
            if (flagExists == 0)
            {
                flag = false;
            }

            return flag;
            //return flagExists <= 128;
        }

        //public static bool IsSafeGroup(int groupType)
        //{
        //    bool flag = true;
        //    int SECURITY_ENABLED = (int)ADS_GROUP_TYPE_ENUM.ADS_GROUP_TYPE_SECURITY_ENABLED;
        //    int flagExists = groupType & SECURITY_ENABLED;
        //    if (flagExists == 0)
        //    {
        //        flag = false;
        //    }

        //    return flag;
        //    //return flagExists <= 128;
        //}

        #endregion

        #region EnableAccount
        public static void EnableUserAccount(DirectoryEntry userEntry)
        {
            int exp = (int)userEntry.Properties["userAccountControl"].Value;
            userEntry.Properties["userAccountControl"].Value = exp | 0x10000;
            userEntry.CommitChanges();
            int val = (int)userEntry.Properties["userAccountControl"].Value;
            userEntry.Properties["userAccountControl"].Value = val & ~0x2;
            userEntry.CommitChanges();
        }
        public static void EnableComputerAccount(DirectoryEntry userEntry)
        {
            EnableAccount(userEntry);
        }


        public static void EnableAccount(DirectoryEntry userEntry)
        {
            int val = (int)userEntry.Properties["userAccountControl"].Value;
            userEntry.Properties["userAccountControl"].Value = val & ~0x2;
            userEntry.CommitChanges();
        }
        #endregion

        /// <summary>
        /// 格式化AD的时间
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToADDateString(DateTime date)
        {
            string year = date.Year.ToString();
            int month = date.Month;
            int day = date.Day;

            StringBuilder sb = new StringBuilder();
            sb.Append(year);
            if (month < 10)
            {
                sb.Append("0");
            }
            sb.Append(month.ToString());
            if (day < 10)
            {
                sb.Append("0");
            }
            sb.Append(day.ToString());
            sb.Append("000000.0Z");
            return sb.ToString();
        }

    }

}