
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace CGLibs.Serialization
{
    /// <summary>
    /// Class Convertor.
    /// </summary>
    public class Convertor 
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="Convertor" /> class from being created.
        /// </summary>
        private Convertor()
        {
           
        }

        public static Xml.XmlSerializationSettings Settings { get; private set; } = new Xml.XmlSerializationSettings();
        public static DataContractJsonSerializerSettings JsonSettings { get; private set; }

        private DataContractJsonSerializerSettings _jsonSettings
        {
            get
            {
                if (JsonSettings == null)
                {
                    JsonSettings = new DataContractJsonSerializerSettings();
                }
                return JsonSettings;
            }
            set
            {
                JsonSettings = value;
            }
        }

        private DataContractJsonSerializerSettings JsonSettingsWithOptions(Action<DataContractJsonSerializerSettings> options)
        {
            DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
            if (JsonSettings != null)
            {
                settings = JsonSettings;
            }
            if (options != null)
            {
                options(settings);                
            }

            return settings;
        }


        /// <summary>
        /// Converts this instance.
        /// </summary>
        /// <returns>Convertor.</returns>
        public static Convertor Convert()
        {
            return new Convertor();
        }

        public static Convertor Translate
        {
            get
            {
                return new Convertor();
            }
        }

        #region "XML Serialization"

        /// <summary>
        /// To the XML file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="filename">The filename.</param>
        public void ToXmlFile<T>(T source, string filename, bool alwayscreatedirectory = false) where T : class
        {
            if (alwayscreatedirectory == true)
            {
                try
                {
                    var fileinfo = new FileInfo(filename);
                    if (!fileinfo.Directory.Exists) fileinfo.Directory.Create();
                }
                catch { }
            }

            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                string xml = ToXml(source);
                sw.WriteLine(xml);
            }
        }

        /// <summary>
        /// Froms the XML file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename">The filename.</param>
        /// <returns>T.</returns>
        public T FromXmlFile<T>(string filename) where T : class
        {
            T response = default(T);
            using (StreamReader sr = new StreamReader(filename))
            {
                string xml = sr.ReadToEnd();
                response = FromXml<T>(xml);
            }

            return response;
        }

        /// <summary>
        /// To the XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public string ToXml<T>(T source) where T : class
        {
            string finalxml = string.Empty;

            try
            {
                //memorystream holds data
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    //settings to format xml
                    System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true };
                    //create emtpty namespaces
                    System.Xml.Serialization.XmlSerializerNamespaces ns = new System.Xml.Serialization.XmlSerializerNamespaces();
                    ns.Add(string.Empty, string.Empty);
                    //create writer
                    System.Xml.XmlWriter sw = System.Xml.XmlWriter.Create(ms, settings);
                    //create serializer
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    //serialize
                    xs.Serialize(sw, source, ns);
                    //reset memorystream to start
                    ms.Position = 0;
                    //read string data out of memory stream
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(ms))
                    {
                        finalxml = sr.ReadToEnd();
                    }

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.Message + "\n" + ex.StackTrace);
                throw new NotSupportedException(ex.Message);
            }

            return finalxml;
        }

        /// <summary>
        /// Froms the XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>T.</returns>
        public T FromXml<T>(string source) where T : class
        {
            T retitm = default(T);

            if (Settings.StripXmlNameSpaces) source = source.StripXmlNameSpacesFromRawXml();
         
            //string reader to read xml
            using (System.IO.StringReader sr = new System.IO.StringReader(source))
            {
                //xml text reader to read string reader stream
                using (System.Xml.XmlTextReader xr = new System.Xml.XmlTextReader(sr))
                {
                    if (Settings.DefaultSerializer == Xml.SerializerType.XmlSerializer)
                    {
                        //deserialize information from xml text reader
                        System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                        retitm = (T)xs.Deserialize(xr);
                    }
                    else
                    {
                        //deserialize information from xml text reader
                        DataContractSerializer dcs = new DataContractSerializer(typeof(T));
                        retitm = (T)dcs.ReadObject(xr);
                    }
                }
            }

            return retitm;
        }

        #endregion

        #region "Json serialization"

        /// <summary>
        /// To the json file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="filename">The filename.</param>
        public void ToJsonFile<T>(T source, string filename, bool alwayscreatedirectory = false) where T : class
        {
            if (alwayscreatedirectory == true)
            {
                try
                {
                    var fileinfo = new FileInfo(filename);
                    if (!fileinfo.Directory.Exists) fileinfo.Directory.Create();
                }
                catch { }
            }

            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                string json = ToJson(source);
                sw.WriteLine(json);
            }
        }

        /// <summary>
        /// Froms the json file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename">The filename.</param>
        /// <returns>T.</returns>
        public T FromJsonFile<T>(string filename) where T : class
        {
            T response = default(T);
            using (StreamReader sr = new StreamReader(filename))
            {
                string json = sr.ReadToEnd();
                response = FromJson<T>(json);
            }

            return response;
        }

        /// <summary>
        /// To the json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>System.String.</returns>
        public string ToJson<T>(T source, Action<DataContractJsonSerializerSettings> options = null) where T : class
        {
            string serialized = string.Empty;

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T), JsonSettingsWithOptions(options));
                json.WriteObject(ms, source);

                //reset stream for reading
                ms.Position = 0;

                using (System.IO.StreamReader sr = new System.IO.StreamReader(ms))
                {
                    serialized = sr.ReadToEnd();
                }
            }

            return serialized;
        }

        public string ToJsonDateFormatted<T>(T source, Action<DataContractJsonSerializerSettings> options = null) where T : class
        {
            string serialized = string.Empty;

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T), JsonSettingsWithOptions((options == null ? (t) => 
                {
                    t.DateTimeFormat = new DateTimeFormat("yyyy-MM-dd'T'HH:mm:ssK");
                } : options)));

                json.WriteObject(ms, source);

                //reset stream for reading
                ms.Position = 0;

                using (System.IO.StreamReader sr = new System.IO.StreamReader(ms))
                {
                    serialized = sr.ReadToEnd();
                }
            }

            return serialized;
        }

        /// <summary>
        /// Froms the json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>T.</returns>
        public T FromJson<T>(string source, Action<DataContractJsonSerializerSettings> options = null) where T : class
        {
            T deserialized;

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(ms))
                {
                    //write the serialized data to stream
                    sw.Write(source);
                    //flush to the memorystream
                    sw.Flush();
                    //reset stream for reading
                    ms.Position = 0;

                    DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T), JsonSettingsWithOptions(options));
                    deserialized = (T)json.ReadObject(ms);
                }
            }

            return deserialized;
        }

        public T FromJsonDateFormatted<T>(string source, Action<DataContractJsonSerializerSettings> options = null) where T : class
        {
            T deserialized;

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(ms))
                {
                    //write the serialized data to stream
                    sw.Write(source);
                    //flush to the memorystream
                    sw.Flush();
                    //reset stream for reading
                    ms.Position = 0;

                    DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T), JsonSettingsWithOptions((options == null ? (t) =>
                    {
                        t.DateTimeFormat = new DateTimeFormat("yyyy-MM-dd'T'HH:mm:ssK");
                    }
                    : options)));

                    deserialized = (T)json.ReadObject(ms);
                }
            }

            return deserialized;
        }

        #endregion

    }



}
