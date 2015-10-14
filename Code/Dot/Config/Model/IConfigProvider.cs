using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Config.Model
{
    /// <summary>
    /// 框架配置文件操作接口定义
    /// </summary>
    public interface IConfigProvider
    {
        /// <summary>
        /// 保存配置文件--存储的具体位置，根据不同的Provider实现来提供
        /// </summary>
        /// <param name="configInstance"></param>
        void SaveConfig(object configInstance);

        /// <summary>
        /// 根据指定的类型，返回类型的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configKeyName">配置文件名称</param>
        /// <returns></returns>
        T GetConfig<T>(string configKeyName) where T : class, new();
    }
}
