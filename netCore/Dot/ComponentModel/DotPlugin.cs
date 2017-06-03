
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dot.ComponentModel;
using Dot.Adapter;
using Dot.Config.Model;
using Dot.Log;

namespace Dot.IOC
{
    /// <summary>
    /// 从 Unity 适配到 IObjectContainer 的插件。
    /// 使用此插件后， 平台的 IOC 框架将由 UnityContainer 来实现。
    /// </summary>
    public class DotPlugin : AppPlugin
    {
        /// <summary>
        /// 启动级别：系统级。
        /// </summary>
        protected override int SetupLevel
        {
            get
            {
                return PluginSetupLevel.System;
            }
        }

        /// <summary>
        /// 插件的初始化方法。
        /// </summary>
        /// <param name="app">应用程序对象。</param>
        public override void Initialize(IApp app)
        {
            ObjectContainerFactory.CreateContainer().RegisterType<ITypeAdapterFactory, AutomapperTypeAdapterFactory>();           
        }
        
    }
}