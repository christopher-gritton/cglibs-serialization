using System.Runtime.Serialization.Json;

namespace CGLibs.Serialization.Json
{

    /// <summary>
    /// 
    /// </summary>
    public class JsonSerializer
    {

        /// <summary>
        /// Serializes the specified contract.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contract">The contract.</param>
        /// <returns></returns>
        public static string Serialize<T>(T contract)
        {

            //TODO: check for DataContract attribute

            string serialized = string.Empty;

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T));
                json.WriteObject(ms, contract);

                //reset stream for reading
                ms.Position = 0;

                using (System.IO.StreamReader sr = new System.IO.StreamReader(ms))
                {
                   serialized  = sr.ReadToEnd();
                }
            }

            return serialized;
        }

        /// <summary>
        /// Deserializes the specified Json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serialized">The serialized.</param>
        /// <returns></returns>
        public static T Deserialize<T>(string serialized)
        {
            T deserialized;

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(ms))
                {
                    //write the serialized data to stream
                    sw.Write(serialized);
                    //flush to the memorystream
                    sw.Flush();
                    //reset stream for reading
                    ms.Position = 0;

                    DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T));
                    deserialized = (T)json.ReadObject(ms);
                }
            }

            return deserialized;
        }
        
    }
}
