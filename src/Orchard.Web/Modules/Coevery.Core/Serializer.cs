using System.IO;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Coevery.Core
{
    public static class Serializer
    {
        public static string ToJson(object entity)
        {
            return new JavaScriptSerializer().Serialize(entity);
        }

        public static T FromJson<T>(string json)
        {
            return new JavaScriptSerializer().Deserialize<T>(json);
        }
    }
}
