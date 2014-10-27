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
            Console.Title = "MySensors Controller";
            Console.WriteLine("Starting MySensors Controller.");

            Console.WriteLine("*******************************************************");

            controller = new Controller(true);
            controller.ComponentStartEvent += controller_ComponentStartEvent;

            bool exit = false;
            Thread thread = new Thread(() => {
                while (!exit && !controller.Start())
                {
                    Console.WriteLine("*******************************************************");
                    Thread.Sleep(3000);
                }
                
                if (!exit)
                    Console.WriteLine("Controller started successfuly!");
            });
            thread.Start();

            //Console.WriteLine();
            //Console.WriteLine("Type q to exit.");
            //Console.WriteLine();

            while (!Console.ReadLine().Equals("q")) ;
            //string s;
            //while (!(s = Console.ReadLine()).Equals("\n")) ;

            bool _continue = true;
            string msg;
            while (_continue)
            {
                msg = Console.ReadLine();

                if (msg.Equals("q"))
                {
                    _continue = false;
                }
            }

            //exit = true;
            thread.Join();

            controller.Stop();
        }

        private static void controller_ComponentStartEvent(Controller sender, string text, string textLine)
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
