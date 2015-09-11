using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Adapter
{
    /// <summary>
    /// Base contract for adapter factory
    /// </summary>
    public interface ITypeAdapterFactory
    {
        /// <summary>
        /// 创建类型映射适配器
        /// </summary>
        /// <returns>The created ITypeAdapter</returns>
        ITypeAdapter Create();
    }
}
