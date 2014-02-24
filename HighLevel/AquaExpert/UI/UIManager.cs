using System;
using Microsoft.SPOT;
using Gadgeteer.Modules.KKS;
using MFE.Graphics;
using MFE.Graphics.Controls;
using MFE.Graphics.Media;
using System.Threading;

namespace AquaExpert.UI
{
    class UIManager
    {
        //private DisplayS22 display;
        private GraphicsManager gm;
        private Desktop desktop;

        private Font fontRegular;
        private Font fontCourierNew10;
        private Font fontTitle;

        private Panel pnlSplash;

        public UIManager()
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


            fontRegular = Resources.GetFont(Resources.FontResources.LucidaSansUnicode_8);
            fontCourierNew10 = Resources.GetFont(Resources.FontResources.CourierNew_10);
            fontTitle = Resources.GetFont(Resources.FontResources.SegoeUI_BoldItalian_32);

            gm = new GraphicsManager(320, 240);
            desktop = gm.Desktop;

            //desktop.SuspendLayout();

            ImageBrush brush = new ImageBrush(GetBitmap(Resources.BinaryResources.Background, Bitmap.BitmapImageType.Jpeg));
            brush.Stretch = Stretch.Fill;
            desktop.Background = brush;

            InitSplashForm();

            

            //desktop.ResumeLayout();
        }




        private void InitSplashForm()
        {
            if (pnlSplash == null)
            {
                pnlSplash = new Panel(0, 0, desktop.Width, desktop.Height);

                TextBlock title = new TextBlock(0, 0, desktop.Width, desktop.Height / 2, fontTitle, "Aqua Expert")
                {
                    ForeColor = Color.CornflowerBlue,
                    TextAlignment = TextAlignment.Center,
                    TextVerticalAlignment = VerticalAlignment.Center,
                    TextWrap = true
                };
                pnlSplash.Children.Add(title);

                ProgressBar pb = new ProgressBar(desktop.Width / 6, 5 * desktop.Height / 6, 2 * desktop.Width / 3, 20)
                {
                    Background = new LinearGradientBrush(Color.CornflowerBlue, Color.Black),
                    Foreground = new LinearGradientBrush(Color.CornflowerBlue, Color.LimeGreen),
                    Value = 0
                };
                pnlSplash.Children.Add(pb);

                TextBlock text = new TextBlock(pb.X, pb.Y, pb.Width, pb.Height, fontCourierNew10, "")
                {
                    TextAlignment = TextAlignment.Center,
                    TextVerticalAlignment = VerticalAlignment.Center,
                    TextWrap = true
                };
                pnlSplash.Children.Add(text);


                desktop.Children.Add(pnlSplash);

                for (int i = 0; i <= 100; i++)
                {
                    pb.Value = i;
                    text.Text = i + " %";
                    Thread.Sleep(50);
                }
            }
        }

        private void CheckCalibration()
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
        private Bitmap GetBitmap(Resources.BinaryResources id, Bitmap.BitmapImageType type)
        {
            return new Bitmap(Resources.GetBytes(id), type);
        }
    }
}
