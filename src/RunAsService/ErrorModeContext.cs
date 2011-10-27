using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace RunAsService
{
    /// <summary>
    /// Disposable error mode context switcher. This allows you to suppress (or allow) specific error
    /// notification dialogs, for example the Just-in-time (JIT) debugging dialog that pops up when an
    /// application crashes. This is especially useful in the case where child processes are spawned
    /// since they inherit the error mode settings from the parent process, thereby allowing you
    /// to supress error dialogs from child processes. See http://msdn.microsoft.com/en-us/library/windows/desktop/ms680621%28v=vs.85%29.aspx
    /// for more information on the Win32 SetErrorMode function.
    /// </summary>
    public class ErrorModeContext : IDisposable
    {
        #region Members

        private readonly int _oldMode;

        #endregion


        #region Constructors and destructors

        public ErrorModeContext(ErrorModes mode)
        {
            _oldMode = SetErrorMode((int)mode);
        }

        ~ErrorModeContext()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            SetErrorMode(_oldMode);
        }

        #endregion


        #region IDisposable implementation

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        #endregion


        #region Win32 methods

        [DllImport("kernel32.dll")]
        private static extern int SetErrorMode(int newMode);

        #endregion


        [Flags]
        public enum ErrorModes
        {
            /// <summary>
            /// Display all dialog boxes
            /// </summary>
            Default = 0x0,
            /// <summary>
            /// Do not display the critical-error-handler message box. Errors are instead sent
            /// to the calling process
            /// </summary>
            FailCriticalErrors = 0x1,
            /// <summary>
            /// Do not display the Windows Error Reporting dialog (JIT debugger)
            /// </summary>
            NoGpFaultErrorBox = 0x2,
            /// <summary>
            /// Do not display the memory alignment fault dialog (tries to fix them automatically)
            /// </summary>
            NoAlignmentFaultExcept = 0x4,
            /// <summary>
            /// Do not display an error message when the application fails to find a file
            /// </summary>
            NoOpenFileErrorBox = 0x8000
        }
    }
}
