using SmartHub.Core.Infrastructure;
using System;

namespace SmartHub.ApplicationConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Smart Hub";
            Console.Write("Starting hub... ");

            //OldStart();
            NewStart();
        }

        private static void NewStart()
        {
            HubEnvironment.Init();

            var controller = new Hub();
            controller.Init();
            controller.StartServices();

            Console.WriteLine("Success!");
            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();

            controller.StopServices();
        }
        private static void OldStart()
        {
            //MySensors.Controllers.Controller controller = new MySensors.Controllers.Controller(true);
            //controller.Log += (sender, text, isLine, logLevel) => {
            //    if (!string.IsNullOrEmpty(text))
            //    {
            //        switch (logLevel)
            //        {
            //            case LogLevel.Success: Console.ForegroundColor = ConsoleColor.Green; break;
            //            case LogLevel.Error: Console.ForegroundColor = ConsoleColor.Red; break;
            //            case LogLevel.Warning: Console.ForegroundColor = ConsoleColor.Yellow; break;
            //            case LogLevel.Normal:
            //            default:
            //                Console.ResetColor(); break;
            //        }

            //        if (isLine)
            //            Console.WriteLine(text);
            //        else
            //            Console.Write(text);
            //    }

            //    Console.ResetColor();
            //};
            

            //while (!controller.Start())
            //{
            //    Console.WriteLine("*******************************************************");
            //    Thread.Sleep(1000);
            //}

            //Console.WriteLine("Controller started successfuly! Press ENTER to exit.");
            ////while (!Console.ReadLine().ToLower().Equals("q")) ;
            //Console.WriteLine();
            //Console.ReadLine();

            //controller.Stop();
        }
    }
}
