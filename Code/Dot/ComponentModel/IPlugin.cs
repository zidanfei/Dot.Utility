
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dot.ComponentModel
{
    /// <summary>
    /// 插件定义。
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// 插件对应的程序集。
        /// </summary>
        Assembly Assembly { get; }

        /// <summary>
        /// 插件的启动级别
        /// </summary>
        int SetupLevel { get; }

        /// <summary>
        /// 插件的初始化方法。
        /// 框架会在启动时根据启动级别顺序调用本方法。
        /// 
        /// 方法有两个职责：
        /// 1.依赖注入。
        /// 2.注册 app 生命周期中事件，进行特定的初始化工作。
        /// </summary>
        /// <param name="app">应用程序对象。</param>
        void Initialize(IApp app);
    }

    //[Flags]
    //public enum PluginComponent
    //{
    //    None = 0,
    //    Entity = 1,
    //    EntityRepository = 2,
    //    EntityRepositoryDataProvider = 4,
    //    EntityConfiguration = 8,


    //    ViewConfiguration = 1024,

    //    All = Entity | EntityRepository | EntityRepositoryDataProvider | EntityConfiguration
    //        | ViewConfiguration
    //}

    ///// <summary>
    ///// 本领域插件提供的所有
    ///// </summary>
    //public virtual DomainPluginComponent Components
    //{
    //    get { return DomainPluginComponent.All; }
    //}

    //public abstract class Plugin
    //{
    //    System.Reflection.BindingFlags
    //}
}
