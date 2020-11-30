using System.IO;

namespace FileWatcherService
{
    class ConfigurationProvider
    {
        private readonly IConfigurationParser parser;

        private string path;

        public ConfigurationProvider(string path)
        {
            this.path = path;

            switch (Path.GetExtension(path))
            {
                case ".xml":
                    parser = new XMLParser();
                    break;
                case ".json":
                    parser = new JSONParser();
                    break;
            }
        }

        public T Parse<T>() where T: new()
        {
            return parser.Parse<T>(path);
        }
    }
}
