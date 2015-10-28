using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Web
{
    public class DateTypeFormatter : MediaTypeFormatter
    {
        #region 实现基类方法

        public DateTypeFormatter()
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            base.SupportedEncodings.Add(new UTF8Encoding(false, true));
            base.SupportedEncodings.Add(new UnicodeEncoding(false, true, true));
            base.MediaTypeMappings.Add(new XmlHttpRequestHeaderMapping());
            this.SerializeAsCamelProperty = true;
        }

        public override bool CanReadType(Type type)
        {
            return true;
        }

        public override bool CanWriteType(Type type)
        {
            return true;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            return Task.Run(() => Serialize(type, value, writeStream, content));
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            return Deserialize(type, readStream, content, formatterLogger);
        }

        #endregion

        /// <summary>
        /// 是否使用舵峰式。 默认为 true。
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use camel property]; otherwise, <c>false</c>.
        /// </value>
        public bool SerializeAsCamelProperty { get; set; }

        private void Serialize(Type type, object value, Stream writeStream, HttpContent content)
        {

            SerializeByJsonSerializer(value, writeStream, content);
        }

        private async Task<object> Deserialize(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {


            var headers = (content == null) ? null : content.Headers;
            if (headers != null && headers.ContentLength == 0L)
            {
                return MediaTypeFormatter.GetDefaultValueForType(type);
            }

            object result = null;
            try
            {

                result = DeserializeByJsonSerializer(type, readStream, headers);

            }
            catch (Exception exception)
            {
                if (formatterLogger != null)
                {
                    formatterLogger.LogError(string.Empty, exception);
                }

                result = MediaTypeFormatter.GetDefaultValueForType(type);
            }

            return result;
        }

        #region Other

        /// <summary>
        /// 这个方法 Copy 自 ASP.NET MVC 源码。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="readStream"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        private object DeserializeByJsonSerializer(Type type, Stream readStream, HttpContentHeaders headers)
        {
            Encoding encoding = this.SelectCharacterEncoding(headers);
            using (var jsonTextReader = new JsonTextReader(new StreamReader(readStream, encoding)))
            //using (var jsonTextReader = new JsonTextReader(new StringReader(strContent)))
            {
                jsonTextReader.CloseInput = false;
                //jsonTextReader.MaxDepth = this.MaxDepth;
                var jsonSerializer = JsonSerializer.Create();
                //var jsonSerializer = JsonSerializer.Create(this.SerializerSettings);
                //if (formatterLogger != null)
                //{
                //    jsonSerializer.Error += delegate(object sender, ErrorEventArgs e)
                //    {
                //        Exception error = e.get_ErrorContext().get_Error();
                //        formatterLogger.LogError(e.get_ErrorContext().get_Path(), error);
                //        e.get_ErrorContext().set_Handled(true);
                //    });
                //}
                return jsonSerializer.Deserialize(jsonTextReader, type);
            }
        }

        private void SerializeByJsonSerializer(object value, Stream writeStream, HttpContent content)
        {
            Encoding encoding = this.SelectCharacterEncoding((content == null) ? null : content.Headers);

            using (var jsonTextWriter = new JsonTextWriter(new StreamWriter(writeStream, encoding)))
            {
                jsonTextWriter.CloseOutput = false;
                JsonSerializer jsonSerializer = JsonSerializer.Create();
                jsonSerializer.DateFormatString = "MM/dd/yyyy";
                //首字母小写
                //if (this.SerializeAsCamelProperty)
                //{
                //    jsonSerializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
                //}

                jsonSerializer.Serialize(jsonTextWriter, value);
                jsonTextWriter.Flush();
            }
        }

        #endregion
    }
}
