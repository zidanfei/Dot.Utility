
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Dot.IOC
{
    /// <summary>
    /// UnityContainer 创建完成事件。
    /// </summary>
    public class UnityContainerCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnityContainerCreatedEventArgs"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public UnityContainerCreatedEventArgs(UnityContainer container)
        {
            this.Container = container;
        }

        /// <summary>
        /// 被创建的 UnityContainer。
        /// </summary>
        public UnityContainer Container { get; private set; }
    }
}