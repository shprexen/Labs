using System;
using System.IO;
using System.Text.Json;

namespace FileWatcherService
{
    class JSONParser : IConfigurationParser
    {
        public T Parse<T>(string path) where T : new()
        {
            T obj = new T();

            try
            {
                string jsonString = File.ReadAllText(path);
                obj = JsonSerializer.Deserialize<T>(jsonString);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return obj;
        }
    }
}
