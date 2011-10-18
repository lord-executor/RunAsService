using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using log4net;
using RunAsService.Properties;

namespace RunAsService
{
    /// <summary>
    /// RunAsService service implementation. It sets up the environment to ensure correct logging
    /// and starts the child process when the service is started. stdout and stderr of the child
    /// process are redirected and logged into the RunAsService log file
    /// </summary>
    public partial class RunAsService : ServiceBase
    {
        #region Members

        /// <summary>
        /// Service logger
        /// </summary>
        private readonly ILog _log;
        /// <summary>
        /// Direcotry where the RunAsService executable is located (used as base path for all
        /// relative paths)
        /// </summary>
        private readonly string _serviceDirectory;
        /// <summary>
        /// Child process wrapper
        /// </summary>
        private ChildProcessWrapper _wrapper;

        #endregion


        #region Constructors

        public RunAsService()
        {
            InitializeComponent();

            // get the path of the RunAsService assembly
            _serviceDirectory = (new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase))).AbsolutePath;
            // set up the SERVICE_DIRECTORY environment variable so that it can be used in the log4net configuration
            Environment.SetEnvironmentVariable("SERVICE_DIRECTORY", _serviceDirectory);

            // load the log4net configuration and get the two loggers
            log4net.Config.XmlConfigurator.Configure();
            _log = LogManager.GetLogger(typeof (RunAsService));
        }

        #endregion


        #region Service implementation

        protected override void OnStart(string[] args)
        {
            _log.Info("Service starting");

            if (_wrapper == null)
            {
                if (String.IsNullOrEmpty(Settings.Default.ServiceExecutable))
                {
                    _log.Error("ServiceExecutable can not be empty");
                    Stop();
                    return;
                }

                var fullExePath = Path.IsPathRooted(Settings.Default.ServiceExecutable)
                                      ? Settings.Default.ServiceExecutable
                                      : Path.Combine(_serviceDirectory, Settings.Default.ServiceExecutable);

                var childSettings = new ChildProcessSettings
                                        {
                                            FileName = fullExePath,
                                            Arguments = Settings.Default.ServiceArguments ?? String.Empty,
                                            WorkingDirectory = _serviceDirectory,
                                        };
                _wrapper = new ChildProcessWrapper(_log, childSettings);
            }

            if (!_wrapper.Start())
                Stop();
        }

        protected override void OnStop()
        {
            _log.Info("Service stopping");

            if (_wrapper != null)
                _wrapper.Terminate();

            _log.Info("Service stopped");
        }

        #endregion
    }
}
