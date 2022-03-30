using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FuFuShop.WeChat.Utilities
{
    /// <summary>XML 工具类</summary>
    public static class XmlUtility
    {
        /// <summary>反序列化</summary>
        /// <param name="xml">XML字符串</param>
        /// <returns></returns>
        public static object Deserialize<T>(string xml)
        {
            try
            {
                using (StringReader stringReader = new StringReader(xml))
                    return new XmlSerializer(typeof(T)).Deserialize(stringReader);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>反序列化</summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static object Deserialize<T>(Stream stream) => new XmlSerializer(typeof(T)).Deserialize(stream);

        /// <summary>
        /// 序列化
        /// 说明：此方法序列化复杂类，如果没有声明XmlInclude等特性，可能会引发“使用 XmlInclude 或 SoapInclude 特性静态指定非已知的类型。”的错误。
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string Serializer<T>(T obj)
        {
            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            try
            {
                xmlSerializer.Serialize(memoryStream, obj);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            memoryStream.Position = 0L;
            StreamReader streamReader = new StreamReader(memoryStream);
            string end = streamReader.ReadToEnd();
            streamReader.Dispose();
            memoryStream.Dispose();
            return end;
        }

        /// <summary>序列化将流转成XML字符串</summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static XDocument Convert(Stream stream)
        {
            if (stream.CanSeek)
                stream.Seek(0L, SeekOrigin.Begin);
            using (XmlReader reader = XmlReader.Create(stream))
                return XDocument.Load(reader);
        }

        /// <summary>序列化将流转成XML字符串</summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ConvertToString(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string sHtml = reader.ReadToEnd();
            return sHtml;
        }

    }
}
