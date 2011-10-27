using System;
using System.Diagnostics;
using System.Threading;
using UnitTests.Application.Strings;
using System.Reflection;
using System.Linq;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Common.StartUp);

            if (args.Contains("--globalhandler"))
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Process.GetCurrentProcess().Exited += ProgramExited;

            if (args.Length >= 1)
            {
                var methodName = args[0];
                var method = typeof(Program).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);

                if (method == null)
                    throw new Exception(String.Format("Method '{0}'", methodName));

                try
                {
                    method.Invoke(null, null);
                }
                catch (TargetInvocationException e)
                {
                    typeof (Exception)
                        .GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic)
                        .Invoke(e.InnerException, null);
                    throw e.InnerException;
                }
            }
            else
            {
                Console.WriteLine("No method name specified");
            }

            Console.WriteLine(Common.ShutDown);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(((Exception)e.ExceptionObject).Message);
            Environment.Exit(-1);
        }

        static void ProgramExited(object sender, EventArgs e)
        {
            Console.WriteLine(Common.Exiting);
        }

        static void TestTerminatingNormally()
        {
            Console.WriteLine(Common.ServiceMessage);
        }

        static void TestNeverTerminating()
        {
            int i = 0;
            while (true)
            {
                Console.WriteLine(Common.ServiceMessage);
                if (i % 13 == 0)
                    Console.Error.WriteLine(Common.WarningMessage);

                Thread.Sleep(5000);
                i++;
            }
        }

        static void TestExceptionTerminating()
        {
            while (true)
            {
                Console.WriteLine(Common.ServiceMessage);
                Thread.Sleep(250);
                throw new Exception(Common.ExceptionMessage);
            }
        }

        static void TestExitTerminating()
        {
            while (true)
            {
                Console.WriteLine(Common.ServiceMessage);
                Environment.Exit(42);
            }
        }
    }
}
