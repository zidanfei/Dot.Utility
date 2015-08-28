
using Dot.IOC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dot.ComponentModel
{
    /// <summary>
    /// 这个类为 ClientApp、ServerApp、WebApp 等类提供了一致的基类。
    /// </summary>
    public class AppImplementationBase : IServerApp
    {

        /// <summary>
        /// 子类在合适的时间调用此方法来启动整个  应用程序。
        /// 
        /// 注意，为了支持重新启动，这个类中的所有方法都应该可以运行多次。
        /// 
        /// 但是，第二次及之后的重启，不应该使用同一个 AppImplementationBase 的实例，否则可能会造成插件重复监听同一事件。
        /// </summary>
        public void StartupApplication()
        {
            this.PrepareToStartup();

            this.InitEnvironment();

          

            //调用所有插件的 Initialize 方法。
            this.InitAllPlugins();
            this.OnAllPluginsIntialized();

            //初始化编译期元数据
            this.CompileMeta();
            this.OnMetaCompiled();

            //定义模块列表
            this.RaiseModuleOpertions();
            this.OnModuleOpertionsCompleted();

            this.OnAppMetaCompleted();

            //组合所有模块的 IOC、事件、
            this.RaiseComposeOperations();
            this.OnComposed();

            //开始运行时行为。此行代码后的所有代码都可以看作运行时行为。
            this.OnRuntimeStarting();

            //设置多国语言
            this.SetupLanguage();

            //启动主过程
            this.OnMainProcessStarting();
            this.StartMainProcess();

            //整个初始化完毕。
            this.OnStartupCompleted();
        }

        /// <summary>
        /// 此方法中会重置整个  环境。这样可以保证各插件的注册机制能再次运行。
        /// 例如，当启动过程中出现异常时，可以重新使用 Startup 来启动应用程序开始全新的启动流程。
        /// </summary>
        protected virtual void PrepareToStartup()
        {
            PluginTable.Plugins.Unlock();

         
        }

        /// <summary>
        /// 初始化应用程序的环境。
        /// 子类可在此方法中添加所需的插件、设置 <see cref="Environment.Location"/> 等。
        /// </summary>
        protected virtual void InitEnvironment()
        {

            PluginTable.Plugins.AddPlugin<DotPlugin>();
          
        }

        /// <summary>
        /// 初始化所有Plugins
        /// </summary>
        private void InitAllPlugins()
        {
            DotEnvironment.StartupAppPlugins();
           
            
        }

        /// <summary>
        /// 设置当前语言
        /// 
        /// 需要在所有 Translator 依赖注入完成后调用。
        /// </summary>
        private void SetupLanguage()
        {
          
        }

        /// <summary>
        /// 初始化必须在初始化期定义的各种元数据。
        /// </summary>
        protected virtual void CompileMeta() { }

        /// <summary>
        /// 子类重写此方法实现启动主逻辑。
        /// </summary>
        protected virtual void StartMainProcess() { }

        #region IServerApp 事件

        /// <summary>
        /// 所有实体元数据初始化完毕，包括实体元数据之间的关系。
        /// </summary>
        public event EventHandler AllPluginsIntialized;

        /// <summary>
        /// 触发 AllPluginsIntialized 事件。
        /// </summary>
        protected virtual void OnAllPluginsIntialized()
        {
            var handler = this.AllPluginsIntialized;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// 所有初始化期定义的元数据初始化完成时事件。
        /// </summary>
        public event EventHandler MetaCompiled;

        /// <summary>
        /// 触发 MetaCompiled 事件。
        /// </summary>
        protected virtual void OnMetaCompiled()
        {
            var handler = this.MetaCompiled;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// 模块的定义先于其它模型的操作。这样可以先设置好模板默认的按钮。
        /// </summary>
        public event EventHandler ModuleOperations;

        /// <summary>
        /// 触发 ModuleOperations 事件。
        /// </summary>
        protected virtual void RaiseModuleOpertions()
        {
            var handler = this.ModuleOperations;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// 模块的定义完成
        /// </summary>
        public event EventHandler ModuleOperationsCompleted;

        /// <summary>
        /// 触发 ModuleOperationsCompleted 事件。
        /// </summary>
        protected virtual void OnModuleOpertionsCompleted()
        {
            var handler = this.ModuleOperationsCompleted;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// 所有初始化工作完成
        /// </summary>
        public event EventHandler AppMetaCompleted;

        /// <summary>
        /// 触发 AppMetaCompleted 事件。
        /// </summary>
        protected virtual void OnAppMetaCompleted()
        {
            var handler = this.AppMetaCompleted;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// 组件的组合操作。
        /// 组合可以在此事件中添加自己的组合逻辑，例如 A 订阅 B 的某个事件。
        /// </summary>
        public event EventHandler ComposeOperations;

        /// <summary>
        /// 触发 ComposeOperations 事件。
        /// </summary>
        protected virtual void RaiseComposeOperations()
        {
            var handler = this.ComposeOperations;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// 所有组件组合完毕。
        /// </summary>
        public event EventHandler Composed;

        /// <summary>
        /// 触发 Composed 事件。
        /// </summary>
        protected virtual void OnComposed()
        {
            var handler = this.Composed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// 应用程序运行时行为开始。
        /// </summary>
        public event EventHandler RuntimeStarting;

        /// <summary>
        /// 触发 RuntimeStarting 事件。
        /// </summary>
        protected virtual void OnRuntimeStarting()
        {
            var handler = this.RuntimeStarting;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// 主过程开始前事件。
        /// </summary>
        public event EventHandler MainProcessStarting;

        /// <summary>
        /// 触发 MainProcessStarting 事件。
        /// </summary>
        protected virtual void OnMainProcessStarting()
        {
            var handler = this.MainProcessStarting;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// 应用程序完全退出
        /// </summary>
        public event EventHandler Exit;

        /// <summary>
        /// 触发 Exit 事件。
        /// </summary>
        protected virtual void OnExit()
        {
            var handler = this.Exit;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// AppStartup 完毕
        /// </summary>
        public event EventHandler StartupCompleted;

        /// <summary>
        /// 触发 StartupCompleted 事件。
        /// </summary>
        protected virtual void OnStartupCompleted()
        {
            var handler = this.StartupCompleted;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion
    }
}