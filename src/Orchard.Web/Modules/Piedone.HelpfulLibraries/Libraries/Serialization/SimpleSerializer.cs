using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Serialization
{
    [OrchardFeature("Piedone.HelpfulLibraries.Serialization")]
    public class SimpleSerializer : ISimpleSerializer
    {
        public string XmlSerialize<T>(T obj)
        {
            string serialization;

            using (var sw = new StringWriter())
            {
                using (var writer = new XmlTextWriter(sw))
                {
                    var serializer = new DataContractSerializer(obj.GetType());
                    serializer.WriteObject(writer, obj);
                    writer.Flush();
                    serialization = sw.ToString();
                }
            }

            return serialization;
        }

        public T XmlDeserialize<T>(string serialization)
        {
            var serializer = new DataContractSerializer(typeof(T));
            var doc = new XmlDocument();
            doc.LoadXml(serialization);
            var reader = new XmlNodeReader(doc.DocumentElement);
            return (T)serializer.ReadObject(reader);
        }


        public string JsonSerialize<T>(T obj)
        {
            using (var stream = new MemoryStream())
            {
                using (var sr = new StreamReader(stream))
                {
                    var serializer = new DataContractJsonSerializer(obj.GetType());
                    serializer.WriteObject(stream, obj);
                    stream.Position = 0;

                    return sr.ReadToEnd();
                }
            }
        }

        public T JsonDeserialize<T>(string serialization)
        {
            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(serialization)))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                stream.Position = 0;
                return (T)serializer.ReadObject(stream);
            }
        }

        public string Base64Serialize<T>(T obj)
        {
            // Taken entirely from http://snipplr.com/view/8013/, refactored
            var ms = new MemoryStream();
            var bf = new BinaryFormatter();
            bf.Serialize(ms, obj);
            var bytes = ms.GetBuffer();
            return bytes.Length + ":" + Convert.ToBase64String(bytes, 0, bytes.Length, Base64FormattingOptions.None);
        }

        public T Base64Deserialize<T>(string serialization)
        {
            // Taken entirely from http://snipplr.com/view/8014/, refactored

            // We need to know the exact length of the string - Base64 can sometimes pad us by a byte or two
            int lengthDelimiterPosition = serialization.IndexOf(':');
            int length = Int32.Parse(serialization.Substring(0, lengthDelimiterPosition));

            // Extract data from the base 64 string!
            var bytes = Convert.FromBase64String(serialization.Substring(lengthDelimiterPosition + 1));
            var ms = new MemoryStream(bytes, 0, length);
            var bf = new BinaryFormatter();

            return (T)bf.Deserialize(ms);
        }
    }
}
