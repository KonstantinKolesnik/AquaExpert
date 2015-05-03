using SmartHub.Plugins.MySensors.Core;
using System;
using System.IO;
using System.IO.Ports;

namespace SmartHub.Plugins.MySensors.GatewayProxies
{
    class SerialGatewayProxy : IGatewayProxy
    {
        #region Fields
        private SerialPort serialPort;
        #endregion

        #region Properties
        public bool IsStarted
        {
            get { return serialPort.IsOpen; }
        }
        #endregion

        #region Events
        public event SensorMessageEventHandler MessageReceived;
        #endregion

        #region Constructor
        public SerialGatewayProxy()
        {
            serialPort = new SerialPort();
            serialPort.BaudRate = 115200;
            serialPort.DtrEnable = true;
            serialPort.NewLine = "\n";
            serialPort.ReadTimeout = 4000;
            serialPort.WriteTimeout = 4000;

            serialPort.DataReceived += serialPort_DataReceived;
            serialPort.ErrorReceived += serialPort_ErrorReceived;
            serialPort.PinChanged += serialPort_PinChanged;
        }
        #endregion

        #region Public methods
        public void Start()
        {
            if (!serialPort.IsOpen)
            {
                var names = SerialPort.GetPortNames();

                foreach (string portName in names)
                {
                    serialPort.PortName = portName;

                    try
                    {
                        serialPort.Open();
                        if (serialPort.IsOpen)
                        {
                            serialPort.DiscardInBuffer();

                            try
                            {
                                string str = serialPort.ReadLine();
                                SensorMessage msg = SensorMessage.FromRawMessage(str);
                                if (msg != null && msg.Type == SensorMessageType.Internal && (InternalValueType)msg.SubType == InternalValueType.GatewayReady)
                                {
                                    if (MessageReceived != null)
                                        MessageReceived(this, new SensorMessageEventArgs(msg));

                                    return;
                                }
                            }
                            catch (TimeoutException) { }

                            Stop();
                        }
                    }
                    catch (Exception)
                    {
                        Stop();
                    }
                }
            }
        }
        public void Stop()
        {
            if (serialPort.IsOpen)
                serialPort.Close();
        }

        public void Send(SensorMessage message)
        {
            if (serialPort.IsOpen && message != null)
                serialPort.WriteLine(message.ToRawMessage());
        }
        #endregion

        #region Event handlers
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string str = null;
                while (serialPort.IsOpen && !string.IsNullOrEmpty(str = serialPort.ReadLine()))
                {
                    SensorMessage msg = SensorMessage.FromRawMessage(str);
                    if (msg != null && MessageReceived != null)
                        MessageReceived(this, new SensorMessageEventArgs(msg));
                }
            }
            catch (TimeoutException) { }
            catch (IOException) { }
            //catch (Exception) { }
        }
        private void serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            int a = 0;
            int b = a;
        }
        private void serialPort_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            int a = 0;
            int b = a;
        }
        #endregion
    }
}
