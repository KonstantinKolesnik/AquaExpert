using MySensors.Controller.Core.Connectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MySensors.Controller
{
    class Program
    {
        static bool _continue;


        static IGatewayConnector connector;

        static void Main(string[] args)
        {
            //Thread readThread = new Thread(Read);
            int tries = 10;

            Console.WriteLine();
            Console.WriteLine("Starting MySensors network automation Controller.");
            Console.WriteLine();

            Console.Write("Starting gateway connector.");
            connector = new SerialGatewayConnector();
            connector.MessageReceived += connector_MessageReceived;
            while (tries > 0 && !connector.Connect())
            {
                Console.Write(".");
                tries--;
            }
            if (tries == 0)
                Console.WriteLine(" Failed.");
            else
            {
                Console.WriteLine(" Success.");




            }
            Console.WriteLine();



            _continue = true;
            //readThread.Start();

            Console.WriteLine("Controller started successfuly.");
            Console.WriteLine("Type quit to stop Controller and exit.");
            Console.WriteLine();

            while (!Console.ReadLine().Equals("quit")) ;

            //while (_continue)
            //{
            //    message = Console.ReadLine();

            //    if (stringComparer.Equals("quit", message))
            //    {
            //        _continue = false;
            //    }
            //    else
            //    {
            //        _serialPort.WriteLine(String.Format("<{0}>: {1}", name, message));
            //    }
            //}

            //readThread.Join();

            connector.Disconnect();
        }

        static void connector_MessageReceived(IGatewayConnector sender, string message)
        {
            Console.WriteLine(message);
        }

        public static void Read()
        {
            while (_continue)
            {
                try
                {
                    //string message = _serialPort.ReadLine();
                    //Console.WriteLine(message);
                }
                catch (TimeoutException) { }
            }
        }
    }
}
