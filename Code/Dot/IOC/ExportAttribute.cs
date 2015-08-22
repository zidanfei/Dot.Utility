
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.IOC
{
    /// <summary>
    /// 在某个类型上指定的标记，说明该类型将会注册到 IOC 默认容器中。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class ExportAttribute : Attribute
    {
        /// <summary>
        /// 构造器。
        /// </summary>
        /// <param name="provideFor">为这个类型提供实例。</param>
        public ExportAttribute(Type provideFor)
        {
            if (provideFor == null) throw new ArgumentNullException("provideFor");
            this.ProvideFor = provideFor;

            this.RegisterWay = RegisterWay.Type;
        }

        public ExportAttribute(Type provideFor, string key)
            : this(provideFor)
        {
            Key = key;
        }
        /// <summary>
        /// 为这个类型提供实例。
        /// </summary>
        public Type ProvideFor { get; private set; }

        /// <summary>
        /// 注册到 IOC 容器中的方式。默认值为 <see cref=".ComponentModel.RegisterWay.Type"/>。
        /// </summary>
        public RegisterWay RegisterWay { get; set; }

        /// <summary>
        /// 注册时使用的键。
        /// </summary>
        public string Key { get; set; }
    }

    /// <summary>
    /// 注册到 IOC 容器中的方式。
    /// </summary>
    public enum RegisterWay
    {
        /// <summary>
        /// 以单一实例的方式注册。
        /// </summary>
        Instance,
        /// <summary>
        /// 以类型的方式注册。
        /// </summary>
        Type
    }
}
