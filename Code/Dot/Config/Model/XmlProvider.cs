using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Config.Model
{
    /// <summary>
    /// 将配置文件存储在XML文件中的Provider
    /// </summary>
    public class XmlProvider : IConfigProvider
    {
        /// <summary>
        /// 保存配置文件--存储的具体位置，根据不同的Provider实现来提供
        /// </summary>
        /// <param name="configInstance"></param>
        public void SaveConfig(object configInstance)
        {
            PlatformConfig.SaveConfig(configInstance);
        }

        /// <summary>
        /// 获取配置文件的值
        /// </summary>
        /// <returns>返回键对应的值</returns>
        public object GetConfig()
        {
           return PlatformConfig.ServerConfig;
        }

        /// <summary>
        /// 获取配置文件对象信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configKeyName">键</param>
        /// <returns></returns>
        public T GetConfig<T>(string configKeyName) where T : class, new()
        {
            return PlatformConfig.GetConfig<T>();
        }
    }
}
