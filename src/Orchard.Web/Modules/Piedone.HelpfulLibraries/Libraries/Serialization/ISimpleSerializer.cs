using Orchard;

namespace Piedone.HelpfulLibraries.Serialization
{
    /// <summary>
    /// Easy object serialization and deserialization
    /// </summary>
    public interface ISimpleSerializer : ISingletonDependency
    {
        /// <summary>
        /// Serializes an object to an XML string. Note that since the method uses DataContractSerializer under the hood classes
        /// and their members should be decorated with the appropriate attributes, like [DataContract] (for classes) and 
        /// [DataMember] (for properties).
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize</typeparam>
        /// <param name="obj">The object to serialize</param>
        /// <returns>The XML string serialization of the object</returns>
        string XmlSerialize<T>(T obj);

        /// <summary>
        /// Deserializes an object previously serialized with XmlSerialize()
        /// </summary>
        /// <typeparam name="T">The type of the object that was serialized</typeparam>
        /// <param name="serialization">XML string serialization of the object</param>
        /// <returns>The deserialized object</returns>
        T XmlDeserialize<T>(string serialization);

        /// <summary>
        /// Serializes an object to a JSON string. Note that since the method uses DataContractJsonSerializer under the hood classes
        /// and their members should be decorated with the appropriate attributes, like [DataContract] (for classes) and 
        /// [DataMember] (for properties).
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize</typeparam>
        /// <param name="obj">The object to serialize</param>
        /// <returns>The JSON string serialization of the object</returns>
        string JsonSerialize<T>(T obj);

        /// <summary>
        /// Deserializes an object previously serialized with JsonSerialize()
        /// </summary>
        /// <typeparam name="T">The type of the object that was serialized</typeparam>
        /// <param name="serialization">JSON string serialization of the object</param>
        /// <returns>The deserialized object</returns>
        T JsonDeserialize<T>(string serialization);

        /// <summary>
        /// Serializes an object to a Base64 string
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize</typeparam>
        /// <param name="obj">The object to serialize</param>
        /// <returns>The Base64 string serialization of the object</returns>
        string Base64Serialize<T>(T obj);

        /// <summary>
        /// Deserializes an object previously serialized with Base64Serialize()
        /// </summary>
        /// <typeparam name="T">The type of the object that was serialized</typeparam>
        /// <param name="serialization">Base64 string serialization of the object</param>
        /// <returns>The deserialized object</returns>
        T Base64Deserialize<T>(string serialization);
    }
}
