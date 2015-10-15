using Dot.Utility.Config;
using Dot.Utility.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Config
{
    internal static class XmlHelperExtend
    {
        /// <summary>
        /// 加载配置
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <returns></returns>
        internal static T LoadConfig<T>(this XmlHelper xmlHelper) where T : class, new()
        { 
            string ConfigPath = GetConfigPath();
            Type t = typeof(T);
            string path = System.IO.Path.Combine(ConfigPath, t.Name + ".xml");
            try
            {
                if (File.Exists(path))
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(path, System.Text.Encoding.Unicode))
                    {
                        var res = XmlHelper.GetSerializer<T>().Deserialize(sr) as T;
                        return res;
                    }
                }
            }
            catch { }

            return new T();
        }


        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="cfg">要保存的配置对象</param>
        internal static void SaveConfig(this XmlHelper xmlHelper, object cfg)
        {
            if (cfg == null)
                throw new ArgumentNullException();
            string ConfigPath = GetConfigPath();
            try
            {
                Type t = cfg.GetType();

                var tempDirectoryPath = ConfigPath;

                if (!Directory.Exists(ConfigPath))
                {
                    tempDirectoryPath = ConfigPath.Replace("\\Config", "");
                }

                string path = System.IO.Path.Combine(tempDirectoryPath, t.Name + ".xml");
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));

                using (System.IO.StreamWriter sr = new System.IO.StreamWriter(path, false, System.Text.Encoding.Unicode))
                {
                    XmlHelper.GetSerializer(t).Serialize(sr, cfg);
                    sr.Close();
                }
            }
            catch
            {
            }
        }
        internal static string GetConfigPath()
        {

            var serverSettingsPath = ConfigHelper.GetAppSettingOrDefault("ServerSettingsPath", "Config");
            string ConfigPath = DotEnvironment.MapAbsolutePath(serverSettingsPath);
            return ConfigPath;

        }
    }
}
