using MySensors.Core;
using System;
using System.Threading;

namespace MySensors.Server
{
    class Program
    {
        private static Controller controller;

        static void Main(string[] args)
        {
            Console.Title = "MySensors Windows Controller";
            Console.WriteLine("Starting MySensors Controller.");
            Console.WriteLine("*******************************************************");

            controller = new Controller(true);
            controller.Log += controller_Log;

            bool exit = false;

            Thread thread = new Thread(() =>
            {
                while (!exit && !controller.Start())
                {
                    Console.WriteLine("*******************************************************");
                    Thread.Sleep(3000);
                }

                if (!exit)
                    Console.WriteLine("Controller started successfuly!");
            });
            thread.Start();

            Console.WriteLine();
            Console.WriteLine("Type q to exit.");
            Console.WriteLine();

            while (!exit)
                if (Console.ReadLine().Equals("q"))
                    exit = true;

            thread.Join();

            controller.Stop();
        }

        private static void controller_Log(Controller sender, string text, string textLine)
        {
            Console.ResetColor();

            if (!string.IsNullOrEmpty(text))
                Console.Write(text);
            else
            {
                //Console.ForegroundColor = result.Value ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine(textLine);
            }

            Console.ResetColor();
        }
    }
}
