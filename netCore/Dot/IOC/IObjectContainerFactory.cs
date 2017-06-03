
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.IOC
{
    /// <summary>
    /// IOC 容器工厂。
    /// </summary>
    public interface IObjectContainerFactory
    {
        /// <summary>
        /// 创建一个独立的的 IOC 容器
        /// </summary>
        /// <returns></returns>
        IObjectContainer CreateContainer();
    }
}
