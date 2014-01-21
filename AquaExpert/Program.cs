using AquaExpert.Sensors;
using GHI.Premium.Hardware;
using MFE.Net.Managers;
using MFE.Net.Udp;
using Microsoft.SPOT;
using Microsoft.SPOT.Time;
using System;
using System.Net;
using System.Threading;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.Seeed;
using Gadgeteer.Modules.LoveElectronics;
using MFE.Net.Http;
using MFE.Net.WebSocket;

namespace AquaExpert
{
    public partial class Program
    {
        #region fields
        private INetworkManager networkManager;
        private Gadgeteer.Timer timerNetworkConnect;
        
        private HttpServer httpServer;
        private WSServer wsServer;

        private WaterLevelSensor sensorWaterMax;
        private WaterLevelSensor sensorWaterMin;
        private PHSensor sensorPH;

        private State State = new State();
        private Settings Settings = null;

        private int relayWaterIn = 0, ledWaterIn = 0;
        private int relayWaterOut = 1, ledWaterOut = 1;
        private int relayLight = 2, ledLight = 2;
        private int relayFeed = 3, ledFeed = 3;
        private int ledNetwork = 4;

        private Gadgeteer.Timer timerWorkflow;
        #endregion

        private void ProgramStarted()
        {
            indicators.TurnAllLedsOff();

            timerNetworkConnect = new Gadgeteer.Timer(500);
            timerNetworkConnect.Tick += delegate(Gadgeteer.Timer t) { indicators[ledNetwork] = !indicators[ledNetwork]; };

            timerWorkflow = new Gadgeteer.Timer(500);
            timerWorkflow.Tick += timerWorkflow_Tick;

            sensorWaterMax = new WaterLevelSensor(moistureSensorUpper);

            Settings = Settings.LoadFromFlash(0);

            InitNetwork();

            Mainboard.SetDebugLED(true);
        }

        #region Private methods
        private void InitNetwork()
        {
            //discoveryListener = new DiscoveryListener();

            //tcpServer = new TcpServer(Options.IPPort);
            //tcpServer.SessionConnected += Session_Connected;
            //tcpServer.SessionDataReceived += Session_DataReceived;
            //tcpServer.SessionDisconnected += Session_Disconnected;

            wsServer = new WSServer(Settings.WSPort);
            //wsServer.SessionConnected += Session_Connected;
            //wsServer.SessionDataReceived += Session_DataReceived;
            //wsServer.SessionDisconnected += Session_Disconnected;

            httpServer = new HttpServer();
            //httpServer.OnGetRequest += httpServer_OnGetRequest;
            //httpServer.OnResponse += httpServer_OnResponse;

            //if (options.UseWiFi)
            networkManager = new GadgeteerWiFiManager(wifi_RS21, Settings.WiFiSSID, Settings.WiFiPassword);
            //else
            //networkManager = new GadgeteerEthernetManager(ethernet_ENC28);

            networkManager.Started += new EventHandler(Network_Started);
            networkManager.Stopped += new EventHandler(Network_Stopped);

            StartNetwork();
        }
        private void StartNetwork()
        {
            timerNetworkConnect.Start();
            new Thread(delegate
            {
                networkManager.Start();
            }).Start();
        }
        private void InitTimeService()
        {
            FixedTimeService.Settings = new TimeServiceSettings()
            {
                AutoDayLightSavings = true,
                ForceSyncAtWakeUp = true,
                // "nist1-sj.ustiming.org,us.pool.ntp.org,clock.tricity.wsu.edu,clock-1.cs.cmu.edu,time-a.nist.gov"
                PrimaryServer = Dns.GetHostEntry("nist1-sj.ustiming.org").AddressList[0].GetAddressBytes(),
                AlternateServer = Dns.GetHostEntry("pool.ntp.org").AddressList[0].GetAddressBytes(),
                RefreshTime = 24 * 60 * 60 // once a day
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
                // wait for internet connection
                while (IPAddress.GetDefaultLocalAddress() == IPAddress.Any)
                    Thread.Sleep(500);

                FixedTimeService.Start();
            }).Start();
        }

        private void PopulateState()
        {
            DateTime dt = RealTimeClock.GetTime();
            //Debug.Print(dt.ToString());

            State.IsLightOn = dt.Hour >= Settings.LightOnHour && dt.Hour < Settings.LightOffHour;

            //relays[relayWaterIn] = !relays[relayWaterIn];

        }
        private void DoWork()
        {
            if (!State.IsWaterOutMode)
            {
                relays[relayWaterIn] = !sensorWaterMax.IsWet;
                relays[relayWaterOut] = false;
            }
            else
            {
                relays[relayWaterIn] = false;
                relays[relayWaterOut] = sensorWaterMin.IsWet;
                if (!sensorWaterMin.IsWet)
                    State.IsWaterOutMode = false;
            }

            relays[relayLight] = State.IsLightOn;
            relays[relayFeed] = State.IsFeedMode;

            indicators[ledWaterIn] = relays[relayWaterIn];
            indicators[ledWaterOut] = relays[relayWaterOut];
            indicators[ledLight] = relays[relayLight];
            indicators[ledFeed] = relays[relayFeed];
        }

        private void ReplaceWater()
        {
            State.IsWaterOutMode = true;
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

            //discoveryListener.Start(Options.UDPPort, "TyphoonCentralStation");

            //NameService ns = new NameService();
            //ns.AddName("TYPHOON", NameService.NameType.Unique, NameService.MsSuffix.Default);

            InitTimeService();
            SyncTime();
        }
        private void Network_Stopped(object sender, EventArgs e)
        {
            indicators[ledNetwork] = false;

            //httpServer.Stop();
            //wsServer.Stop();
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
            RealTimeClock.SetTime(e.EventTime);
            //timerClock.Start();
        }

        private void timerWorkflow_Tick(Gadgeteer.Timer timer)
        {
            PopulateState();
            DoWork();
        }
        #endregion
    }
}
