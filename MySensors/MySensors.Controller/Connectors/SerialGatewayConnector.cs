using MySensors.Core.Messaging;
using MySensors.Core.Nodes;
using System;
using System.IO.Ports;

namespace MySensors.Controller.Connectors
{
    public class SerialGatewayConnector : IGatewayConnector
    {
        private SerialPort serialPort;

        public Node Node { get; private set; }

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
                                Node = new Node(msg.NodeID);
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
            string str = serialPort.ReadLine();
            Message msg = Message.FromRawString(str);

            if (msg != null && MessageReceived != null)
                MessageReceived(this, msg);
        }
    }
}
