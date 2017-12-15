using SmartHub.Core.Infrastructure;
using System;

namespace SmartHub.ApplicationConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Smart Hub";
            Console.WriteLine("Starting Hub...");

            HubEnvironment.Init();

            var hub = new Hub();
            hub.Init();
            hub.StartServices();

            Console.WriteLine("Hub started successfully!");
            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();

            hub.StopServices();
        }
    }
}
