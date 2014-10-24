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
            //Console.WriteLine("Controller will start the following components:");
            //Console.WriteLine("\t1) gateway connector;");
            //Console.WriteLine("\t2) DNS network name;");
            //Console.WriteLine("\t3) http server.");

            Console.WriteLine("*******************************************************");

            controller = new Controller(true);
            controller.ComponentStartEvent += controller_ComponentStartEvent;

            bool exit = false;
            Thread thread = new Thread(() => {
                //while (!exit && !controller.Start())
                    Console.WriteLine("*******************************************************");
                
                if (!exit)
                    Console.WriteLine("Controller started successfuly!");
            });
            thread.Start();

            Console.WriteLine();
            Console.WriteLine("Type q to exit.");
            Console.WriteLine();

            //while (!Console.ReadLine().Equals("q")) ;
            //string s;
            //while (!(s = Console.ReadLine()).Equals("\n")) ;

            bool _continue = true;
            string msg;
            while (_continue)
            {
                msg = Console.ReadLine();

                if (msg.Equals("quit"))
                {
                    _continue = false;
                }
            }

            //exit = true;
            thread.Join();

            controller.Stop();
        }

        private static void controller_ComponentStartEvent(Controller sender, ControllerComponent component, bool? result)
        {
            Console.ResetColor();

            if (!result.HasValue)
            {
                switch (component)
                {
                    case ControllerComponent.GatewayConnector: Console.Write("Connecting to gateway..."); break;
                    case ControllerComponent.NameService: Console.Write("Starting name service..."); break;
                    case ControllerComponent.WebServer: Console.Write("Starting web-server..."); break;
                }
            }
            else
            {
                Console.ForegroundColor = result.Value ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine(result.Value ? " Success." : " Failed.");
            }

            Console.ResetColor();
        }
    }
}
