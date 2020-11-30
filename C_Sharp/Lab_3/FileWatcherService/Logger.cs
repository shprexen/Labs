using System;
using System.IO;
using System.Threading;

namespace FileWatcherService
{
    class Logger
    {
        FileSystemWatcher sourceWatcher;
        FileSystemWatcher targetWatcher;

        ConfigurationOptions options;

        object obj = new object();

        bool enabled = true;

        private string sourceDirPath;

        private string targetDirPath;

        public Logger(ConfigurationOptions options)
        {
            this.options = options;

            sourceDirPath = options.SourcePath;
            targetDirPath = options.TargetPath;

            sourceWatcher = new FileSystemWatcher(sourceDirPath);
            targetWatcher = new FileSystemWatcher(targetDirPath);

            targetWatcher.Changed += WatcherChanged;
            targetWatcher.Deleted += WatcherChanged;
            sourceWatcher.Created += WatcherCreated;
            targetWatcher.Renamed += WatcherRenamed;
        }

        public void Start()
        {
            sourceWatcher.EnableRaisingEvents = true;
            targetWatcher.EnableRaisingEvents = true;

            while(enabled)
            {
                Thread.Sleep(1000);
            }
        }

        public void Stop()
        {
            sourceWatcher.EnableRaisingEvents = false;
            targetWatcher.EnableRaisingEvents = false;

            enabled = false;
        }

        private void RecordEntry(string fileEvent, string filePath)
        {
            string LogFileLocation = String.Concat(Directory.GetCurrentDirectory(), @"\log.txt");

            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter(LogFileLocation,true))
                {
                    writer.WriteLine(String.Format("{0} файл {1} был {2}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent));
                    writer.Flush();
                }
            }
        }

        private void WatcherChanged(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "";
            string filePath = e.FullPath;

            switch(e.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    fileEvent = "изменён";
                    break;
                case WatcherChangeTypes.Deleted:
                    fileEvent = "удалён";
                    break;
            }

            RecordEntry(fileEvent, filePath);
        }

        private void WatcherRenamed(object sender, RenamedEventArgs e)
        {
            if (Path.GetFileNameWithoutExtension(e.FullPath).Contains("DEARCHIVE"))
            { 
                Archivator.Dearchive(e.FullPath, targetDirPath);
                File.Delete(e.FullPath);
            }

            string fileEvent = "переименован в " + e.FullPath;
            string filePath = e.OldFullPath;

            RecordEntry(fileEvent, filePath);
        }

        private void WatcherCreated(object sender, FileSystemEventArgs e)
        {
            if(Directory.Exists(options.TargetPath))
            {
                targetDirPath = options.TargetPath;
            }

            if (options.Archive == true)
            {
                Archivator.Archive(e.FullPath, targetDirPath, options.Encrypt);
            }
        }
    }
}
