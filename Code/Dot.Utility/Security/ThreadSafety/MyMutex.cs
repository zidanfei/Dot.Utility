using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.AccessControl;

namespace Dot.Utility.Security
{
    public static class MyMutex
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mutexName">互斥对象的名称</param>
        /// <param name="tryCleanAbandoned">是否尝试释放已经存在的被放弃的mutex</param>
        /// <param name="mutex">成功时返回的互斥对象</param>
        /// <param name="errcode">错误代码,0无错误,1无法创建互斥对象,2互斥对象存在,无法获取访问权限,3互斥对象正被其他线程占用,4有被弃用的互斥对象,被本方法释放,5互斥对象已存在</param>
        /// <returns></returns>
        private static bool TryEnter(string mutexName, bool tryCleanAbandoned, out Mutex mutex, out int errcode)
        {
            mutex = null;
            errcode = 0; // 无错误
            if (string.IsNullOrEmpty(mutexName)) return false;
            Mutex m = null;
            bool doesNotExist = false;
            bool unauthorized = false;
            bool mutexWasCreated = false;
            bool hasAccessException = false;
            try
            {
                // 尝试打开现有的
                m = Mutex.OpenExisting(mutexName);
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                // 不存在
                doesNotExist = true;
            }
            catch (UnauthorizedAccessException)
            {
                // 没有访问权限
                unauthorized = true;
            }

            // 如果不存在则创建
            if (doesNotExist)
            {
                MutexSecurity mSec = null;
                try
                {
                    string user = Environment.UserDomainName + "\\" + Environment.UserName;
                    mSec = new MutexSecurity();
                    MutexAccessRule rule = new MutexAccessRule(user,
                        MutexRights.Synchronize | MutexRights.Modify,
                        AccessControlType.Deny);
                    mSec.AddAccessRule(rule);

                    rule = new MutexAccessRule(user,
                        MutexRights.ReadPermissions | MutexRights.ChangePermissions,
                        AccessControlType.Allow);
                    mSec.AddAccessRule(rule);
                }
                catch
                { hasAccessException = true; }
                if (hasAccessException)
                    m = new Mutex(true, mutexName, out mutexWasCreated);
                else
                    m = new Mutex(true, mutexName, out mutexWasCreated, mSec);

                if (mutexWasCreated)
                {
                    mutex = m;
                    return true;
                }
                else
                {
                    errcode = 1; //无法创建对象 
                    return false;
                }
            }

            // 尝试增加访问权限
            if (unauthorized)
            {
                try
                {
                    m = Mutex.OpenExisting(mutexName,
                        MutexRights.ReadPermissions | MutexRights.ChangePermissions);

                    MutexSecurity mSec = m.GetAccessControl();

                    string user = Environment.UserDomainName + "\\" + Environment.UserName;
                    MutexAccessRule rule = new MutexAccessRule(user,
                          MutexRights.Synchronize | MutexRights.Modify,
                          AccessControlType.Deny);
                    mSec.RemoveAccessRule(rule);

                    rule = new MutexAccessRule(user,
                        MutexRights.Synchronize | MutexRights.Modify,
                        AccessControlType.Allow);
                    mSec.AddAccessRule(rule);

                    m.SetAccessControl(mSec);

                    m = Mutex.OpenExisting(mutexName);
                }
                catch (UnauthorizedAccessException)
                {
                    errcode = 2;// 无法获取访问权限
                    return false;
                }
                catch (Exception)
                {
                    errcode = 3;// mutex对象正被其他线程占用     
                    return false;
                }
            }

            // 对于已经存在的具备访问权限了
            if (tryCleanAbandoned)
            {
                try
                {
                    // 尝试立刻获取控制
                    bool waitSuccess = m.WaitOne(0);
                    // 不排除有能正好获取到访问权限的
                    if (!waitSuccess)
                        errcode = 3;// mutex对象正被其他线程占用                    
                    else
                        mutex = m;
                    return waitSuccess;
                }
                // 如果发现是被弃用的                    
                catch (AbandonedMutexException)
                {
                    m.ReleaseMutex();
                    errcode = 4; // 有被弃用的mutex对象
                    return false;
                }
            }
            else
            {
                errcode = 5; // mutex对象存在,用户未尝试释放
                return false;
            }
        }

        public static Mutex TryEnter(string mutexName)
        {
            Mutex mutex;
            int errcode;
            bool result = TryEnter(mutexName, true, out mutex, out errcode);
            return mutex;
        }

        public static void TryRelease(Mutex mutex)
        {
            if (mutex == null) return;
            try { mutex.ReleaseMutex(); }
            catch (ApplicationException)
            { }
        }
    }
}
