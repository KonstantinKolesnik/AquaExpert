using AquaExpert.UI;
using MFE.Net.Http;
using MFE.Net.Managers;
using MFE.Net.Messaging;
using MFE.Net.Tcp;
using MFE.Net.Udp;
using MFE.Net.WebSocket;
using Microsoft.SPOT;
using SmartNetwork.Network;
using System;
using System.Collections;
using System.Net;
using System.Reflection;
using System.Threading;
using GT = Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.LoveElectronics;
using Networking;
using System.Text;
using System.IO;
using Microsoft.SPOT.Hardware;

namespace AquaExpert
{
    public partial class Program
    {
        #region Fields
        private INetworkManager networkManager;
        private HttpServer httpServer;
        private WSServer wsServer;
        //private TcpServer tcpServer;
        private DiscoveryListener discoveryListener;
        private NetworkMessageFormat msgFormat = NetworkMessageFormat.Text;

        private State state = new State();
        private Settings settings = null;

        private NetworkCoordinator networkCoordinator;
        private BusHubI2C busHubI2C;

        private GT.Timer timerNetworkOff;
        private GT.Timer timerTest;

        private RFManager rfManager;

        internal static Gadgeteer.Modules.LoveElectronics.Button Button;
        #endregion

        #region Properties
        public static string Name
        {
            get { return Assembly.GetExecutingAssembly().GetName().Name; }
        }
        public static string Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }
        public Settings Settings
        {
            get { return settings; }
        }
        public State State
        {
            get { return state; }
        }
        #endregion

        private void ProgramStarted()
        {
            Button = button;

            InitUI();

            InitSettings();
            //InitRF();
            //InitBus();
            InitHardware();
            InitNetwork();

            //Mainboard.SetDebugLED(true);
        }

        #region Private methods
        private void InitUI()
        {
            //UIManager.SplashPage.Title = "Smart Network";// "Aqua Expert";
            //UIManager.Desktop.Children.Add(UIManager.SplashPage);
            //for (int i = 0; i <= 100; i++)
            //{
            //    UIManager.SplashPage.ProgressValue = i;
            //    Thread.Sleep(10);
            //}
            //UIManager.Desktop.Children.Remove(UIManager.SplashPage);

            UIManager.Desktop.Children.Add(UIManager.DebugPage);
        }
        private void InitSettings()
        {
            //if (sdCard.IsCardInserted)
            //{
            //    sdCard.MountSDCard();
            //    Thread.Sleep(500);
            //    settings = Settings.LoadFromSD(sdCard.GetStorageDevice().RootDirectory);
            //}
            //settings = Settings.LoadFromFlash(0);
            settings = new Settings();
        }
        private void InitRF()
        {
            rfManager = new RFManager();
        }
        private void InitBus()
        {
            //var socket = Socket.GetSocket(12, true, null, null);
            //var i2c = new I2CBus(socket, 1, busClockRate, null);

            //byte[] b = new byte[5];
            //int i = i2c.Read(b, 1000);
            //Debug.Print(b[0].ToString() + i);

            //byte[] b = new byte[1];
            //int i = i2c.WriteRead(new byte[] {0x00}, b, 1000);
            //Debug.Print(b[0].ToString() + i);
            //- See more at: https://www.ghielectronics.com/community/forum/topic?id=13503&page=2#msg137894


            networkCoordinator = new NetworkCoordinator();

            //busHubI2C = new BusHubI2C(new BusConfiguration(new I2CDevice(null)));
            //busHubI2C.BusModulesCollectionChanged += busHubI2C_BusModulesCollectionChanged;
            //networkCoordinator.BusHubs.Add(busHubI2C);
        }
        private void InitHardware()
        {
            indicators.TurnAllLedsOff();

            GHI.Premium.Hardware.LowLevel.Watchdog.Disable();
            //GHI.Premium.Hardware.LowLevel.Watchdog.Enable(10 * 1000); // 10 sec

            timerTest = new Gadgeteer.Timer(500);
            timerTest.Tick += timerTest_Tick;
            timerTest.Start();
        }
        private void InitNetwork()
        {
            //tcpServer = new TcpServer(Settings.IPPort);
            //tcpServer.SessionConnected += Session_Connected;
            //tcpServer.SessionDataReceived += Session_DataReceived;
            //tcpServer.SessionDisconnected += Session_Disconnected;

            discoveryListener = new DiscoveryListener(Settings.UDPPort, "AquaExpert");

            wsServer = new WSServer(Settings.WSPort);
            wsServer.SessionConnected += Session_Connected;
            wsServer.SessionDataReceived += Session_DataReceived;
            wsServer.SessionDisconnected += Session_Disconnected;

            httpServer = new HttpServer();
            httpServer.OnRequest += httpServer_OnRequest;
            httpServer.OnGetRequest += httpServer_OnGetRequest;
            httpServer.OnResponse += httpServer_OnResponse;

            //if (!Utils.StringIsNullOrEmpty(Settings.WiFiSSID))
                //networkManager = new WiFiManager(wifiRS21.Interface, settings.WiFiSSID, settings.WiFiPassword);
            networkManager = new EthernetManager(ethernetENC28.Interface);
            networkManager.Started += new EventHandler(Network_Started);
            networkManager.Stopped += new EventHandler(Network_Stopped);
            
            timerNetworkOff = new Gadgeteer.Timer(500);
            timerNetworkOff.Tick += delegate(Gadgeteer.Timer t) { SetLed(Leds.Network, !GetLed(Leds.Network)); };
            timerNetworkOff.Start();

            new Thread(() => { networkManager.Start(); }).Start();
        }

        private void SetState()
        {
            //state.Temperature = sensorPHTemp.Temperature;
            //state.PH = sensorPHTemp.PH;

            //if (!state.IsManualMode)
            //{
            //    DateTime dt = RealTimeClock.GetTime();
            //    //Debug.Print(dt.ToString());

            //    state.IsLightOn = dt.Hour >= settings.LightOnHour && dt.Hour < settings.LightOffHour;
            //    state.IsCO2On = dt.Hour >= settings.CO2OnHour && dt.Hour < settings.CO2OffHour;

            //    if (!sensorWaterMin.IsWet) // если мин. уровень достигнут, то останавливаем слив
            //        state.IsWaterOutMode = false;

            //    state.IsWaterInMode = !state.IsWaterOutMode; // если нет слива в данный момент, то набираем
            //}

            //// ПО-ЛЮБОМУ!!! если макс. уровень достигнут, то останавливаем набор
            //if (sensorWaterMax.IsWet)
            //    state.IsWaterInMode = false;
        }
        private void SendStateToClients()
        {
            //NetworkMessage msg = new NetworkMessage("State");
            //msg["TimeStamp"] = DateTime.Now.ToString();
            //msg["Version"] = Version;
            //byte[] data = msg.Pack(msgFormat);

            //wsServer.SendToAll(data, 0, data.Length);
            ////tcpServer.SendToAll(data);
        }

        private void BlinkLed(Leds led)
        {
            new Thread(() =>
            {
                SetLed(led, false);
                Thread.Sleep(10);
                SetLed(led, true);
            }).Start();
        }
        private void SetLed(Leds led, bool state)
        {
            indicators[(int)led] = state;
        }
        private bool GetLed(Leds led)
        {
            return indicators[(int)led];
        }

        private void Beep(int frequency)
        {
            tunes.Play(frequency);
            Thread.Sleep(100);
            tunes.Stop();
        }
        #endregion

        #region Public methods
        public void WaterIn(bool on) // набор воды
        {
            // в автоматическом режиме не влияет на поведение
            // в ручном режиме идет набор до достижения макс. уровня, независимо от режима слива
            state.IsWaterInMode = on;
        }
        public void WaterOut(bool on) // слив воды
        {
            // в ручном режиме спустит всю воду
            // в автоматическом режиме сделает замену воды
            state.IsWaterOutMode = on;
        }
        #endregion

        #region Event handlers
        private void Network_Started(object sender, EventArgs e)
        {
            timerNetworkOff.Stop();
            SetLed(Leds.Network, true);

            Beep(1000);

            //tcpServer.Start();
            discoveryListener.Start();
            //httpServer.Start("http", 80);
            wsServer.Start();
            TimeManager.Start();




            // socket S:
            // pin 3: inerrupt (PB1)
            // pin 4: reset (PC23)
            // pin 5: (PB3)
            // pin 6: CS (PA28)
            // pin 7: MOSI (PA12)
            // pin 8: MISO (PA11)
            // pin 9: SCK (PA13)

            //var enc = new GHI.Premium.Net.EthernetENC28J60(
            //    SPI.SPI_module.SPI1,
            //    Cpu.Pin.GPIO_Pin0 , // chip select
            //    Cpu.Pin.GPIO_Pin1 , // external interrupt
            //    Cpu.Pin.GPIO_Pin2 // reset
            //);
            Adapter.Start(
                new byte[] { 0x5c, 0x86, 0x4a, 0x00, 0x00, 0xde },
                "AquaExpert",
                GT.Socket.GetSocket(3, true, null, null).SPIModule,
                GT.Socket.GetSocket(3, true, null, null).CpuPins[3],
                GT.Socket.GetSocket(3, true, null, null).CpuPins[6]
                );

            //Adapter.Start(
            //    new byte[] { 0x5c, 0x86, 0x4a, 0x00, 0x00, 0xde },
            //    "AquaExpert",
            //    SPI.SPI_module.SPI1,
            //    GHI.Hardware.G400.Pin.PB1, //GT.Socket.GetSocket(3, true, null, null).CpuPins[3],
            //    GHI.Hardware.G400.Pin.PA28 // GT.Socket.GetSocket(3, true, null, null).CpuPins[6]
            //    );


            Adapter.OnHttpReceivedPacketEvent += new Adapter.HttpPacketReceivedEventHandler(Adapter_OnHttpReceivedPacketEvent);
            Adapter.ListenToPort(80);  // Listen on Port 80, the default web server port
        }
        private void Network_Stopped(object sender, EventArgs e)
        {
            SetLed(Leds.Network, false);
            timerNetworkOff.Start();

            //tcpServer.Stop();
            //httpServer.Stop();
            wsServer.Stop();
            TimeManager.Stop();



            Adapter.Stop();
        }


        private void Adapter_OnHttpReceivedPacketEvent(HttpRequest request)
        {
            byte[] webPage = Encoding.UTF8.GetBytes("<html><head><meta http-equiv=\"refresh\" content=\"5\"></head><body><font face=\"verdana\"><h1>mip - A Managed TCP/IP Stack</h1><p>for .NET MicroFramework</p><p>This is just the beginning :)</p><p><a href=\"http://mip.codeplex.com\">Visit us at Codeplex!</a></p></font></body></html>");
            var s = new MemoryStream(webPage);  // substitute a FileStream here when reading from MicroSD
            request.SendResponse(new HttpResponse(s));
        }


        private void httpServer_OnRequest(HttpListenerRequest request)
        {
            BlinkLed(Leds.Network);
        }
        private void httpServer_OnGetRequest(string path, Hashtable parameters, HttpListenerResponse response)
        {
            if (sdCard.IsCardMounted)
            {
                if (path.ToLower() == "\\admin") // There is one particular URL that we process differently
                {
                    //httpServer.ProcessPasswordProtectedArea(request, response);
                }
                else
                    httpServer.SendFile(sdCard.GetStorageDevice().RootDirectory + "\\" + Name + path, response);
            }
        }
        private void httpServer_OnResponse(HttpListenerResponse response)
        {
            BlinkLed(Leds.Network);
        }

        private void Session_Connected(TcpSession session)
        {
            session.Tag = new NetworkMessageReceiver(msgFormat);
        }
        private bool Session_DataReceived(TcpSession session, byte[] data)
        {
            BlinkLed(Leds.Network);

            NetworkMessageReceiver nmr = session.Tag as NetworkMessageReceiver;
            NetworkMessage[] msgs = nmr.Process(data);
            if (msgs != null)
                foreach (NetworkMessage msg in msgs)
                {
                    NetworkMessage response = ProcessNetworkMessage(msg);
                    if (response != null)
                    {
                        session.Send(WSDataFrame.WrapString(response.PackToString(nmr.MessageFormat)));
                        BlinkLed(Leds.Network);
                    }
                }

            return false; // don't disconnect
        }
        private void Session_Disconnected(TcpSession session)
        {
            
        }

        //bool on = false;

        private void timerTest_Tick(Gadgeteer.Timer timer)
        {
            timerTest.Stop();

            //GHI.Premium.Hardware.LowLevel.Watchdog.ResetCounter();

            SetLed(Leds.Test, !GetLed(Leds.Test));

            UIManager.DebugPage.Text = ethernetENC28.Interface.NetworkInterface.IPAddress + "\n" + TimeManager.CurrentTime.ToString();


            //(busHubI2C.BusControlLines[1] as ControlLine).SetState(new byte[] { (byte)(on ? 1 : 0), 0 });
            //on = !on;

            //SetState();
            //SendStateToClients();


            timerTest.Start();
        }
        //private void busHubI2C_BusModulesCollectionChanged(ArrayList addressesAdded, ArrayList addressesRemoved)
        //{
        //    //new Thread(() => { 
        //    //    uint x = 10;
        //    //    uint indent = 10;
        //    //    uint y = 10;
        //    //    uint lineHight = 15;
        //    //    Font fontTitle = Resources.GetFont(Resources.FontResources.NinaB);
        //    //    Font font = Resources.GetFont(Resources.FontResources.small);
        //    //    GT.Color color = GT.Color.FromRGB(101, 156, 239); // cornflower blue

        //    //    y = 10;
        //    //    display.SimpleGraphics.Clear();
        //    //    //display.SimpleGraphics.DisplayText("****************************", fontTitle, color, x, y); y += lineHight;
        //    //    display.SimpleGraphics.DisplayText(busHubI2C.ProductName, fontTitle, color, x, y); y += lineHight;

        //    //    foreach (BusModule busModule in busHubI2C.BusModules)
        //    //    {
        //    //        if (y > display.Height)
        //    //            return;

        //    //        display.SimpleGraphics.DisplayText("[" + busModule.Address + "]   " + busModule.ProductName, fontTitle, color, x+5, y); y += lineHight;

        //    //        foreach (ControlLine controlLine in busModule.ControlLines)
        //    //        {
        //    //            if (y > display.Height)
        //    //                return;

        //    //            string state = "[" + controlLine.State[0] + "][" + controlLine.State[1] + "]";
        //    //            display.SimpleGraphics.DisplayText(controlLine.ProductName + ": " + state, font, color, x + indent, y); y += lineHight;
        //    //        }

        //    //        display.SimpleGraphics.DisplayText("****************************", fontTitle, color, x, y); y += lineHight;
        //    //    }
        //    //}).Start();
        //}
        #endregion

        #region Clients messages processing
        private NetworkMessage ProcessNetworkMessage(NetworkMessage msg)
        {
            NetworkMessage response = null;

            if (msg != null)
            {
                switch (msg.ID)
                {

                    #region Version
                    case "Version": response = GetVersionMessage(); break;
                    #endregion



                    default: break;
                }
            }

            return response;
        }


        private NetworkMessage GetVersionMessage()
        {
            NetworkMessage msg = new NetworkMessage("Version");
            msg["Version"] = Version;
            return msg;
        }

        #endregion
    }
}
