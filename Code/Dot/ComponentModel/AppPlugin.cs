
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dot.ComponentModel
{
    /// <summary>
    /// 插件程序集基类。
    /// </summary>
    public abstract class AppPlugin : IPlugin
    {
        /// <summary>
        /// 插件对应的程序集。
        /// </summary>
        public Assembly Assembly
        {
            get { return this.GetType().Assembly; }
        }

        /// <summary>
        /// 插件的启动级别。默认值为 <see cref=".ComponentModel.ReuseLevel.Main"/>。
        /// </summary>
        protected virtual int SetupLevel
        {
            get { return ReuseLevel.Main; }
        }

        /// <summary>
        /// 插件的初始化方法。
        /// 框架会在启动时根据启动级别顺序调用本方法。
        /// 
        /// 方法有两个职责：
        /// 1.依赖注入。
        /// 2.注册 app 生命周期中事件，进行特定的初始化工作。
        /// </summary>
        /// <param name="app">应用程序对象。</param>
        public abstract void Initialize(IApp app);

        int IPlugin.SetupLevel
        {
            get { return this.SetupLevel; }
        }
    }
}