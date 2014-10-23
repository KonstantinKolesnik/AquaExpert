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
            Thread readThread = new Thread(Read);

            Console.WriteLine("Starting Serial Gateway locator...");
            connector = new SerialGatewayConnector();
            connector.Connect();


            _continue = true;
            readThread.Start();

            Console.WriteLine("Type quit to exit");
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

            readThread.Join();
            //_serialPort.Close();
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
