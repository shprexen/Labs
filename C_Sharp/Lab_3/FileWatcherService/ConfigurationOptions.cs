namespace FileWatcherService
{
    class ConfigurationOptions
    {
        public string SourcePath { get; set; }

        public string TargetPath { get; set; }

        public bool Archive { get; set; } = false;

        public bool Encrypt { get; set; } = false;
    }
}
