using MySensors.Controller.Core.Connectors;
using MySensors.Controller.Core.Messaging;
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



                Console.WriteLine();
                Console.WriteLine("Controller started successfuly! Type quit to stop Controller and exit.");
            }
            Console.WriteLine();



            _continue = true;
            //readThread.Start();


            while (!Console.ReadLine().Equals("quit")) ;

            connector.Disconnect();
        }

        static void connector_MessageReceived(IGatewayConnector sender, string message)
        {
            Message msg = Message.FromString(message);
            if (msg != null)
            {
                Console.WriteLine(msg.ToString());
                Console.WriteLine();
            }
        }
    }
}
