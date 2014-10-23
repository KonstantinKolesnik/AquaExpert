using MySensors.Controller.Connectors;
using System;

namespace MySensors.Controller
{
    class Program
    {
        private static Controller controller;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting MySensors network automation Controller.");
            Console.WriteLine();

            controller = new Controller();
            controller.GatewayConnector = new SerialGatewayConnector();

            if (StartConnector())
            {


                Console.WriteLine("Controller started successfuly!");
            }
            else
                Console.WriteLine("Controller stopped.");

            Console.WriteLine();
            Console.WriteLine("Type quit to exit.");
            Console.WriteLine();

            while (!Console.ReadLine().Equals("quit")) ;

            Stop();
        }

        private static bool StartConnector()
        {
            Console.Write("Starting gateway connector...");
            
            if (!controller.GatewayConnector.Connect())
            {
                Console.WriteLine(" Failed.");
                return false;
            }
            else
            {
                Console.WriteLine(" Success.");
                return true;
            }
        }


        private static void Stop()
        {
            controller.GatewayConnector.Disconnect();
        }
    }
}
