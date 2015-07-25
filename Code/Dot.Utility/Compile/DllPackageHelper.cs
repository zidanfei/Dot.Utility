using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Compile
{
    /// <summary> 载入资源中的动态链接库(dll)文件
    /// </summary>
    public class DllPackageHelper
    {
        static Dictionary<string, Assembly> Dlls = new Dictionary<string, Assembly>();
        static Dictionary<string, object> Assemblies = new Dictionary<string, object>();

        static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //程序集
            Assembly ass;
            //获取加载失败的程序集的全名
            var assName = new AssemblyName(args.Name).FullName;
            //判断Dlls集合中是否有已加载的同名程序集
            if (Dlls.TryGetValue(assName, out ass) && ass != null)
            {
                Dlls[assName] = null;//如果有则置空并返回
                return ass;
            }
            else
            {
                throw new DllNotFoundException(assName);//否则抛出加载失败的异常
            }
        }

        /// <summary> 注册资源中的dll
        /// </summary>
        public static void RegistDLL()
        {
            //获取调用者的程序集
            var ass = new StackTrace(0).GetFrame(1).GetMethod().Module.Assembly;
            //判断程序集是否已经处理
            if (Assemblies.ContainsKey(ass.FullName))
            {
                return;
            }
            //程序集加入已处理集合
            Assemblies.Add(ass.FullName, null);
            //绑定程序集加载失败事件(这里我测试了,就算重复绑也是没关系的)
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
            //获取所有资源文件文件名
            var res = ass.GetManifestResourceNames();
            foreach (var r in res)
            {
                //如果是dll,则加载
                if (r.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var s = ass.GetManifestResourceStream(r);
                        var bts = new byte[s.Length];
                        s.Read(bts, 0, (int)s.Length);
                        var da = Assembly.Load(bts);
                        //判断是否已经加载
                        if (Dlls.ContainsKey(da.FullName))
                        {
                            continue;
                        }
                        Dlls[da.FullName] = da;
                    }
                    catch
                    {
                        //加载失败就算了...
                    }
                }
            }
        }
    }
}
