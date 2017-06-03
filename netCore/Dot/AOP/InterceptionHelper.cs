
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Dot.IOC;
using Dot.ComponentModel;
using Dot;

namespace Dot.AOP
{
    /// <summary>
    /// 带有拦截器功能的 IOC 注册方法集。
    /// </summary>
    public static class InterceptionHelper
    {
        private static UnityContainer _unityContainer;

        /// <summary>
        /// 获取 IOC 容器中真实使用的 UnityContainer。
        /// </summary>
        /// <returns></returns>
        public static UnityContainer UnityContainer
        {
            get
            {
                if (_unityContainer == null)
                {
                    _unityContainer = UnityAdapterHelper.GetUnityContainer(Composer.ObjectContainer);

                    _unityContainer.AddExtension(new Interception());
                }

                return _unityContainer;
            }
        }

        /// <summary>
        /// 空的拦截数组。
        /// </summary>
        public static InjectionMember[] EmptyInjections = new InjectionMember[0];

        /// <summary>
        /// 把 IInterceptionBehavior 列表转换为可用于 UnityContainer 的 InjectionMember 列表。
        /// </summary>
        /// <param name="interceptors"></param>
        /// <returns></returns>
        public static InjectionMember[] CreateInjections(params IInterceptionBehavior[] interceptors)
        {
            if (interceptors == null) throw new ArgumentNullException("interceptors");

            //转换 injectionMembers
            if (interceptors != null && interceptors.Length > 0)
            {
                var injections = new List<InjectionMember>();
                foreach (var item in interceptors)
                {
                    injections.Add(new InterceptionBehavior(item));
                }
                if (injections.Count > 0)
                {
                    injections.Add(new Interceptor<InterfaceInterceptor>());
                }
                var injectionMembers = injections.ToArray();
                return injectionMembers;
            }
            else
            {
                return EmptyInjections;
            }
        }

        /// <summary>
        /// 是否关闭 RegisterPluginByAttribute 方法。默认为 false。
        /// </summary>
        public static bool DisableRegisterPlugin { get; set; }

        /// <summary>
        /// 注册指定插件中，所有标记了 ExportAttribute 的类型到 IOC 容器中。
        /// 并同时可以指定所需要的拦截器。
        /// 
        /// 注意，可使用 <see cref="DisableRegisterPlugin"/> 属性来禁用此功能。
        /// 
        /// 使用时，应该在插件中注册应用程序的 <see cref="IApp.ComposeOperations"/> 事件，并在事件处理函数中调用。
        /// </summary>
        public static void RegisterPluginByAttribute(
            IPlugin plugin,
            params InjectionMember[] injections
            )
        {
            if (!DisableRegisterPlugin)
            {
                if (injections == null) throw new ArgumentNullException("interceptors");

                RegisterByAttribute(new Assembly[] { plugin.Assembly }, injections);
            }
        }

        /// <summary>
        /// 注册指定程序集中，所有标记了 ExportAttribute 的类型到 IOC 容器中。
        /// 并同时可以指定所需要的拦截器。
        /// 
        /// 使用时，应该重写 <see cref="AppImplementationBase.RaiseComposeOperations"/> 方法，并在方法体内调用。
        /// </summary>
        public static void RegisterByAttribute(
            params InjectionMember[] injections
            )
        {
            if (injections == null) throw new ArgumentNullException("interceptors");

            var assemblies = DotEnvironment.GetAppPlugins().Select(p => p.Assembly);
            RegisterByAttribute(assemblies, injections);
        }

        /// <summary>
        /// 注册指定程序集中，所有标记了 ExportAttribute 的类型到 IOC 容器中。
        /// 并同时可以指定所需要的拦截器。
        /// 
        /// 使用时，应该重写 <see cref="AppImplementationBase.RaiseComposeOperations"/> 方法，并在方法体内调用。
        /// </summary>
        public static void RegisterByAttribute(
            IEnumerable<Assembly> assemblies,
            params InjectionMember[] injections
            )
        {
            if (assemblies == null) throw new ArgumentNullException("assemblies");

            var types = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition && t.IsDefined(typeof(ExportAttribute), false));
            RegisterByAttribute(types, injections);
        }

        /// <summary>
        /// 注册指定类型集合中，所有标记了 ExportAttribute 的类型到 IOC 容器中。
        /// 并同时可以指定所需要的拦截器。
        /// 
        /// 使用时，应该重写 <see cref="AppImplementationBase.RaiseComposeOperations"/> 方法，并在方法体内调用。
        /// </summary>
        public static void RegisterByAttribute(IEnumerable<Type> types, params InjectionMember[] injections)
        {
            foreach (var type in types)
            {
                RegisterType(type, injections);
            }
        }

        /// <summary>
        /// 注册指定的标记了 ExportAttribute 的类型到 IOC 容器中。
        /// 并同时可以指定所需要的拦截器。
        /// 
        /// 使用时，应该重写 <see cref="AppImplementationBase.RaiseComposeOperations"/> 方法，并在方法体内调用。
        /// </summary>
        public static void RegisterType(Type type, params InjectionMember[] injections)
        {
            //处理类型中标记了 ExportAttribute 的类型。
            var unityContainer = UnityContainer;
            var injectionMembers = injections ?? EmptyInjections;

            //对于每一个标记，都注册 IOC
            if (!type.IsAbstract && !type.IsGenericTypeDefinition)
            {
                var attriList = type.GetCustomAttributes(typeof(ExportAttribute), false);
                if (attriList.Length > 0)
                {
                    foreach (ExportAttribute attri in attriList)
                    {
                        if (attri.RegisterWay == RegisterWay.Type)
                        {
                            unityContainer.RegisterType(attri.ProvideFor, type, attri.Key, injectionMembers);
                        }
                        else
                        {
                            unityContainer.RegisterType(
                                attri.ProvideFor,
                                type,
                                attri.Key,
                                new ContainerControlledLifetimeManager(),
                                injectionMembers
                                );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 通过 PlatformConfig.ServerConfig.IOCSetting 来初始化整个 UnityContainer。
        /// 完成两个操作：
        /// 1. 加载所有插件中的 Export 标记的项到容器中，并通过 IOCSetting 中的配置进行拦截。
        /// 2. 加载 IOCSetting 中配置的注入项。
        /// </summary>
        public static void LoadByIOCSettings()
        {
            var loader = new IOCSettingsLoader(UnityContainer);
            loader.Load();
        }
    }
}
