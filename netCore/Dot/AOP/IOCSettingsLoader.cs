
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dot.Utility;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Dot.Utility.Config;
using Dot.Config.Model;
using Dot.IOC;

namespace Dot.AOP
{
    class IOCSettingsLoader
    {
        private UnityContainer _container;
        private IOCSetting _settings;

        internal IOCSettingsLoader(UnityContainer container)
        {
            _container = container;
            if (PlatformConfig.ServerConfig != null)
                _settings = PlatformConfig.ServerConfig.IOCSetting;
        }

        internal void Load()
        {
            if (_settings == null) { return; }

            this.LoadAllPlugins();

            this.LoadObjectItems();

            this.LoadCustomItems();
        }

        private void LoadAllPlugins()
        {
            var assemblies = DotEnvironment.GetAppPlugins().Select(p => p.Assembly);
            var types = assemblies.SelectMany(a => a.GetTypes())
                .Where(t =>
                    !t.IsAbstract &&
                    !t.IsInterface &&
                    !t.IsGenericTypeDefinition &&
                    t.IsDefined(typeof(ExportAttribute), false)
                    ).ToArray();
            foreach (var toType in types)
            {
                this.RegisterType(toType);
            }
        }

        private void LoadObjectItems()
        {
            var objectItems = _settings.ObjectItems;
            if (objectItems != null && objectItems.Count > 0)
            {
                foreach (var item in objectItems)
                {
                    if (string.IsNullOrEmpty(item.Implement)) { continue; }
                    var toType = Type.GetType(item.Implement);
                    if (toType != null)
                    {
                        this.RegisterType(toType);
                    }
                }
            }
        }

        private void LoadCustomItems()
        {
            var objectItems = _settings.CustomObjectItems;
            if (objectItems != null && objectItems.Count > 0)
            {
                foreach (var item in objectItems)
                {
                    if (string.IsNullOrEmpty(item.Implement)) { continue; }
                    var toType = Type.GetType(item.Implement);
                    if (toType != null)
                    {
                        this.RegisterType(toType);
                    }
                }
            }
        }

        private void RegisterType(Type toType)
        {
            //通过 InterceptorItems 来创建需要的拦截器对象。
            var injectionMembers = new List<InjectionMember>();
            if (_settings.InterceptorItems != null)
            {
                string assemblyName = toType.Assembly.GetName().Name;
                foreach (var InterceptorItem in _settings.InterceptorItems)
                {
                    if (string.IsNullOrEmpty(InterceptorItem.TypeName)) continue;

                    var assembly = InterceptorItem.InterceptorAssemblys.FirstOrDefault(pre => pre.AssemblyName == assemblyName);
                    if (assembly == null) continue;
                    if (assembly.InterceptorTypeIgnores.Any(pre => pre.Type == toType.FullName)) continue;

                    Type behaviorType = Type.GetType(InterceptorItem.TypeName, false);
                    if (behaviorType == null) continue;

                    injectionMembers.Add(new InterceptionBehavior(behaviorType));
                }
            }
            //数组化
            var injections = InterceptionHelper.EmptyInjections;
            if (injectionMembers.Count > 0)
            {
                injectionMembers.Add(new Interceptor<InterfaceInterceptor>());
                injections = injectionMembers.ToArray();
            }

            InterceptionHelper.RegisterType(toType, injections);
        }

        private static bool IsSingleton(string lifeCycle)
        {
            return !string.IsNullOrWhiteSpace(lifeCycle) && lifeCycle.ToLower() == "singleton";
        }
    }
}