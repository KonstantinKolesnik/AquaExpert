using MySensors.Core.Messaging;
using MySensors.Core.Nodes;
using System;
using System.IO.Ports;

namespace MySensors.Core.Connectors
{
    public class SerialGatewayConnector : IGatewayConnector
    {
        private SerialPort serialPort;

        public bool IsConnected { get { return serialPort.IsOpen; } }

        public event MessageEventHandler MessageReceived;

        public SerialGatewayConnector()
        {
            serialPort = new SerialPort();
            serialPort.BaudRate = 115200;
            serialPort.DtrEnable = true;
            serialPort.ReadTimeout = 3000;
            serialPort.WriteTimeout = 3000;
        }

        public bool Connect()
        {
            foreach (string portName in SerialPort.GetPortNames())
            {
                serialPort.PortName = portName;

                try
                {
                    serialPort.Open();

                    if (serialPort.IsOpen)
                    {
                        try
                        {
                            string str = serialPort.ReadLine();
                            Message msg = Message.FromRawString(str);
                            if (msg != null && msg.MessageType == MessageType.Internal && (InternalValueType)msg.SubType == InternalValueType.GatewayReady)
                            {
                                serialPort.DataReceived += serialPort_DataReceived;

                                if (MessageReceived != null)
                                    MessageReceived(this, msg);

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
            serialPort.DataReceived -= serialPort_DataReceived;

            if (serialPort.IsOpen)
                serialPort.Close();
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string str = serialPort.ReadLine();
            Message msg = Message.FromRawString(str);

            if (msg != null && MessageReceived != null)
                MessageReceived(this, msg);
        }
    }
}
