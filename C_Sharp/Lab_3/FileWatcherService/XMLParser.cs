using System;
using System.Xml.Schema;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FileWatcherService
{
    class XMLParser : IConfigurationParser
    {
        public T Parse<T>(string path) where T : new()
        {
            T obj = new T();

            try
            {
                Validate(path);

                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (var fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    obj = (T)serializer.Deserialize(fs);
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return obj;
        }

        private void Validate(string path)
        {
            XmlSchemaSet schemas = new XmlSchemaSet();
            var validationPath = Path.Combine(Directory.GetCurrentDirectory(), "config.xsd");
            schemas.Add(null, validationPath);

            XDocument document = XDocument.Load(path);

            document.Validate(schemas, (sender, validationEventArgs) =>
            {
                throw validationEventArgs.Exception;
            });
        }


    }
}
