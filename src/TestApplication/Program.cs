using System;
using System.Diagnostics;
using System.Threading;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting application");

            Process.GetCurrentProcess().Exited += ProgramExited;

            int i = 0;
            while (true)
            {
                Console.WriteLine("Doing service-y stuff...");
                if (i % 13 == 0)
                    Console.Error.WriteLine("There might be the occasional error");

                Thread.Sleep(5000);
                i++;
            }
        }

        static void ProgramExited(object sender, EventArgs e)
        {
            Console.WriteLine("Stopping application");
        }
    }
}
