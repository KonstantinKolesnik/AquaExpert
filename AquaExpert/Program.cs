using AquaExpert.Sensors;
using GHI.Premium.Hardware;
using MFE.Net.Http;
using MFE.Net.Managers;
using MFE.Net.Messaging;
using MFE.Net.Tcp;
using MFE.Net.Udp;
using MFE.Net.WebSocket;
using Microsoft.SPOT;
using Microsoft.SPOT.Time;
using System;
using System.Collections;
using System.Net;
using System.Reflection;
using System.Threading;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.Seeed;
using Gadgeteer.Modules.LoveElectronics;
using MFE.Hardware;
using Gadgeteer;
using Gadgeteer.Interfaces;

namespace AquaExpert
{
    public partial class Program
    {
        #region fields
        private INetworkManager networkManager;
        private Gadgeteer.Timer timerNetworkConnect;
        private HttpServer httpServer;
        private WSServer wsServer;
        //private TcpServer tcpServer;
        private NetworkMessageFormat msgFormat = NetworkMessageFormat.Text;

        //private WaterLevelSensor sensorWaterMax;
        //private WaterLevelSensor sensorWaterMin;
        //private PHTempSensor sensorPHTemp;

        private State state = new State();
        private Settings settings = null;

        private int relayWaterIn = 0, ledWaterIn = 0;
        private int relayWaterOut = 1, ledWaterOut = 1;
        private int relayLight = 2, ledLight = 2;
        private int relayHeater = 3, ledHeater = 3;
        private int relayCO2 = 4, ledCO2 = 4;

        private int ledNetwork = 5;

        private Gadgeteer.Timer timerWorkflow;
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
        public State State
        {
            get { return state; }
        }
        public Settings Settings
        {
            get { return settings; }
        }
        #endregion

        private void ProgramStarted()
        {
            InitSettings();
            InitHardware();
            //InitTimeService();
            //InitNetwork();

            Mainboard.SetDebugLED(true);
        }

        #region Private methods
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
        private void InitHardware()
        {
            indicators.TurnAllLedsOff();


            //Watchdog

            //ArrayList res = I2CExtension.Scan(0, 127, 400);

            //relays[relayWaterIn] = false;
            //relays[relayWaterOut] = false;
            //relays[relayLight] = false;
            //relays[relayHeater] = false;

            timerNetworkConnect = new Gadgeteer.Timer(500);
            timerNetworkConnect.Tick += delegate(Gadgeteer.Timer t) { indicators[ledNetwork] = !indicators[ledNetwork]; };

            timerWorkflow = new Gadgeteer.Timer(500);
            timerWorkflow.Tick += timerWorkflow_Tick;

            //sensorWaterMax = new WaterLevelSensor(moistureSensorUpper);
            //sensorPHTemp = new PHTempSensor(pHTempSensor);
        }
        private void InitNetwork()
        {
            //discoveryListener = new DiscoveryListener();

            //tcpServer = new TcpServer(Settings.IPPort);
            //tcpServer.SessionConnected += Session_Connected;
            //tcpServer.SessionDataReceived += Session_DataReceived;
            //tcpServer.SessionDisconnected += Session_Disconnected;

            wsServer = new WSServer(Settings.WSPort);
            wsServer.SessionConnected += Session_Connected;
            wsServer.SessionDataReceived += Session_DataReceived;
            wsServer.SessionDisconnected += Session_Disconnected;

            httpServer = new HttpServer();
            httpServer.OnRequest += httpServer_OnRequest;
            httpServer.OnGetRequest += httpServer_OnGetRequest;
            httpServer.OnResponse += httpServer_OnResponse;

            networkManager = new GadgeteerWiFiManager(wifi_RS21, settings.WiFiSSID, settings.WiFiPassword);
            networkManager.Started += new EventHandler(Network_Started);
            networkManager.Stopped += new EventHandler(Network_Stopped);

            StartNetwork();
        }
        private void StartNetwork()
        {
            timerNetworkConnect.Start();
            new Thread(() => { networkManager.Start(); }).Start();
        }
        private void InitTimeService()
        {
            FixedTimeService.Settings = new TimeServiceSettings()
            {
                AutoDayLightSavings = true,
                ForceSyncAtWakeUp = true,
                RefreshTime = 24 * 60 * 60, // once a day
            };
            FixedTimeService.SetTimeZoneOffset(+2 * 60); /// PST
            FixedTimeService.SetDst("Mar Sun>=8 @2", "Nov Sun>=1 @2", 60); // US DST

            FixedTimeService.SystemTimeChanged += new SystemTimeChangedEventHandler(TimeService_SystemTimeChanged);
            FixedTimeService.TimeSyncFailed += new TimeSyncFailedEventHandler(TimeService_TimeSyncFailed);
            // New event: called when after we check the time (even if we don't end up changing the time)
            FixedTimeService.SystemTimeChecked += new SystemTimeChangedEventHandler(TimeService_SystemTimeChecked);
        }
        private void SyncTime()
        {
            new Thread(() =>
            {
                // "nist1-sj.ustiming.org,us.pool.ntp.org,clock.tricity.wsu.edu,clock-1.cs.cmu.edu,time-a.nist.gov"
                FixedTimeService.Settings.PrimaryServer = Dns.GetHostEntry("nist1-sj.ustiming.org").AddressList[0].GetAddressBytes();
                FixedTimeService.Settings.AlternateServer = Dns.GetHostEntry("pool.ntp.org").AddressList[0].GetAddressBytes();

                // wait for internet connection
                while (IPAddress.GetDefaultLocalAddress() == IPAddress.Any)
                    Thread.Sleep(500);

                FixedTimeService.Start();
            }).Start();
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
        private void DoWork()
        {
            //relays[relayWaterIn] = state.IsWaterInMode;
            //relays[relayWaterOut] = state.IsWaterOutMode;
            //relays[relayLight] = state.IsLightOn;
            //relays[relayHeater] = state.IsHeaterOn;
            //relays[relayCO2] = state.IsCO2On;

            //// double with indicators:
            //indicators[ledWaterIn] = state.IsWaterInMode;
            //indicators[ledWaterOut] = state.IsWaterOutMode;
            //indicators[ledLight] = state.IsLightOn;
            //indicators[ledHeater] = state.IsHeaterOn;
            //indicators[ledCO2] = state.IsCO2On;
        }
        private void SendStateToClients()
        {
            NetworkMessage msg = new NetworkMessage("State");
            msg["TimeStamp"] = DateTime.Now.ToString();
            msg["Version"] = Version;
            byte[] data = msg.Pack(msgFormat);

            wsServer.SendToAll(data, 0, data.Length);
            //tcpServer.SendToAll(data);
        }

        private void BlinkLED(int led)
        {
            new Thread(() =>
            {
                indicators[led] = false;
                Thread.Sleep(20);
                indicators[led] = true;
            }).Start();
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
            //HWConfig.WiFi.NetworkSettings.IPAddress
            //HWConfig.Ethernet.Interface.NetworkInterface.IPAddress

            timerNetworkConnect.Stop();
            indicators[ledNetwork] = true;

            httpServer.Start("http", 80);
            wsServer.Start();
            //tcpServer.Start();
            SyncTime();

            //discoveryListener.Start(Options.UDPPort, "TyphoonCentralStation");
        }
        private void Network_Stopped(object sender, EventArgs e)
        {
            indicators[ledNetwork] = false;

            httpServer.Stop();
            wsServer.Stop();
            //tcpServer.Stop();

            Thread.Sleep(1000);

            StartNetwork();
        }

        private void TimeService_SystemTimeChanged(object sender, SystemTimeChangedEventArgs e)
        {
            RealTimeClock.SetTime(e.EventTime);
            timerWorkflow.Start();
        }
        private void TimeService_TimeSyncFailed(object sender, TimeSyncFailedEventArgs e)
        {
        }
        private void TimeService_SystemTimeChecked(object sender, SystemTimeChangedEventArgs e)
        {
            //RealTimeClock.SetTime(e.EventTime);
            //timerWorkflow.Start();
        }

        private void httpServer_OnRequest(HttpListenerRequest request)
        {
            BlinkLED(ledNetwork);
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
            BlinkLED(ledNetwork);
        }

        private void Session_Connected(TcpSession session)
        {
            session.Tag = new NetworkMessageReceiver(msgFormat);
        }
        private bool Session_DataReceived(TcpSession session, byte[] data)
        {
            BlinkLED(ledNetwork);

            NetworkMessageReceiver nmr = session.Tag as NetworkMessageReceiver;
            NetworkMessage[] msgs = nmr.Process(data);
            if (msgs != null)
                foreach (NetworkMessage msg in msgs)
                {
                    NetworkMessage response = ProcessNetworkMessage(msg);
                    if (response != null)
                    {
                        session.Send(WSDataFrame.WrapString(response.PackToString(nmr.MessageFormat)));
                        BlinkLED(ledNetwork);
                    }
                }

            return false; // don't disconnect
        }
        private void Session_Disconnected(TcpSession session)
        {
            
        }

        private void timerWorkflow_Tick(Gadgeteer.Timer timer)
        {
            //SetState();
            //DoWork();
            //SendStateToClients();
        }
        #endregion

        #region Network message processing
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
