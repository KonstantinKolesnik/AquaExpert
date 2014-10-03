using MFE.Graphics;
using MFE.Graphics.Controls;
using MFE.Graphics.Media;
using Microsoft.SPOT;

namespace AquaExpert.Server.UI
{
    static class UIManager
    {
        //private static DisplayS22 display;
        private static GraphicsManager gm;

        public static Font FontRegular;
        public static Font FontCourierNew10;
        public static Font FontTitle;

        public static Desktop Desktop;
        public static DebugPage DebugPage;
        public static SplashPage SplashPage;

        static UIManager()
        {
            //if (Mainboard.NativeBitmapConverter == null)
            //    Mainboard.NativeBitmapConverter = new Gadgeteer.Mainboard.BitmapConvertBPP(delegate(byte[] bitmapBytes, byte[] pixelBytes, GT.Mainboard.BPP bpp)
            //    {
            //        if (bpp != GT.Mainboard.BPP.BPP16_BGR_BE)
            //            throw new ArgumentOutOfRangeException("bpp", "Only BPP16_BGR_LE supported");

            //        Util.BitmapConvertBPP(bitmapBytes, pixelBytes, Util.BPP_Type.BPP16_BGR_BE);
            //    });

            //display = new DisplayS22(1);

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


            FontRegular = Resources.GetFont(Resources.FontResources.LucidaSansUnicode_8);
            FontCourierNew10 = Resources.GetFont(Resources.FontResources.CourierNew_10);
            FontTitle = Resources.GetFont(Resources.FontResources.SegoeUI_BoldItalian_32);

            gm = new GraphicsManager(320, 240);
            //gm = new GraphicsManager(480, 272);
            //gm = new GraphicsManager(800, 480);
            Desktop = gm.Desktop;

            //desktop.SuspendLayout();

            ImageBrush brush = new ImageBrush(GetBitmap(Resources.BinaryResources.Background, Bitmap.BitmapImageType.Jpeg));
            brush.Stretch = Stretch.Fill;
            Desktop.Background = brush;

            DebugPage = new DebugPage();
            SplashPage = new SplashPage();
            

            //desktop.ResumeLayout();
        }

        public static void CheckCalibration()
        {
            if (!gm.IsCalibrated)
            {
                var cw = gm.CalibrationWindow;
                cw.Background = new SolidColorBrush(Color.CornflowerBlue);
                cw.CrosshairPen = new Pen(Color.Red, 1);

                TextBlock text = new TextBlock(0, 0, cw.Width, cw.Height / 2, Resources.GetFont(Resources.FontResources.CourierNew_10), "Please touch the crosshair")
                {
                    ForeColor = Color.White,
                    TextAlignment = TextAlignment.Center,
                    TextVerticalAlignment = VerticalAlignment.Center,
                    TextWrap = true
                };
                cw.Children.Add(text);

                cw.Show();
            }
        }
        public static Bitmap GetBitmap(Resources.BinaryResources id, Bitmap.BitmapImageType type)
        {
            return new Bitmap(Resources.GetBytes(id), type);
        }
    }
}
