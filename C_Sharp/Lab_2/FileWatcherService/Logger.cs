using System;
using System.IO;
using System.Threading;

namespace FileWatcherService
{
    class Logger
    {
        FileSystemWatcher sourceWatcher;
        FileSystemWatcher targetWatcher;

        object obj = new object();

        bool enabled = true;

        private readonly string sourceDirPath = @"D:\3_Sem\sharpP\Source";

        private readonly string targetDirPath = @"D:\3_Sem\sharpP\Archive";

        private readonly string extractDirPath = @"D:\3_Sem\sharpP\Extract";

        public Logger()
        {
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
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter(@"D:\3_Sem\sharpP\FileWatcherService\templog.txt",true))
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
                string fileEvent = "переименован в " + e.FullPath;
                string filePath = e.OldFullPath;

                RecordEntry(fileEvent, filePath);

                Archivator.Dearchive(e.FullPath, extractDirPath);
                File.Delete(e.FullPath);
            }
        }

        private void WatcherCreated(object sender, FileSystemEventArgs e)
        {
            Archivator.Archive(e.FullPath, targetDirPath);
        }
    }
}
