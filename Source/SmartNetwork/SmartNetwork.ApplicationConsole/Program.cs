using MySensors.Controllers;
using SmartNetwork.Core.Infrastructure;
using System;
using System.Threading;

namespace SmartNetwork.ApplicationConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //OldStart();
            NewStart();
        }

        private static void NewStart()
        {
            ControllerEnvironment.Init();

            var app = new SmartNetwork.Core.Infrastructure.Controller();

            app.Init();
            app.StartServices();

            Console.WriteLine("Service is available. Press ENTER to exit.");
            Console.ReadLine();

            app.StopServices();
        }

        private static void OldStart()
        {
            Console.Title = "MySensors Windows Controller";
            Console.WriteLine("Starting MySensors Controller.");
            Console.WriteLine("*******************************************************");

            MySensors.Controllers.Controller controller = new MySensors.Controllers.Controller(true);
            controller.Log += controller_Log;

            while (!controller.Start())
            {
                Console.WriteLine("*******************************************************");
                Thread.Sleep(1000);
            }

            Console.WriteLine("Controller started successfuly!");
            Console.WriteLine();
            Console.WriteLine("Type q to exit.");

            while (!Console.ReadLine().ToLower().Equals("q")) ;

            controller.Stop();
        }

        private static void controller_Log(MySensors.Controllers.Controller sender, string text, bool isLine, LogLevel logLevel)
        {
            if (!string.IsNullOrEmpty(text))
            {
                switch (logLevel)
                {
                    case LogLevel.Success: Console.ForegroundColor = ConsoleColor.Green; break;
                    case LogLevel.Error: Console.ForegroundColor = ConsoleColor.Red; break;
                    case LogLevel.Warning: Console.ForegroundColor = ConsoleColor.Yellow; break;
                    case LogLevel.Normal:
                    default:
                        Console.ResetColor(); break;
                }

                if (isLine)
                    Console.WriteLine(text);
                else
                    Console.Write(text);
            }

            Console.ResetColor();
        }
    }
}
