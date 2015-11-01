using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility
{
    /// <summary>
    /// 枚举辅助类
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// 获取枚举名称
        /// </summary>
        /// <param name="t">枚举类型</param>
        /// <param name="v">枚举值</param>
        /// <returns></returns>
        private static string GetName(System.Type t, object v)
        {
            try
            {
                return Enum.GetName(t, v);
            }
            catch
            {
                return "UNKNOWN";
            }
        }

        /// <summary>
        /// 获取指定枚举值的描述信息
        /// </summary>
        /// <param name="t">枚举类型</param>
        /// <param name="v">枚举值</param>
        /// <returns></returns>
        public static string GetDescription(System.Type t, object v)
        {
            try
            {
                FieldInfo oFieldInfo = t.GetField(GetName(t, v));
                DescriptionAttribute[] attributes = (DescriptionAttribute[])oFieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return (attributes.Length > 0) ? attributes[0].Description : GetName(t, v);
            }
            catch
            {
                return "未知";
            }
        }

        /// <summary>
        /// 获取指定枚举值的描述信息
        /// </summary>
        /// <param name="t">枚举类型</param>
        /// <param name="v">枚举值</param>
        /// <returns></returns>
        public static string GetDisplayName<T>(T v) where T :class,new()
        {
            try
            {
                FieldInfo oFieldInfo = typeof(T).GetField(GetName(typeof(T), v));
                DisplayNameAttribute[] attributes = (DisplayNameAttribute[])oFieldInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false);
                return (attributes.Length > 0) ? attributes[0].DisplayName : GetName(typeof(T), v);
            }
            catch
            {
                return "未知";
            }
        }

        /// <summary>
        /// 获取枚举类型中的所有枚举项
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static IList<EnumItem> GetEnumItems(System.Type t)
        {
            try
            {
                List<EnumItem> returnlist = new List<EnumItem>();
                Array getEnumLists = Enum.GetValues(t);
                if (getEnumLists != null)
                {
                    if (getEnumLists.Length > 0)
                    {
                        for (int i = 0; i < getEnumLists.Length; i++)
                        {
                            EnumItem entity = new EnumItem();
                            string getEnumName = GetDescription(t, getEnumLists.GetValue(i));
                            entity.Name = getEnumName;
                            string getEnumValue = Convert.ToInt32(getEnumLists.GetValue(i)).ToString();
                            entity.Value = getEnumValue;
                            returnlist.Add(entity);
                        }
                    }
                }
                return returnlist;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 根据枚举名称获取枚举值
        /// </summary>
        /// <param name="t">枚举类型</param>
        /// <param name="EnumName">枚举名称</param>
        /// <returns></returns>
        public static string GetEnumValue(System.Type t, string EnumName)
        {
            IList<EnumItem> list = GetEnumItems(t);
            string ReturnValue = "";
            if (list != null)
            {
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].Name.Equals(EnumName, StringComparison.OrdinalIgnoreCase))
                        {
                            ReturnValue = list[i].Value;
                        }

                    }
                }
            }
            return ReturnValue;
        }

        /// <summary>
        /// 枚举项定义
        /// </summary>
        public class EnumItem
        {
            /// <summary>
            /// 枚举项名称
            /// </summary>
            public string Name
            {
                get;
                set;
            }

            /// <summary>
            /// 枚举值
            /// </summary>
            public string Value
            {
                get;
                set;
            }
        }
    }
}
