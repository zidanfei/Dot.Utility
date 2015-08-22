
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dot.ComponentModel
{
    /// <summary>
    /// 使用代码添加的插件程序集。
    /// 只是以插件机制加载，但本质上是必需的，并非插件。
    /// </summary>
    public class PluginTable : Collection<Assembly>
    {
        private static PluginTable _assemblys = new PluginTable();

        /// <summary>
        /// 已经添加的实体插件程序集。
        /// </summary>
        public static PluginTable Assemblys
        {
            get { return _assemblys; }
        }

       

        /// <summary>
        /// 直接添加一个插件对应的程序集引用。
        /// </summary>
        /// <typeparam name="TPlugin"></typeparam>
        public void AddPlugin<TPlugin>()
            where TPlugin : IPlugin
        {
            this.Add(typeof(TPlugin).Assembly);
        }

        private bool _locked;

        protected override void ClearItems()
        {
            this.EnsureWritable();

            base.ClearItems();
        }

        protected override void RemoveItem(int index)
        {
            this.EnsureWritable();

            base.RemoveItem(index);
        }

        protected override void InsertItem(int index, Assembly item)
        {
            if (!this.Contains(item))
            {
                this.EnsureWritable();

                base.InsertItem(index, item);
            }
        }

        protected override void SetItem(int index, Assembly item)
        {
            if (!this.Contains(item))
            {
                this.EnsureWritable();

                base.SetItem(index, item);
            }
        }

        internal void Lock()
        {
            this._locked = true;
        }

        internal void Unlock()
        {
            this._locked = false;
        }

        private void EnsureWritable()
        {
            if (this._locked) throw new InvalidOperationException("集合不可再变更。");
        }
    }
}