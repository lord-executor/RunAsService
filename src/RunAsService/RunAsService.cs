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
        /// <summary>
        /// Service logger
        /// </summary>
        private readonly ILog _log;
        /// <summary>
        /// Child process logger
        /// </summary>
        private readonly ILog _childLog;
        /// <summary>
        /// Direcotry where the RunAsService executable is located (used as base path for all
        /// relative paths)
        /// </summary>
        private readonly string _serviceDirectory;
        /// <summary>
        /// Child process handle
        /// </summary>
        private Process _childProcess;

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
            _childLog = LogManager.GetLogger("ChildProcess");
        }

        protected override void OnStart(string[] args)
        {
            _log.Info("Service starting");

            if (String.IsNullOrEmpty(Settings.Default.ServiceExecutable))
            {
                _log.Error("ServiceExecutable can not be empty");
                Stop();
                return;
            }

            var fullExePath = Path.IsPathRooted(Settings.Default.ServiceExecutable)
                                  ? Settings.Default.ServiceExecutable
                                  : Path.Combine(_serviceDirectory, Settings.Default.ServiceExecutable);

            var startInfo = new ProcessStartInfo
                                {
                                    FileName = fullExePath,
                                    Arguments = Settings.Default.ServiceArguments ?? String.Empty,
                                    CreateNoWindow = true,
                                    ErrorDialog = false,
                                    RedirectStandardError = true,
                                    RedirectStandardOutput = true,
                                    StandardErrorEncoding = Encoding.UTF8,
                                    StandardOutputEncoding = Encoding.UTF8,
                                    UseShellExecute = false, // required for error/output redirection
                                    WorkingDirectory = _serviceDirectory, // set working directory to service directory
                                };

            try
            {
                _childProcess = Process.Start(startInfo);
            }
            catch (Exception e)
            {
                _log.Error("Unable to start child process", e);
                Stop();
                return;
            }

            _log.Info(String.Format("Child process '{0}' ({1}) spawned", _childProcess.StartInfo.FileName, _childProcess.Id));

            // register child process event handlers and start asynchronous listening
            _childProcess.Exited += ChildProcessExited;
            _childProcess.OutputDataReceived += ChildProcessOutputDataReceived;
            _childProcess.ErrorDataReceived += ChildProcessErrorDataReceived;

            _childProcess.BeginOutputReadLine();
            _childProcess.BeginErrorReadLine();
        }

        private void ChildProcessExited(object sender, EventArgs e)
        {
            _childLog.InfoFormat("Child process exited with exit code {0} ({0:x8})", _childProcess.ExitCode);
        }

        private void ChildProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _childLog.Info(e.Data);
        }

        private void ChildProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            _childLog.Warn(e.Data);
        }

        protected override void OnStop()
        {
            _log.Info("Service stopping");

            if (_childProcess != null && !_childProcess.HasExited)
            {
                _log.Info("Attempting graceful shutdown");
                _childProcess.CloseMainWindow();
                if (!_childProcess.WaitForExit(3000))
                {
                    _log.Warn("Child process did not terminate within 3s. Killing it now.");
                    _childProcess.Kill();
                    if (!_childProcess.WaitForExit(3000))
                    {
                        _log.Error("Child process did not react to KILL within 3s");
                        
                    }
                }

                _childProcess.Close();
            }

            _childProcess = null;
            _log.Info("Service stopped");
        }
    }
}
