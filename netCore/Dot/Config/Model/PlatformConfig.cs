using Dot.Utility.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Config.Model
{
    /// <summary>
    /// 平台配置信息
    /// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration
    /// </summary>
    [Serializable]
    public class PlatformConfig
    {
        #region 平台单一实例

        /// <summary>
        /// 静态单例-客户端配置
        /// </summary>
        private static ClientSettings clientInstance;

        /// <summary>
        /// 静态单例-服务端配置
        /// </summary>
        private static ServerSettings serverInstance;

        /// <summary>
        /// 线程安全的锁
        /// </summary>
        private static Object syncLock = new Object();


        /// <summary>
        /// 客户端配置
        /// </summary>
        public static ClientSettings ClientConfig
        {
            get
            {
                lock (syncLock)
                {
                    if (clientInstance == null)
                    {
                        clientInstance = XmlHelper.Instance.LoadConfig<ClientSettings>();

                        if (clientInstance == null)
                        {
                            clientInstance = new ClientSettings();
                            XmlHelper.Instance.SaveConfig(clientInstance);
                        } 
                    }
                    return clientInstance;
                }
            }
        }

        /// <summary>
        /// 服务器端配置
        /// </summary>
        public static ServerSettings ServerConfig
        {
            get
            {
                lock (syncLock)
                {
                    if (serverInstance == null)
                    {
                        serverInstance = XmlHelper.Instance.LoadConfig<ServerSettings>();

                        if (serverInstance == null)
                        {
                            serverInstance = new ServerSettings();
                            XmlHelper.Instance.SaveConfig(serverInstance);
                        }
                        else
                        {
                            serverInstance.ConfigFilePath = XmlHelperExtend.GetConfigPath();
                        }
                    }
                    return serverInstance;
                }
            }
        }

        public static T GetConfig<T>() where T : class, new()
        {
            return XmlHelper.Instance.LoadConfig<T>();
        }

        public static void UpdateServerConfig(ServerSettings serverConfig)
        {
            lock (syncLock)
                serverInstance = serverConfig;
        }

        public static void SaveConfig(object config)
        {
            lock (syncLock)
                XmlHelper.Instance.SaveConfig(config);
        }

        public static void UpdateClientConfig(ClientSettings clientConfig)
        {
            lock (syncLock)
                clientInstance = clientConfig;
        }

        #endregion
    }
}
