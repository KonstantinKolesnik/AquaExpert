using AquaExpert.Managers;
using Gadgeteer;
using GTM.MFE.Displays;
using MFE.Net.Http;
using MFE.Net.Managers;
using MFE.Net.Messaging;
using MFE.Net.Tcp;
using MFE.Net.WebSocket;
using Microsoft.SPOT;
using System.Collections;
using System.Net;
using System.Reflection;
using System.Threading;
using Gadgeteer.Modules.GHIElectronics;
using System;

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

        private Display display;

        //private WaterLevelSensor sensorWaterMax;
        //private WaterLevelSensor sensorWaterMin;
        //private PHTempSensor sensorPHTemp;

        private State state = new State();
        private Settings settings = null;

        private int ledNetwork = 0;

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
            InitDisplay();

            //if (!Utils.StringIsNullOrEmpty(Settings.WiFiSSID))
            //    InitNetwork();

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

            //Watchdog!!!

            timerNetworkConnect = new Gadgeteer.Timer(500);
            timerNetworkConnect.Tick += delegate(Gadgeteer.Timer t) { indicators[ledNetwork] = !indicators[ledNetwork]; };

            timerWorkflow = new Gadgeteer.Timer(1000);
            timerWorkflow.Tick += timerWorkflow_Tick;
            timerWorkflow.Start();
        }
        private void InitDisplay()
        {
            display = new Display(ModelType.TFT01_22SP, 1);

            // Usage example #1. Passing a Bitmap to the driver.
            Bitmap picture = new Bitmap(Resources.GetBytes(Resources.BinaryResources.test_24b), Bitmap.BitmapImageType.Bmp);
            //display.Draw(picture);

            //display.SimpleGraphics.BackgroundColor = Gadgeteer.Color.Green;
            //display.SimpleGraphics.DisplayText("Hello World!", Resources.GetFont(Resources.FontResources.NinaB), Gadgeteer.Color.Red, 5, 5);
            //display.SimpleGraphics.DisplayText("Kotyara!", Resources.GetFont(Resources.FontResources.small), Gadgeteer.Color.Red, 5, 25);
            //Thread.Sleep(2000);
            //display.SimpleGraphics.Clear();
            //display.SimpleGraphics.DisplayEllipse(Gadgeteer.Color.Blue, 60, 120, 30, 20);
            //Thread.Sleep(2000);
            //display.SimpleGraphics.DisplayImage(picture, 0, 0);

            //loop(display);




        }
        //void loop(Display display)
        //{
        //    Gadgeteer.Modules.Module.DisplayModule.SimpleGraphicsInterface graphics = display.SimpleGraphics;
        //  int buf[318];
        //  int x, x2;
        //  int y, y2;
        //  int r;

        //// Clear the screen and draw the frame
        //  graphics.Clear();

        //  graphics.setColor(255, 0, 0);
        //  graphics.fillRect(0, 0, 319, 13);
        //  graphics.setColor(64, 64, 64);
        //  graphics.fillRect(0, 226, 319, 239);
        //  graphics.setColor(255, 255, 255);
        //  graphics.setBackColor(255, 0, 0);
        //  graphics.print("* Universal Color TFT Display Library *", CENTER, 1);
        //  graphics.setBackColor(64, 64, 64);
        //  graphics.setColor(255,255,0);
        //  graphics.print("<http://electronics.henningkarlsen.com>", CENTER, 227);

        //  graphics.setColor(0, 0, 255);
        //  graphics.drawRect(0, 14, 319, 225);

        //// Draw crosshairs
        //  graphics.setColor(0, 0, 255);
        //  graphics.setBackColor(0, 0, 0);
        //  graphics.drawLine(159, 15, 159, 224);
        //  graphics.drawLine(1, 119, 318, 119);
        //  for (int i=9; i<310; i+=10)
        //    graphics.drawLine(i, 117, i, 121);
        //  for (int i=19; i<220; i+=10)
        //    graphics.drawLine(157, i, 161, i);

        //// Draw sin-, cos- and tan-lines  
        //  graphics.setColor(0,255,255);
        //  graphics.print("Sin", 5, 15);
        //  for (int i=1; i<318; i++)
        //  {
        //    graphics.drawPixel(i,119+(sin(((i*1.13)*3.14)/180)*95));
        //  }
  
        //  graphics.setColor(255,0,0);
        //  graphics.print("Cos", 5, 27);
        //  for (int i=1; i<318; i++)
        //  {
        //    graphics.drawPixel(i,119+(cos(((i*1.13)*3.14)/180)*95));
        //  }

        //  graphics.setColor(255,255,0);
        //  graphics.print("Tan", 5, 39);
        //  for (int i=1; i<318; i++)
        //  {
        //    graphics.drawPixel(i,119+(tan(((i*1.13)*3.14)/180)));
        //  }

        //  delay(2000);

        //  graphics.setColor(0,0,0);
        //  graphics.fillRect(1,15,318,224);
        //  graphics.setColor(0, 0, 255);
        //  graphics.setBackColor(0, 0, 0);
        //  graphics.drawLine(159, 15, 159, 224);
        //  graphics.drawLine(1, 119, 318, 119);

        //// Draw a moving sinewave
        //  x=1;
        //  for (int i=1; i<(318*20); i++) 
        //  {
        //    x++;
        //    if (x==319)
        //      x=1;
        //    if (i>319)
        //    {
        //      if ((x==159)||(buf[x-1]==119))
        //        graphics.setColor(0,0,255);
        //      else
        //        graphics.setColor(0,0,0);
        //      graphics.drawPixel(x,buf[x-1]);
        //    }
        //    graphics.setColor(0,255,255);
        //    y=119+(sin(((i*1.1)*3.14)/180)*(90-(i / 100)));
        //    graphics.drawPixel(x,y);
        //    buf[x-1]=y;
        //  }

        //  delay(2000);
  
        //  graphics.setColor(0,0,0);
        //  graphics.fillRect(1,15,318,224);

        //// Draw some filled rectangles
        //  for (int i=1; i<6; i++)
        //  {
        //    switch (i)
        //    {
        //      case 1:
        //        graphics.setColor(255,0,255);
        //        break;
        //      case 2:
        //        graphics.setColor(255,0,0);
        //        break;
        //      case 3:
        //        graphics.setColor(0,255,0);
        //        break;
        //      case 4:
        //        graphics.setColor(0,0,255);
        //        break;
        //      case 5:
        //        graphics.setColor(255,255,0);
        //        break;
        //    }
        //    graphics.fillRect(70+(i*20), 30+(i*20), 130+(i*20), 90+(i*20));
        //  }

        //  delay(2000);
  
        //  graphics.setColor(0,0,0);
        //  graphics.fillRect(1,15,318,224);

        //// Draw some filled, rounded rectangles
        //  for (int i=1; i<6; i++)
        //  {
        //    switch (i)
        //    {
        //      case 1:
        //        graphics.setColor(255,0,255);
        //        break;
        //      case 2:
        //        graphics.setColor(255,0,0);
        //        break;
        //      case 3:
        //        graphics.setColor(0,255,0);
        //        break;
        //      case 4:
        //        graphics.setColor(0,0,255);
        //        break;
        //      case 5:
        //        graphics.setColor(255,255,0);
        //        break;
        //    }
        //    graphics.fillRoundRect(190-(i*20), 30+(i*20), 250-(i*20), 90+(i*20));
        //  }
  
        //  delay(2000);
  
        //  graphics.setColor(0,0,0);
        //  graphics.fillRect(1,15,318,224);

        //// Draw some filled circles
        //  for (int i=1; i<6; i++)
        //  {
        //    switch (i)
        //    {
        //      case 1:
        //        graphics.setColor(255,0,255);
        //        break;
        //      case 2:
        //        graphics.setColor(255,0,0);
        //        break;
        //      case 3:
        //        graphics.setColor(0,255,0);
        //        break;
        //      case 4:
        //        graphics.setColor(0,0,255);
        //        break;
        //      case 5:
        //        graphics.setColor(255,255,0);
        //        break;
        //    }
        //    graphics.fillCircle(100+(i*20),60+(i*20), 30);
        //  }
  
        //  delay(2000);
  
        //  graphics.setColor(0,0,0);
        //  graphics.fillRect(1,15,318,224);

        //// Draw some lines in a pattern
        //  graphics.setColor (255,0,0);
        //  for (int i=15; i<224; i+=5)
        //  {
        //    graphics.drawLine(1, i, (i*1.44)-10, 224);
        //  }
        //  graphics.setColor (255,0,0);
        //  for (int i=224; i>15; i-=5)
        //  {
        //    graphics.drawLine(318, i, (i*1.44)-11, 15);
        //  }
        //  graphics.setColor (0,255,255);
        //  for (int i=224; i>15; i-=5)
        //  {
        //    graphics.drawLine(1, i, 331-(i*1.44), 15);
        //  }
        //  graphics.setColor (0,255,255);
        //  for (int i=15; i<224; i+=5)
        //  {
        //    graphics.drawLine(318, i, 330-(i*1.44), 224);
        //  }
  
        //  delay(2000);
  
        //  graphics.setColor(0,0,0);
        //  graphics.fillRect(1,15,318,224);

        //// Draw some random circles
        //  for (int i=0; i<100; i++)
        //  {
        //    graphics.setColor(random(255), random(255), random(255));
        //    x=32+random(256);
        //    y=45+random(146);
        //    r=random(30);
        //    graphics.drawCircle(x, y, r);
        //  }

        //  delay(2000);
  
        //  graphics.setColor(0,0,0);
        //  graphics.fillRect(1,15,318,224);

        //// Draw some random rectangles
        //  for (int i=0; i<100; i++)
        //  {
        //    graphics.setColor(random(255), random(255), random(255));
        //    x=2+random(316);
        //    y=16+random(207);
        //    x2=2+random(316);
        //    y2=16+random(207);
        //    graphics.drawRect(x, y, x2, y2);
        //  }

        //  delay(2000);
  
        //  graphics.setColor(0,0,0);
        //  graphics.fillRect(1,15,318,224);

        //// Draw some random rounded rectangles
        //  for (int i=0; i<100; i++)
        //  {
        //    graphics.setColor(random(255), random(255), random(255));
        //    x=2+random(316);
        //    y=16+random(207);
        //    x2=2+random(316);
        //    y2=16+random(207);
        //    graphics.drawRoundRect(x, y, x2, y2);
        //  }

        //  delay(2000);
  
        //  graphics.setColor(0,0,0);
        //  graphics.fillRect(1,15,318,224);

        //  for (int i=0; i<100; i++)
        //  {
        //    graphics.setColor(random(255), random(255), random(255));
        //    x=2+random(316);
        //    y=16+random(209);
        //    x2=2+random(316);
        //    y2=16+random(209);
        //    graphics.drawLine(x, y, x2, y2);
        //  }

        //  delay(2000);
  
        //  graphics.setColor(0,0,0);
        //  graphics.fillRect(1,15,318,224);

        //  for (int i=0; i<10000; i++)
        //  {
        //    graphics.setColor(random(255), random(255), random(255));
        //    graphics.drawPixel(2+random(316), 16+random(209));
        //  }

        //  delay(2000);

        //  graphics.fillScr(0, 0, 255);
        //  graphics.setColor(255, 0, 0);
        //  graphics.fillRoundRect(80, 70, 239, 169);
  
        //  graphics.setColor(255, 255, 255);
        //  graphics.setBackColor(255, 0, 0);
        //  graphics.print("That's it!", CENTER, 93);
        //  graphics.print("Restarting in a", CENTER, 119);
        //  graphics.print("few seconds...", CENTER, 132);
  
        //  graphics.setColor(0, 255, 0);
        //  graphics.setBackColor(0, 0, 255);
        //  graphics.print("Runtime: (msecs)", CENTER, 210);
        //  graphics.printNumI(millis(), CENTER, 225);
  
        //  delay (10000);
        //}

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
            //NetworkMessage msg = new NetworkMessage("State");
            //msg["TimeStamp"] = DateTime.Now.ToString();
            //msg["Version"] = Version;
            //byte[] data = msg.Pack(msgFormat);

            //wsServer.SendToAll(data, 0, data.Length);
            ////tcpServer.SendToAll(data);
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
            TimeManager.Start();

            //discoveryListener.Start(Options.UDPPort, "TyphoonCentralStation");
        }
        private void Network_Stopped(object sender, EventArgs e)
        {
            indicators[ledNetwork] = false;

            httpServer.Stop();
            wsServer.Stop();
            //tcpServer.Stop();
            TimeManager.Stop();

            Thread.Sleep(1000);

            StartNetwork();
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
            timerWorkflow.Stop();

            //DateTime dt = TimeManager.CurrentTime;

            ModulesManager.Scan();

            //SetState();
            //DoWork();
            //SendStateToClients();


            timerWorkflow.Start();
        }
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
