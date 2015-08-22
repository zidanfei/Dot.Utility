
using Dot.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Windows; 

namespace Dot
{
    /// <summary>
    /// Rafy 的上下文环境
    /// </summary>
    public static partial class DotEnvironment
    {
        #region Provider

        private static EnvironmentProvider _provider = new EnvironmentProvider();

        /// <summary>
        /// 获取应用程序环境的值提供器。
        /// </summary>
        /// <value>
        /// The provider.
        /// </value>
        public static EnvironmentProvider Provider
        {
            get { return _provider; }
        }

        #endregion

        #region 身份

        /// <summary>
        /// 返回当前用户。
        /// 
        /// 本属性不会为 null，请使用 IsAuthenticated 属性来判断是否已经登录。
        /// </summary>
        public static IIdentity Identity
        {
            get
            {
                var principal = Principal;
                if (principal != null)
                {
                    var user = principal.Identity as IIdentity;
                    if (user != null) return user;
                }

                return null;
            }
        }

        /// <summary>
        /// 返回当前身份。
        /// 
        /// 可能返回 null。如果不想判断 null，请使用 Identity 属性。
        /// </summary>
        public static IPrincipal Principal
        {
            get
            {
                var current = Thread.CurrentPrincipal;
               
                return current;
            }
           
        }

        #endregion
 

        #region IApp AppCore

        private static IApp _appCore;

        /// <summary>
        /// 当前的应用程序运行时。
        /// </summary>
        public static IApp App
        {
            get { return _appCore; }
        }

        internal static void SetApp(IApp appCore)
        {
            _appCore = appCore;
        }

        #endregion




        #region NewLocalId

        private static int _maxId = 1000000000;// 本地临时 Id 从 这个值开始（int.MaxValue:2147483647)
        private static object _maxIdLock = new object();

        /// <summary>
        /// 返回一个本地的 Id，该 Id 在当前应用程序中是唯一的，每次调用都会自增一。
        /// </summary>
        /// <returns></returns>
        public static int NewLocalId()
        {
            lock (_maxIdLock) { return _maxId++; }
        }

        #endregion

        /// <summary>
        /// 帮助调试的变量，可随时把即时窗口中的临时对象放在这里进行查看。
        /// </summary>
        public static object DebugHelper;


        #region Path Mapping

        /// <summary>
        /// 使用一个相对的路径来计算绝对路径
        /// </summary>
        /// <param name="appRootRelative"></param>
        /// <returns></returns>
        public static string MapDllPath(string appRootRelative)
        {
            return Path.Combine(Provider.DllRootDirectory, appRootRelative);
        }

        /// <summary>
        /// 相对路径转换为绝对路径。
        /// </summary>
        /// <param name="appRootRelative"></param>
        /// <returns></returns>
        public static string MapAbsolutePath(string appRootRelative)
        {
            return Path.Combine(Provider.RootDirectory, appRootRelative);
        }

        /// <summary>
        /// 把绝对路径转换为相对路径。
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <returns></returns>
        public static string MapRelativePath(string absolutePath)
        {
            return absolutePath.Replace(Provider.RootDirectory, string.Empty);
        }

        #endregion
    }
}