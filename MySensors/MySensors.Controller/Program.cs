using MySensors.Controller.Connectors;
using System;

namespace MySensors.Controller
{
    class Program
    {
        private static Controller controller;

        static void Main(string[] args)
        {
            //Thread readThread = new Thread(Read);

            Console.WriteLine("Starting MySensors network automation Controller.");
            Console.WriteLine();

            controller = new Controller();
            controller.GatewayConnector = new SerialGatewayConnector();

            if (StartConnector())
            {



            }


            Console.WriteLine();
            Console.WriteLine("Controller started successfuly! Type quit to stop Controller and exit.");
            Console.WriteLine();

            //readThread.Start();


            while (!Console.ReadLine().Equals("quit")) ;

            Stop();
        }

        private static bool StartConnector()
        {
            Console.Write("Starting gateway connector.");
            
            int tries = 10;
            while (tries-- > 0 && !controller.GatewayConnector.Connect())
                Console.Write(".");

            if (tries == 0)
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
