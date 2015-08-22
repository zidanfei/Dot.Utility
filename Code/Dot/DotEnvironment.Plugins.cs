/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20110331
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20100331
 * 
*******************************************************/

using Dot.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Dot
{
    /// <summary>
    /// Library Module Plugins
    /// </summary>
    public partial class DotEnvironment
    {
        private const string AppPluginFolder = "Domain";
        private const string UIPluginFolder = "UI";

        #region 启动 Plugins

        /// <summary>
        /// 启动所有的 实体插件
        /// </summary>
        internal static void StartupAppPlugins()
        {
            var libraries = GetAppPlugins();

            foreach (var pluginAssembly in libraries)
            {
                //调用 ILibrary
                var library = pluginAssembly.Instance as AppPlugin;
                if (library != null) library.Initialize(_appCore);
            }
        }       

        #endregion

      

        #region 获取所有 Plugins

        private static object _librariesLock = new object();

        private static object _modulesLock = new object();

        private static object _allPluginsLock = new object();

        private static IList<PluginAssembly> _libraries;

        private static IList<PluginAssembly> _modules;

        private static IList<PluginAssembly> _allPlugins;

        /// <summary>
        /// 找到当前程序所有可运行的领域实体插件。
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PluginAssembly> GetAppPlugins()
        {
            if (_libraries == null)
            {
                lock (_librariesLock)
                {
                    if (_libraries == null)
                    {
                        var assemblies = EnumerateAllDomainAssemblies().Union(PluginTable.Assemblys).ToArray();
                        _libraries = LoadSortedPlugins(assemblies);

                        PluginTable.Assemblys.Lock();
                    }
                }
            }
            return _libraries;
        }
  

        private static List<PluginAssembly> LoadSortedPlugins(IEnumerable<Assembly> assemblies)
        {
            var list = assemblies.Select(assembly =>
            {
                var pluginType = assembly.GetTypes().FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                IPlugin pluginInstance = null;
                if (pluginType != null)
                {
                    pluginInstance = Activator.CreateInstance(pluginType) as IPlugin;
                    //throw new NotSupportedException("所有插件包中必须有且仅有一个实现 IPlugin 接口的类型！" + Environment.NewLine + "文件路径：" + file);
                }
                else
                {
                    pluginInstance = new EmptyPlugin(assembly);
                }

                return new PluginAssembly(assembly, pluginInstance);
            })
            .ToList();

            //将 list 中集合中的元素，先按照 SetupLevel 排序；
            //然后同一个启动级别中的插件，再按照引用关系来排序。
            var sorted = new List<PluginAssembly>(list.Count);
            var groups = list.GroupBy(a => a.Instance.SetupLevel).OrderBy(g => g.Key);
            var index = 0;
            foreach (var group in groups)
            {
                var sortedItems = SortByReference(group);
                for (int i = 0, c = sortedItems.Count; i < c; i++)
                {
                    var item = sortedItems[i];
                    item.SetupIndex = index++;
                    sorted.Add(item);
                }
            }

            return sorted;
        }

        private static List<PluginAssembly> SortByReference(IEnumerable<PluginAssembly> list)
        {
            //items 表示待处理列表。
            var items = list.ToList();
            var sorted = new List<PluginAssembly>(items.Count);

            while (items.Count > 0)
            {
                for (int i = 0, c = items.Count; i < c; i++)
                {
                    var item = items[i];
                    bool referencesOther = false;
                    var refItems = item.Assembly.GetReferencedAssemblies();
                    for (int j = 0, c2 = items.Count; j < c2; j++)
                    {
                        if (i != j)
                        {
                            if (refItems.Any(ri => ri.FullName == items[j].Assembly.FullName))
                            {
                                referencesOther = true;
                                break;
                            }
                        }
                    }
                    //没有被任何一个程序集引用，则把这个加入到结果列表中，并从待处理列表中删除。
                    if (!referencesOther)
                    {
                        sorted.Add(item);
                        items.RemoveAt(i);

                        //跳出循环，从新开始。
                        break;
                    }
                }
            }

            return sorted;
        }

        private static IEnumerable<Assembly> EnumerateAllDomainAssemblies()
        {
            yield return LoadAssembly("Dot");

            foreach (var file in EnumerateAllAppPluginFiles())
            {
                yield return Assembly.LoadFrom(file);
            }
        }

        private static IEnumerable<string> EnumerateAllAppPluginFiles()
        {
            //查找Library目录下的所有程序集
            string libraryPath = MapDllPath(AppPluginFolder + "\\");
            if (Directory.Exists(libraryPath))
            {
                foreach (var dll in Directory.GetFiles(libraryPath, "*.dll"))
                {
                    yield return dll;
                }
            }

          
        }


        private static Assembly LoadAssembly(string name)
        {
            var aName = typeof(DotEnvironment).Assembly.GetName();
            aName.Name = name;
            return Assembly.Load(aName);
        }

        #endregion

       
         

    }
}