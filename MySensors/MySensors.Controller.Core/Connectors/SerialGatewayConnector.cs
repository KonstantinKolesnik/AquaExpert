using System;
using System.IO.Ports;

namespace MySensors.Controller.Core.Connectors
{
    public class SerialGatewayConnector : IGatewayConnector
    {
        private SerialPort serialPort;
        private string portName;

        public event MessageEventHandler MessageReceived;

        public SerialGatewayConnector()
        {
            serialPort = new SerialPort();
            serialPort.BaudRate = 115200;
            serialPort.DtrEnable = true;
            serialPort.ReadTimeout = 1000;
            serialPort.WriteTimeout = 1000;
        }

        public bool Connect()
        {
            portName = null;

            foreach (string pn in SerialPort.GetPortNames())
            {
                serialPort.PortName = pn;

                try
                {
                    serialPort.Open();

                    if (serialPort.IsOpen)
                    {
                        try
                        {
                            serialPort.Write("SGW\n");
                            string message = serialPort.ReadLine();
                            if (message.Equals("SGWOK"))
                            {
                                serialPort.DataReceived += serialPort_DataReceived;
                                return true;
                            }
                        }
                        catch (TimeoutException) { }

                        Disconnect();
                    }
                }
                catch (Exception) {}
            }

            return false;
        }
        public void Disconnect()
        {
            if (serialPort.IsOpen)
                serialPort.Close();

            serialPort.DataReceived -= serialPort_DataReceived;
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string msg = serialPort.ReadLine();
            if (MessageReceived != null)
                MessageReceived(this, msg);
        }
    }
}
