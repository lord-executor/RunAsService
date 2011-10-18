using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using RunAsService.Properties;
using log4net;

namespace RunAsService
{
    public class ChildProcessWrapper
    {
        #region Members

        /// <summary>
        /// Service logger
        /// </summary>
        private readonly ILog _serviceLog;
        /// <summary>
        /// Child process logger
        /// </summary>
        private readonly ILog _childLog;
        /// <summary>
        /// Child process start info
        /// </summary>
        private readonly ProcessStartInfo _startInfo;
        /// <summary>
        /// Child process handle
        /// </summary>
        private Process _childProcess;

        #endregion


        #region Constructors

        public ChildProcessWrapper(ILog serviceLog, ChildProcessSettings settings)
        {
            _serviceLog = serviceLog;

            _childLog = LogManager.GetLogger("ChildProcess");

            _startInfo = new ProcessStartInfo
            {
                FileName = settings.FileName,
                Arguments = settings.Arguments,
                CreateNoWindow = true,
                ErrorDialog = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                StandardErrorEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8,
                UseShellExecute = false, // required for error/output redirection
                WorkingDirectory = settings.WorkingDirectory,
            };
        }

        #endregion


        #region Child process management

        public bool Start()
        {
            try
            {
                _childProcess = Process.Start(_startInfo);
            }
            catch (Exception e)
            {
                _serviceLog.Error("Unable to start child process", e);
                return false;
            }

            _serviceLog.Info(String.Format("Child process '{0}' ({1}) spawned", _childProcess.StartInfo.FileName, _childProcess.Id));

            // register child process event handlers and start asynchronous listening
            _childProcess.Exited += ChildProcessExited;
            _childProcess.OutputDataReceived += ChildProcessOutputDataReceived;
            _childProcess.ErrorDataReceived += ChildProcessErrorDataReceived;

            _childProcess.BeginOutputReadLine();
            _childProcess.BeginErrorReadLine();

            return true;
        }

        public void Terminate()
        {
            if (_childProcess != null && !_childProcess.HasExited)
            {
                _serviceLog.Info("Attempting graceful shutdown");
                _childProcess.CloseMainWindow();
                if (!_childProcess.WaitForExit(3000))
                {
                    _serviceLog.Warn("Child process did not terminate within 3s. Killing it now.");
                    _childProcess.Kill();
                    if (!_childProcess.WaitForExit(3000))
                    {
                        _serviceLog.Error("Child process did not react to KILL within 3s");

                    }
                }

                _childProcess.Close();
            }

            _childProcess = null;
        }

        #endregion


        #region Child process output listeners

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

        #endregion
    }
}
