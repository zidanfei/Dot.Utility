using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility
{
    /// <summary>
    /// 基础的数据类型-转换 
    /// </summary>
    public class DataConvert
    {
        /// <summary>
        /// 16进制->整型
        /// </summary>
        /// <param name="HexChar"></param>
        /// <returns></returns>
        private static int HexCharToInt(char HexChar)
        {
            if (HexChar == '0')
            {
                return 0;
            }
            else if (HexChar == '1')
            {
                return 1;
            }
            else if (HexChar == '2')
            {
                return 2;
            }
            else if (HexChar == '3')
            {
                return 3;
            }
            else if (HexChar == '4')
            {
                return 4;
            }
            else if (HexChar == '5')
            {
                return 5;
            }
            else if (HexChar == '6')
            {
                return 6;
            }
            else if (HexChar == '7')
            {
                return 7;
            }
            else if (HexChar == '8')
            {
                return 8;
            }
            else if (HexChar == '9')
            {
                return 9;
            }
            else if (HexChar == 'a' || HexChar == 'A')
            {
                return 10;
            }
            else if (HexChar == 'b' || HexChar == 'B')
            {
                return 11;
            }
            else if (HexChar == 'c' || HexChar == 'C')
            {
                return 12;
            }
            else if (HexChar == 'd' || HexChar == 'D')
            {
                return 13;
            }
            else if (HexChar == 'e' || HexChar == 'E')
            {
                return 14;
            }
            else if (HexChar == 'f' || HexChar == 'F')
            {
                return 15;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 16进制->长整型
        /// </summary>
        /// <param name="HexChar"></param>
        /// <returns></returns>
        public static long HexStringToLong(string HexString)
        {
            if (HexString == null || HexString.Length == 0)
            {
                return 0;
            }
            else
            {
                long v = 0;
                for (int count = 0; count < HexString.Length; count++)
                {
                    char c = HexString[count];
                    v = v * 16 + HexCharToInt(c);
                }
                return v;
            }
        }

        /// <summary>
        /// 将源类型转换为目标类型
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="ConversionType"></param>
        /// <returns></returns>
        public static object Convert(object Source, System.Type ConversionType)
        {
            return Convert(Source, ConversionType, true);
        }

        /// <summary>
        /// 将源类型转换为目标类型
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="ConversionType"></param>
        /// <returns></returns>
        public static object Convert(object Source, System.Type ConversionType, bool ThrowException)
        {
            if (Source != null && Source.GetType() == ConversionType)
            {
                return Source;
            }
            else if (Source == null)
            {
                return GetDefaultValue(ConversionType);
            }
            try
            {
                if (ConversionType == typeof(System.DateTime))
                {
                    return System.DateTime.Parse(Source.ToString());
                }
                else if (ConversionType == typeof(System.TimeSpan))
                {
                    return System.TimeSpan.Parse(Source.ToString());
                }
                else if (ConversionType.IsArray && (Source is string && (string)Source == ""))
                {
                    return null;
                }
                else
                {
                    return System.Convert.ChangeType(Source, ConversionType);
                }
            }
            catch (Exception ex)
            {
                if (ThrowException)
                {
                    throw ex;
                }
                else
                {
                    return GetDefaultValue(ConversionType);
                }
            }
        }

        /// <summary>
        /// 获取类型的默认值
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static object GetDefaultValue(System.Type Type)
        {
            if (Type == typeof(string))
            {
                return null;
            }
            else if (Type == typeof(System.DateTime))
            {
                return System.DateTime.MinValue;
            }
            else if (Type == typeof(System.Boolean))
            {
                return false;
            }
            else if (Type == typeof(System.Double))
            {
                return 0.0;
            }
            else if (Type == typeof(System.Int32))
            {
                return 0;
            }
            else if (Type == typeof(System.Int64))
            {
                return 0;
            }
            else if (Type == typeof(System.TimeSpan))
            {
                return new System.TimeSpan(0);
            }
            else if (Type == typeof(System.Data.DataTable))
            {
                return null;
            }
            else if (Type == typeof(System.Data.SqlTypes.SqlDateTime))
            {
                return System.Data.SqlTypes.SqlDateTime.MinValue;
            }
            else
            {
                return null;
            }
        }

        /// <summary>  
        /// 利用反射和泛型  
        /// </summary>  
        /// <param name="dt"></param>  
        /// <returns></returns>  
        public static IList<T> ToList<T>(DataTable dt) where T : class,new()
        {

            // 定义集合  
            IList<T> ts = new List<T>();

            // 获得此模型的类型  
            Type type = typeof(T);
            //定义一个临时变量  
            string tempName = string.Empty;
            //遍历DataTable中所有的数据行  
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性  
                PropertyInfo[] propertys = t.GetType().GetProperties();
                //遍历该对象的所有属性  
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;//将属性名称赋值给临时变量  
                    //检查DataTable是否包含此列（列名==对象的属性名）    
                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter  
                        if (!pi.CanWrite)
                            continue;//该属性不可写，直接跳出  
                        //取值  
                        object value = dr[tempName];
                        //如果非空，则赋给对象的属性  
                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);
                    }
                }
                //对象添加到泛型集合中  
                ts.Add(t);
            }

            return ts;
        }


        /// <summary>  
        /// 利用反射和泛型  
        /// </summary>  
        /// <param name="dt"></param>  
        /// <returns></returns>  
        public static DataTable ToDataTable<T>(IList<T> list) where T : class,new()
        {

            // 定义集合  
            DataTable dt = new DataTable();

            // 获得此模型的类型  
            Type type = typeof(T);
            PropertyInfo[] propertys = type.GetProperties();
            foreach (var p in propertys)
            {
                DataColumn dc = new DataColumn();
                dc.ColumnName = p.Name;
                dt.Columns.Add(dc);
            }
            foreach (var item in list)
            {
                var newrow = dt.NewRow();
                foreach (DataColumn column in dt.Columns)
                {
                    newrow[column.ColumnName] = item.GetType().GetProperty(column.ColumnName).GetValue(item, null);
                }
                dt.Rows.Add(newrow);
            }
            return dt;
        }


        public static string ToShortGuid(Guid Guid)
        {
            byte[] array = Guid.ToByteArray();
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            long l = 0;
            string s;
            for (int count = 0; count < 15; count = count + 5)
            {
                l = array[count] * 256 * 16 + array[count + 1] * 16 + (array[count + 2] / 16);
                s = ToBase32String(l, 4);
                builder.Append(s);
                l = (array[count + 2] % 16) * 256 * 256 + array[count + 3] * 256 + array[count + 4];
                s = ToBase32String(l, 4);
                builder.Append(s);
            }
            s = ToBase32String((long)array[15], 2);
            builder.Append(s);
            return builder.ToString();
        }

        private static string ToBase32String(long Value, int Length)
        {
            string s = ToBase32String(Value);
            int zeroCount = Length - s.Length;
            for (int count = 0; count < zeroCount; count++)
            {
                s = s.Insert(0, "0");
            }
            return s;
        }

        public static string ToBase32String(byte Value)
        {
            return ToBase32String((long)Value);
        }

        public static string ToBase32String(long Value)
        {
            if (Value < 0)
            {
                return "-" + ToUnsignedBase32String(0 - Value);
            }
            else if (Value == 0)
            {
                return "0";
            }
            else
            {
                return ToUnsignedBase32String(Value);
            }
        }

        private static string ToUnsignedBase32String(long Value)
        {
            if (Value < 0)
            {
                throw new NotImplementedException();
            }

            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            long last = Value;
            while (last > 0)
            {
                int i = (int)(last % 32);
                char c = ToBase32Char(i);
                builder.Insert(0, c);

                last = (long)(last / 32);
            }

            if (builder.Length == 0)
            {
                return "0";
            }
            else
            {
                return builder.ToString();
            }
        }

        private static char ToBase32Char(int Value)
        {
            if (Value < 10)
            {
                char baseChar = '0';
                int baseInt = (int)baseChar;
                int charValue = Value + baseInt;
                char c = (char)charValue;
                return c;
            }
            else if (Value < 32)
            {
                char baseChar = 'a';
                int baseInt = (int)baseChar;
                int charValue = Value + baseInt - 10;
                char c = (char)charValue;
                return c;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
