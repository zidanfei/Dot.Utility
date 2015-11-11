using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization.Formatters.Binary;

namespace Dot.Utility
{
    public class JsonHelper
    {
        public static T ConvertStrToObject<T>(string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
                return default(T);
            JavaScriptSerializer sel = new JavaScriptSerializer();
            var obj = sel.Deserialize<T>(jsonStr);
            return obj;
        }
        public static string ConvertObjectToStr<T>(T obj)
        {
            JavaScriptSerializer sel = new JavaScriptSerializer();
            var str = sel.Serialize(obj);
            return str;
        }

        public static byte[] Serializable<T>(T obj)
        {

            MemoryStream stream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, obj);
            var ss = stream.GetBuffer();
            return ss;
        }

        public static T Deserializable<T>(byte[] inputBinary)
            where T : class
        {
            MemoryStream stream = new MemoryStream();

            BinaryFormatter binaryFormatter = new BinaryFormatter();


            stream.Write(inputBinary, 0, inputBinary.Length);
            stream.Position = 0;
            return binaryFormatter.Deserialize(stream) as T;

        }
    }
}

