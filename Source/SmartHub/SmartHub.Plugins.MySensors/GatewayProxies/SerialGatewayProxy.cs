using SmartHub.Plugins.MySensors.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace SmartHub.Plugins.MySensors.GatewayProxies
{
    class SerialGatewayProxy : IGatewayProxy, IDisposable
    {
        #region Fields
        private bool disposed = false;
        private SerialPort serialPort;
        private bool isPortValid = false;
        private Thread thread;
        //private System.Timers.Timer timer;
        #endregion

        #region Properties
        public bool IsStarted
        {
            get { return serialPort.IsOpen && isPortValid; }
        }
        #endregion

        #region Events
        public event EventHandler Connected;
        public event SensorMessageEventHandler MessageReceived;
        public event EventHandler Disconnected;
        #endregion

        #region Constructor
        public SerialGatewayProxy()
        {
            serialPort = new SerialPort();
            serialPort.BaudRate = 115200;
            serialPort.DtrEnable = true;
            serialPort.Encoding = Encoding.ASCII;
            serialPort.NewLine = "\n";
            serialPort.ReadTimeout = 10000;
            serialPort.WriteTimeout = 5000;

            thread = new Thread(() =>
            {
                while (!IsStarted && !FindAndConnect()) ;
            });
            thread.Priority = ThreadPriority.BelowNormal;

            //timer = new System.Timers.Timer(2000);
            //timer.AutoReset = false;
            //timer.Elapsed += (object source, ElapsedEventArgs e) =>
            //{
            //    Debug.WriteLine("Test");
            //    timer.Stop();

            //    if (!FindAndConnect())
            //        timer.Start();
            //};
        }
        #endregion

        #region Public methods
        public void Start()
        {
            thread.Start();
            //timer.Start();
        }
        public void Stop()
        {
            if (thread.IsAlive)
                //thread.Join();
                thread.Abort();
            //if (timer.Enabled)
            //    timer.Enabled = false;

            if (IsStarted)
            {
                serialPort.DataReceived -= serialPort_DataReceived;
                serialPort.Close();
                serialPort.Dispose();

                isPortValid = false;

                if (Disconnected != null)
                    Disconnected(this, EventArgs.Empty);
            }
        }

        public void Send(SensorMessage message)
        {
            if (IsStarted && message != null)
            {
                try
                {
                    Debug.WriteLine("Send: " + message.ToRawMessage());
                    serialPort.WriteLine(message.ToRawMessage());
                    Thread.Sleep(1);
                }
                catch (Exception)
                {
                    Debug.WriteLine("Error send: " + message.ToRawMessage());
                }
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Event handlers
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string str = null;
                while (IsStarted && !string.IsNullOrEmpty(str = serialPort.ReadLine()))
                {
                    SensorMessage msg = SensorMessage.FromRawMessage(str);
                    if (msg != null && MessageReceived != null)
                        MessageReceived(this, new SensorMessageEventArgs(msg));
                }
            }
            catch (TimeoutException) { }
            catch (IOException) { }
            catch (Exception) { }
        }
        #endregion

        #region Private methods
        private bool FindAndConnect()
        {
            if (!IsStarted)
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
                            Thread.Sleep(3000); // let hardware gateway initialize

                            try
                            {
                                string str = serialPort.ReadLine();
                                SensorMessage msg = SensorMessage.FromRawMessage(str);
                                if (msg != null && msg.Type == SensorMessageType.Internal && (InternalValueType)msg.SubType == InternalValueType.GatewayReady)
                                {
                                    serialPort.DataReceived += serialPort_DataReceived;

                                    isPortValid = true;

                                    if (MessageReceived != null)
                                        MessageReceived(this, new SensorMessageEventArgs(msg));

                                    if (Connected != null)
                                        Connected(this, EventArgs.Empty);

                                    return true;
                                }
                            }
                            catch (TimeoutException)
                            {
                            }

                            serialPort.Close();
                        }
                    }
                    catch (Exception)
                    {
                        serialPort.Close();
                    }
                }

                return false;
            }
            else
                return true;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    if (serialPort != null)
                        serialPort.Dispose();

                disposed = true;
            }
        }
        #endregion
    }
}
