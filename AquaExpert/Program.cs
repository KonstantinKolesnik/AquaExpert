using MFE.Net.Managers;
using MFE.Net.Udp;
using Microsoft.SPOT;
using Microsoft.SPOT.Time;
using System.Net;
using System.Threading;

namespace AquaExpert
{
    public partial class Program
    {
        private INetworkManager networkManager;
        private WaterLevelSensor wlsMax;
        private WaterLevelSensor wlsMin;

        private int relayWaterIn = 0;
        private int relayWaterOut = 1;


        private void ProgramStarted()
        {
            wlsMax = new WaterLevelSensor(wetnessSensorUpper);
            wlsMax.WetnessChanged += wlSensorUpper_WetnessChanged;

            

            //GT.Timer timer = new GT.Timer(1000);
            //timer.Tick += delegate(GT.Timer t)
            //{
            //    relays[0] = !relays[0];
            //    relays[1] = !relays[1];
            //    relays[2] = !relays[2];
            //    relays[3] = !relays[3];
            //};
            //timer.Start();

            //DateTime dt = RealTimeClock.GetTime();
            //Debug.Print(dt.ToString());

            InitNetwork();



            Mainboard.SetDebugLED(true);
        }

        private void InitNetwork()
        {
            //discoveryListener = new DiscoveryListener();

            //tcpServer = new TcpServer(Options.IPPort);
            //tcpServer.SessionConnected += Session_Connected;
            //tcpServer.SessionDataReceived += Session_DataReceived;
            //tcpServer.SessionDisconnected += Session_Disconnected;

            //wsServer = new WSServer(Options.WSPort);
            //wsServer.SessionConnected += Session_Connected;
            //wsServer.SessionDataReceived += Session_DataReceived;
            //wsServer.SessionDisconnected += Session_Disconnected;

            //httpServer = new HttpServer();
            //httpServer.OnGetRequest += httpServer_OnGetRequest;
            //httpServer.OnResponse += httpServer_OnResponse;

            //if (options.UseWiFi)
            //networkManager = new GadgeteerWiFiManager(HWConfig.WiFi, options.WiFiSSID, options.WiFiPassword);//, GT.Socket.GetSocket(18, true, null, null).PWM9);
            //else
            networkManager = new GadgeteerEthernetManager(ethernet_ENC28);

            networkManager.Started += new EventHandler(Network_Started);
            networkManager.Stopped += new EventHandler(Network_Stopped);

            StartNetwork();
        }
        private void StartNetwork()
        {
            //timerNetworkConnect.Start();
            new Thread(delegate
            {
                networkManager.Start();
            }).Start();
        }
        private void InitTimeService()
        {
            // setup callbacks
            FixedTimeService.SystemTimeChanged += new SystemTimeChangedEventHandler(TimeService_SystemTimeChanged);
            FixedTimeService.TimeSyncFailed += new TimeSyncFailedEventHandler(TimeService_TimeSyncFailed);
            // New event: called when after we check the time (even if we don't end up changing the time)
            FixedTimeService.SystemTimeChecked += new SystemTimeChangedEventHandler(TimeService_SystemTimeChecked);

            // start thread to init TimeService
            Thread initTimeserviceThread = new Thread(() => InitTimeservice2());
            initTimeserviceThread.Start();
        }
        void InitTimeservice2()
        {
            Thread.Sleep(1);  // let ProgramStarted finish before doing this

            // wait for internet connection
            while (IPAddress.GetDefaultLocalAddress() == IPAddress.Any)
                Thread.Sleep(500);

            // configure & start FixedTimeService
            FixedTimeService.Settings = new TimeServiceSettings()
            {
                AutoDayLightSavings = true,
                ForceSyncAtWakeUp = true,
                // "nist1-sj.ustiming.org,us.pool.ntp.org,clock.tricity.wsu.edu,clock-1.cs.cmu.edu,time-a.nist.gov"
                PrimaryServer = Dns.GetHostEntry("nist1-sj.ustiming.org").AddressList[0].GetAddressBytes(),
                AlternateServer = Dns.GetHostEntry("pool.ntp.org").AddressList[0].GetAddressBytes(),
                RefreshTime = 24 * 60 * 60 // once a day
            };
            FixedTimeService.SetTimeZoneOffset(-8 * 60); /// PST
            FixedTimeService.SetDst("Mar Sun>=8 @2", "Nov Sun>=1 @2", 60); // US DST
            FixedTimeService.Start();
        }

        #region Event handlers
        private void Network_Started(object sender, EventArgs e)
        {
            //HWConfig.WiFi.NetworkSettings.IPAddress
            //HWConfig.Ethernet.Interface.NetworkInterface.IPAddress

            //timerNetworkConnect.Stop();
            //HWConfig.Indicators[HWConfig.LEDNetwork] = true;

            //httpServer.Start("http", 80);
            //wsServer.Start();
            //tcpServer.Start();

            //discoveryListener.Start(Options.UDPPort, "TyphoonCentralStation");

            //NameService ns = new NameService();
            //ns.AddName("TYPHOON", NameService.NameType.Unique, NameService.MsSuffix.Default);

            InitTimeService();
        }
        private void Network_Stopped(object sender, EventArgs e)
        {
            //httpServer.Stop();
            //wsServer.Stop();
            //tcpServer.Stop();

            Thread.Sleep(1000);

            StartNetwork();
        }

        private void TimeService_SystemTimeChanged(object sender, SystemTimeChangedEventArgs e)
        {

        }
        private void TimeService_TimeSyncFailed(object sender, TimeSyncFailedEventArgs e)
        {

        }
        private void TimeService_SystemTimeChecked(object sender, SystemTimeChangedEventArgs e)
        {

        }

        private void wlSensorUpper_WetnessChanged(object sender, EventArgs e)
        {
            relays[relayWaterIn] = !wlsMax.IsWet;
        }
        #endregion
    }
}
