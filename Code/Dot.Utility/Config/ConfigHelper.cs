using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dot.Utility.Config
{
    public class ConfigHelper
    {
        /// <summary>
        /// 获取配置文件中的AppSettings的指定键的值，并转换为指定类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetAppSettingOrDefault<T>(string key, T defaultValue = default(T))
            where T : struct
        {
            var value = GetAppSettingOrDefault(key);
            if (!string.IsNullOrWhiteSpace(value))
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter.CanConvertFrom(typeof(string)))
                {
                    try
                    {
                        return (T)converter.ConvertFromString(value);
                    }
                    catch { }
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// 获取配置文件中的AppSettings的指定键的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetAppSettingOrDefault(string key, string defaultValue = "")
        {
            return ConfigurationManager.AppSettings[key] ?? defaultValue;
        }

        /// <summary>
        /// 获取配置文件中的ConnectionString的指定键的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetConnectionStringOrDefault(string key, string defaultValue = "")
        {
            return ConfigurationManager.ConnectionStrings[key].ConnectionString ?? defaultValue;
        }

    }
}
