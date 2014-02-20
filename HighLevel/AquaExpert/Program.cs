using Gadgeteer.Modules.KKS;
using Gadgeteer.Modules.KKS.NRF24L01Plus;
using GHI.Premium.System;
using MFE.Graphics;
using MFE.Graphics.Controls;
using MFE.Graphics.Geometry;
using MFE.Graphics.Media;
using MFE.Net.Http;
using MFE.Net.Managers;
using MFE.Net.Messaging;
using MFE.Net.Tcp;
using MFE.Net.WebSocket;
using Microsoft.SPOT;
using SmartNetwork.Network;
using System;
using System.Collections;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using GT = Gadgeteer;

namespace AquaExpert
{
    public partial class Program
    {
        #region Fields
        private INetworkManager networkManager;
        private GT.Timer timerNetworkConnect;
        private HttpServer httpServer;
        private WSServer wsServer;
        //private TcpServer tcpServer;
        private NetworkMessageFormat msgFormat = NetworkMessageFormat.Text;

        private State state = new State();
        private Settings settings = null;

        private int ledNetwork = 0;

        private NetworkCoordinator networkCoordinator;
        private BusHubI2C busHubI2C;

        private GT.Timer timerTest;

        private GraphicsManager gm;
        private DisplayS22 display;

        private NRF24 nrf24;
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
            InitSettings();
            InitRF();
            InitBus();
            InitHardware();
            //InitDisplay();
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
        private void InitRF()
        {
            const byte channel = 10;

            nrf24 = new NRF24(1);

            //nrf24.OnDataReceived += nrf24_Receive;
            //nrf24.OnTransmitFailed += nrf24_OnSendFailure;
            //nrf24.OnTransmitSuccess += nrf24_OnSendSuccess;

            Debug.Print(nrf24.GetStatus().ToString());

            // we need to call Configure() befeore we start using the module
            //nrf24.Configure(Encoding.UTF8.GetBytes("COORD"), channel);

            // to start receiveing we need to call Enable(), call Disable() to stop/pause
            nrf24.Enable();

            // example of reading your own address
            //var myAddress = nrf24.GetAddress(AddressSlot.Zero, 5);
            //Debug.Print("I am " + new string(Encoding.UTF8.GetChars(myAddress)));
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
            //Watchdog!!!

            indicators.TurnAllLedsOff();

            timerNetworkConnect = new Gadgeteer.Timer(500);
            timerNetworkConnect.Tick += delegate(Gadgeteer.Timer t) { indicators[ledNetwork] = !indicators[ledNetwork]; };

            timerTest = new Gadgeteer.Timer(800);
            timerTest.Tick += timerTest_Tick;
            //timerTest.Start();
        }
        
        private void InitDisplay()
        {
            if (Mainboard.NativeBitmapConverter == null)
                Mainboard.NativeBitmapConverter = new Gadgeteer.Mainboard.BitmapConvertBPP(delegate(byte[] bitmapBytes, byte[] pixelBytes, GT.Mainboard.BPP bpp)
                {
                    if (bpp != GT.Mainboard.BPP.BPP16_BGR_BE)
                        throw new ArgumentOutOfRangeException("bpp", "Only BPP16_BGR_LE supported");

                    Util.BitmapConvertBPP(bitmapBytes, pixelBytes, Util.BPP_Type.BPP16_BGR_BE);
                });

            display = new DisplayS22(1);

            // Usage example #1. Passing a Bitmap to the driver.
            //Bitmap bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.test_24b), Bitmap.BitmapImageType.Bmp);
            //display.Draw(bitmap);

            //display.SimpleGraphics.DisplayImage(bitmap, 0, 0);
            ////display.SimpleGraphics.BackgroundColor = GT.Color.Green;
            //display.SimpleGraphics.DisplayText("Igor, mi bogati!", Resources.GetFont(Resources.FontResources.NinaB), GT.Color.Red, 5, 5);
            //display.SimpleGraphics.DisplayText("Pivo v studiyu!", Resources.GetFont(Resources.FontResources.small), GT.Color.Red, 5, 25);
            ////Thread.Sleep(2000);
            ////display.SimpleGraphics.Clear();
            //display.SimpleGraphics.DisplayEllipse(GT.Color.Blue, 120, 160, 30, 20);
            //Thread.Sleep(2000);

            //DisplayDemo(display);

            gm = new GraphicsManager(240, 320);
            gm.OnRender += delegate(Bitmap bitmap, Rect dirtyArea)
            {
                display.SimpleGraphics.DisplayImage(bitmap, (uint)dirtyArea.X, (uint)dirtyArea.Y, (uint)dirtyArea.X, (uint)dirtyArea.Y, (uint)dirtyArea.Width, (uint)dirtyArea.Height);
            };

            //CrashTest();
            //UIDemo();
        }
        private void DisplayDemo(DisplayS22 display)
        {
            GT.Modules.Module.DisplayModule.SimpleGraphicsInterface graphics = display.SimpleGraphics;
            //int buf[318];
            //int x, x2;
            //int y, y2;
            //int r;

            // Clear the screen and draw the frame
            graphics.Clear();

            graphics.BackgroundColor = GT.Color.Red;
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
            graphics.BackgroundColor = GT.Color.Black;
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

            Thread.Sleep(2000);

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

            Thread.Sleep(2000);
  
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

            Thread.Sleep(2000);
  
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

            Thread.Sleep(2000);
  
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

            Thread.Sleep(2000);
  
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

            Thread.Sleep(2000);
  
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

            Thread.Sleep(2000);
  
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

            Thread.Sleep(2000);
  
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

            Thread.Sleep(2000);
  
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

            Thread.Sleep(2000);
  
        //  graphics.setColor(0,0,0);
        //  graphics.fillRect(1,15,318,224);

        //  for (int i=0; i<10000; i++)
        //  {
        //    graphics.setColor(random(255), random(255), random(255));
        //    graphics.drawPixel(2+random(316), 16+random(209));
        //  }

            Thread.Sleep(2000);

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

            Thread.Sleep(10000);
        }
        private void CrashTest()
        {
            //Width = 800;
            //Height = 480;

            //Width = 250;
            //Height = 480;

            font = Resources.GetFont(Resources.FontResources.small);

            display.SimpleGraphics.Clear();

            Panel pnl = new Panel(0, 0, 150, 180);
            //pnl.Background = new LinearGradientBrush(Color.Blue, Color.Red);
            pnl.Background = new SolidColorBrush(Color.Blue);

            gm.Desktop.SuspendLayout();
            for (int i = 0; i < 10; i++)
                pnl.Children.Add(new TextBlock(10, 15 * i, 140, 20, font, "label" + i.ToString()) { ForeColor = Color.Brown });
            gm.Desktop.Children.Add(pnl);
            gm.Desktop.ResumeLayout();

            new Thread(delegate()
            {
                //int n = 0;
                while (true)
                {
                    //Debug.Print("--------- Enter loop");

                    DateTime dt = DateTime.Now;
                    int c = pnl.Children.Count;
                    for (int j = 0; j < c; j++)
                    {
                        //dt0 = DateTime.Now;



                        //Label lbl = (Label)Children.GetAt(j);
                        TextBlock lbl = (TextBlock)pnl.Children[j];
                        //Debug.Print("Label text set");
                        lbl.Text = dt.Ticks.ToString();
                        //Thread.Sleep(10);

                        //ts = DateTime.Now - dt0;
                        //n++;
                        //sum += ts;

                        //Debug.Print("MFE: " + ts.ToString());
                        //Debug.Print("MFE (average): " + new TimeSpan(sum.Ticks / n).ToString());

                    }
                    //Thread.Sleep(1000);
                }
            }
            ).Start();

            //Thread.Sleep(100);

            new Thread(delegate()
            {
                while (true)
                {
                    pnl.Translate(1, 1);

                    if (pnl.X + pnl.Width >= display.Width - 10)
                        pnl.X = 0;
                    if (pnl.Y + pnl.Height >= display.Height - 10)
                        pnl.Y = 0;

                    //Thread.Sleep(100);
                }
            }
            ).Start();
        }
        private void UIDemo()
        {
            //Width = LCDManager.ScreenWidth;
            //Height = LCDManager.ScreenHeight;
            //Width = 320;
            //Height = 240;

            Desktop desktop = gm.Desktop;

            int k = desktop.Height / 240;
            font = Resources.GetFont(Resources.FontResources.CourierNew_10);

            desktop.SuspendLayout();

            //ImageBrush brush = new ImageBrush(Resources.GetBitmap(Resources.BinaryResources.Background_800_600));
            //brush.Stretch = Stretch.Fill;
            //Background = brush;

            int statusbarHeight = 24;
            Panel statusbar = new Panel(0, desktop.Height - statusbarHeight, desktop.Width, statusbarHeight);
            statusbar.Background = new ImageBrush(new Bitmap(Resources.GetBytes(Resources.BinaryResources.Bar), Bitmap.BitmapImageType.Bmp));
            desktop.Children.Add(statusbar);

            Label lblClock = new Label(statusbar.Width - 70, 4, font, "00:00:00");
            lblClock.ForeColor = Color.White;
            statusbar.Children.Add(lblClock);

            Level lvl2 = new Level(statusbar.Width - 120, 7, 40, 10, Orientation.Horizontal, 10);
            lvl2.Foreground = new LinearGradientBrush(Color.LimeGreen, Color.Black);
            lvl2.Value = 50;
            statusbar.Children.Add(lvl2);

            //statusbar.Children.Add(new Image(statusbar.Width - 160, 1, 23, 23, Resources.GetBitmap(Resources.BinaryResources.Drive)));
            //statusbar.Children.Add(new Image(statusbar.Width - 185, 1, 23, 23, Resources.GetBitmap(Resources.BinaryResources.Mouse)));
            //statusbar.Children.Add(new Image(statusbar.Width - 210, 1, 23, 23, Resources.GetBitmap(Resources.BinaryResources.Keyboard)));

        //    //ToolButton btnHome = new ToolButton(10, 0, 70, statusbar.Height);
        //    Button btnHome = new Button(10, 0, 70, statusbar.Height, null, "", Color.Black);
        //    btnHome.Foreground = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.Home));
        //    btnHome.Border = null;
        //    //btnHome.Enabled = false;
        //    statusbar.Children.Add(btnHome);

        //    //-------------------------------

        //    //Children.Add(new Checkbox(20*k, 20*k, 20*k, 20*k));
        //    Children.Add(new Checkbox(20, 20, 20, 20));


        //    //return;
        //    Children.Add(new TextBlock(500, 10, 100, 100, font, "Hello world! I'm a text block. I'm very cool!")
        //    {
        //        ForeColor = Color.White,
        //        Background = new LinearGradientBrush(Color.Aquamarine, Color.Yellow) { Opacity = 100 },
        //        TextAlignment = TextAlignment.Center,
        //        TextVerticalAlignment = VerticalAlignment.Top,
        //        TextWrap = true

        //    });


        //    Level lvl = new Level(20, 40, 60, 20, Orientation.Horizontal, 10);
        //    lvl.Foreground = new LinearGradientBrush(Color.Blue, Color.Black);
        //    //lvl.Value = 0;
        //    Children.Add(lvl);




        //    ProgressBar pg = new ProgressBar(20, 80, 100, 10);
        //    pg.Foreground = new LinearGradientBrush(Color.LimeGreen, Color.Red);
        //    //pg.Foreground.Opacity = 220;
        //    Children.Add(pg);

        //    Panel pnl = new Panel(20, 100, 100, 100);
        //    pnl.Background = new LinearGradientBrush(Color.Blue, Color.LimeGreen);
        //    //pnl.Background.Opacity = 80;
        //    Children.Add(pnl);

        //    Button btn = new Button(20, 220, 80, 30, font, "<", Color.White);
        //    Children.Add(btn);

        //    Button btn2 = new Button(60, 0, 80, 25, font, "Button 2 wwwwwwww", Color.White)
        //    {
        //        //BackgroundUnpressed = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.ButtonBackground)) { Opacity = 100 };
        //        //BackgroundPressed = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.ButtonBackground)) { Opacity = 220 };
        //    };
        //    btn2.Click += delegate(object sender, EventArgs e)
        //    {
        //        wndModal dlg = new wndModal(0, 0, 0, 0);
        //        dlg.ShowModal();

        //        int a = 0;
        //        int b = a;

        //        //Close();
        //    };
        //    statusbar.Children.Add(btn2);

        //    RadioButtonGroup rbg = new RadioButtonGroup(20, 260, 25, 70);
        //    rbg.Background = new LinearGradientBrush(Color.White, Color.DarkGray);
        //    //rbg.Background.Opacity = 120;
        //    rbg.AddRadioButton(new RadioButton(5, 5, 15, true));
        //    rbg.AddRadioButton(new RadioButton(5, 25, 15));
        //    rbg.AddRadioButton(new RadioButton(5, 45, 15));
        //    Children.Add(rbg);


        //    ToolButton tbtn;

        //    tbtn = new ToolButton(300, 150, 128, 128);
        //    //tbtn.BackgroundUnpressed = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.ButtonBackground)) { Opacity = 100 };
        //    //tbtn.BackgroundPressed = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.ButtonBackground)) { Opacity = 220 };
        //    tbtn.Foreground = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.Database));
        //    tbtn.Foreground.Opacity = 200;
        //    Children.Add(tbtn);

        //    tbtn = new ToolButton(450, 150, 128, 128);
        //    tbtn.Foreground = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.Operation));
        //    tbtn.Foreground.Opacity = 200;
        //    Children.Add(tbtn);

        //    tbtn = new ToolButton(600, 150, 128, 128);
        //    tbtn.Foreground = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.Settings));
        //    tbtn.Foreground.Opacity = 200;
        //    Children.Add(tbtn);

        //    Children.Add(new Slider(250, 20, 150, 30, 15, Orientation.Horizontal)
        //    {
        //        Value = 80,
        //        Background = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.Bar)),
        //        Foreground = new LinearGradientBrush(Color.LightGray, Color.Black) { Opacity = 50 }
        //    });
        //    Children.Add(new Slider(200, 20, 30, 150, 12, Orientation.Vertical)
        //    {
        //        Value = 70,
        //        Background = new SolidColorBrush(Color.White) { Opacity = 100 }
        //    });

        //    Slider slider = new Slider(250, 60, 150, 30, 30, Orientation.Horizontal)
        //    {
        //        Value = 80,
        //        Background = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.Bar)),
        //        Foreground = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.GHILogo)) { Opacity = 200 },
        //    };
        //    Children.Add(slider);
        //    Label lbl = new Label(250, 100, font, slider.Value.ToString()) { ForeColor = Color.White };
        //    Children.Add(lbl);
        //    slider.ValueChanged += delegate(object sender, ValueChangedEventArgs e) { lbl.Text = e.Value.ToString(); };




            new Thread(delegate()
            {
                int v = 0;
                while (true)
                {
                    desktop.SuspendLayout();

                    DateTime dt = DateTime.Now;

                    string hour = (dt.Hour < 10) ? "0" + dt.Hour.ToString() : dt.Hour.ToString();
                    string minute = (dt.Minute < 10) ? "0" + dt.Minute.ToString() : dt.Minute.ToString();
                    string second = (dt.Second < 10) ? "0" + dt.Second.ToString() : dt.Second.ToString();
                    string result = hour + ":" + minute + ":" + second;
                    lblClock.Text = result;

                    v += 10;
                    if (v > 100)
                        v = 0;

                    //lvl.Value = v;
                    //pg.Value = v;
                    lvl2.Value = v;

                    //Color temp = ((LinearGradientBrush)pnl.Background).StartColor;
                    //((LinearGradientBrush)pnl.Background).StartColor = ((LinearGradientBrush)pnl.Background).EndColor;
                    //((LinearGradientBrush)pnl.Background).EndColor = temp;
                    //pnl.Invalidate();

                    desktop.ResumeLayout();

                    Thread.Sleep(500);
                }
            }).Start();

            //wndModal wndModal = new wndModal();
            //wndModal.Show();

            desktop.ResumeLayout();
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

        private void BlinkLED(int led)
        {
            new Thread(() =>
            {
                indicators[led] = false;
                Thread.Sleep(10);
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

        bool on = false;
        static DateTime dt0;
        static TimeSpan ts;
        static TimeSpan sum;

        private Font font;
        private Point p;

        private void timerTest_Tick(Gadgeteer.Timer timer)
        {
            timerTest.Stop();

            //DateTime dt = TimeManager.CurrentTime;

            //(busHubI2C.BusControlLines[1] as ControlLine).SetState(new byte[] { (byte)(on ? 1 : 0), 0 });
            //on = !on;




            //SetState();
            //SendStateToClients();


            timerTest.Start();
        }
        private void busHubI2C_BusModulesCollectionChanged(ArrayList addressesAdded, ArrayList addressesRemoved)
        {
            new Thread(() => { 
                uint x = 10;
                uint indent = 10;
                uint y = 10;
                uint lineHight = 15;
                Font fontTitle = Resources.GetFont(Resources.FontResources.NinaB);
                Font font = Resources.GetFont(Resources.FontResources.small);
                GT.Color color = GT.Color.FromRGB(101, 156, 239); // cornflower blue

                y = 10;
                display.SimpleGraphics.Clear();
                //display.SimpleGraphics.DisplayText("****************************", fontTitle, color, x, y); y += lineHight;
                display.SimpleGraphics.DisplayText(busHubI2C.ProductName, fontTitle, color, x, y); y += lineHight;

                foreach (BusModule busModule in busHubI2C.BusModules)
                {
                    if (y > display.Height)
                        return;

                    display.SimpleGraphics.DisplayText("[" + busModule.Address + "]   " + busModule.ProductName, fontTitle, color, x+5, y); y += lineHight;

                    foreach (ControlLine controlLine in busModule.ControlLines)
                    {
                        if (y > display.Height)
                            return;

                        string state = "[" + controlLine.State[0] + "][" + controlLine.State[1] + "]";
                        display.SimpleGraphics.DisplayText(controlLine.ProductName + ": " + state, font, color, x + indent, y); y += lineHight;
                    }

                    display.SimpleGraphics.DisplayText("****************************", fontTitle, color, x, y); y += lineHight;
                }
            }).Start();
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
