using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility
{
    public static class ObjectAndByteArraySerializeExtension
    {
        /// <summary>
        /// 扩展方法：将对象序列化为二进制byte[]数组-通过系统提供的二进制流来完成序列化
        /// </summary>
        /// <param name="Obj">待序列化的对象</param>
        /// <param name="ThrowException">是否抛出异常</param>
        /// <returns>返回结果</returns>
        public static byte[] ObjectToByteArray(this object Obj, bool ThrowException)
        {
            if (Obj == null)
            {
                return null;
            }
            else
            {
                try
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    System.IO.MemoryStream stream = new System.IO.MemoryStream();
                    formatter.Serialize(stream, Obj);
                    return stream.ToArray();
                }
                catch (System.Exception ex)
                {
                    if (ThrowException)
                    {
                        throw ex;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 扩展方法：将二进制byte[]数组反序列化为对象-通过系统提供的二进制流来完成反序列化
        /// </summary>
        /// <param name="SerializedObj">待反序列化的byte[]数组</param>
        /// <param name="ThrowException">是否抛出异常</param>
        /// <returns>返回结果</returns>
        public static object ByteArrayToObject(this byte[] SerializedObj, bool ThrowException)
        {
            if (SerializedObj == null)
            {
                return null;
            }
            else
            {
                try
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    System.IO.MemoryStream stream = new System.IO.MemoryStream(SerializedObj);
                    object obj = formatter.Deserialize(stream);
                    return obj;
                }
                catch (System.Exception ex)
                {
                    if (ThrowException)
                    {
                        throw ex;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}
