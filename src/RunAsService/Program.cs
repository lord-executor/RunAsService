using System;
using System.ServiceProcess;

namespace RunAsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun = null;

            try
            {
                ServicesToRun = new ServiceBase[]
                                    {
                                        new RunAsService()
                                    };
            }
            catch (Exception e)
            {
                var sysroot = System.IO.Path.GetPathRoot(Environment.SystemDirectory);
                System.IO.File.AppendAllText(System.IO.Path.Combine(sysroot, "runasservice-crash.log"), e.ToString());
                Environment.Exit(-1);
            }

            ServiceBase.Run(ServicesToRun);
        }
    }
}
