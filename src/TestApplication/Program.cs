using System;
using System.Diagnostics;
using System.Threading;
using UnitTests.Application.Strings;
using System.Reflection;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Common.StartUp);

            Process.GetCurrentProcess().Exited += ProgramExited;

            if (args.Length == 1)
            {
                var methodName = args[0];
                var method = typeof (Program).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
                method.Invoke(null, null);
            }
            else
            {
                Console.WriteLine("No method name specified");
            }

            Console.WriteLine(Common.ShutDown);
        }

        static void ProgramExited(object sender, EventArgs e)
        {
            Console.WriteLine(Common.Exiting);
        }

        static void TestTerminatingNormally()
        {
            Console.WriteLine(TerminatingNormally.ServiceMessage);
        }

        static void TestNeverTerminating()
        {
            int i = 0;
            while (true)
            {
                Console.WriteLine(NeverTerminating.ServiceMessage);
                if (i % 13 == 0)
                    Console.Error.WriteLine(NeverTerminating.ErrorMessage);

                Thread.Sleep(5000);
                i++;
            }
        }
    }
}
