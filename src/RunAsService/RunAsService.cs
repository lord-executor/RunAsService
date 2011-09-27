using System;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using log4net;

namespace RunAsService
{
    public partial class RunAsService : ServiceBase
    {
        private readonly ILog _log;

        public RunAsService()
        {
            InitializeComponent();

            var serviceDirectory = (new Uri(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase))).AbsolutePath;
            Environment.SetEnvironmentVariable("SERVICE_DIRECTORY", serviceDirectory);
            log4net.Config.XmlConfigurator.Configure();

            _log = LogManager.GetLogger(typeof (RunAsService));
        }

        protected override void OnStart(string[] args)
        {
            _log.Info("Service starting");
        }

        protected override void OnStop()
        {
            _log.Info("Service stopping");
        }
    }
}
