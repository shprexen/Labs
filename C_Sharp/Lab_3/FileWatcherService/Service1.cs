using System;
using System.ServiceProcess;
using System.IO;
using System.Threading;

namespace FileWatcherService
{
    public partial class Service1 : ServiceBase
    {
        Logger logger;
        ConfigurationOptions options;

        public Service1()
        {
            InitializeComponent();
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            string path = String.Concat(Directory.GetCurrentDirectory(), @"\config.xml");

            try
            {
                ConfigurationProvider provider = new ConfigurationProvider(path);
                options = provider.Parse<ConfigurationOptions>();

                logger = new Logger(options);
            }
            catch (Exception ex)
            {
                using (var str = new StreamWriter(new FileStream("Errors.txt", FileMode.OpenOrCreate)))
                {
                    str.Write(ex.Message + "\n" + ex.StackTrace);
                }
            }

            Thread loggerThread = new Thread(new ThreadStart(logger.Start));
            loggerThread.Start();
        }

        protected override void OnStop()
        {
            logger.Stop();
            Thread.Sleep(1000);
        }
    }
}
