namespace FileWatcherService
{
    interface IConfigurationParser
    {
        T Parse<T>(string path) where T : new();
    }
}
