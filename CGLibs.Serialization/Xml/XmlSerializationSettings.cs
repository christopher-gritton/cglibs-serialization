namespace CGLibs.Serialization.Xml
{
    public enum SerializerType
    {
        DataContractSerializer = 0,
        XmlSerializer = 1
    }
    public class XmlSerializationSettings
    {

        /// <summary>
        /// Default serializer for xml deserialization
        /// </summary>
        public SerializerType DefaultSerializer { get; set; } = SerializerType.XmlSerializer;

        /// <summary>
        /// Strip xml namespaces prior to deserialization
        /// </summary>
        /// <remarks>default (true)</remarks>
        public bool StripXmlNameSpaces { get; set; } = true;
    }
}
