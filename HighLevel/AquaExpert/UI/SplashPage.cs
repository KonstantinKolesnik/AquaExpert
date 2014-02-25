using MFE.Graphics.Controls;
using MFE.Graphics.Media;

namespace AquaExpert.UI
{
    class SplashPage : Panel
    {
        private TextBlock tbTitle;
        private ProgressBar pbLoad;
        private TextBlock tbLoad;

        public string Title
        {
            set { tbTitle.Text = value; }
        }
        public int ProgressValue
        {
            set
            {
                pbLoad.Value = value;
                tbLoad.Text = value + " %";
            }
        }

        public SplashPage()
            : base(0, 0, UIManager.Desktop.Width, UIManager.Desktop.Height)
        {
            tbTitle = new TextBlock(0, 0, UIManager.Desktop.Width, UIManager.Desktop.Height / 2, UIManager.FontTitle, "")
            {
                ForeColor = Color.CornflowerBlue,
                TextAlignment = TextAlignment.Center,
                TextVerticalAlignment = VerticalAlignment.Center,
                TextWrap = true
            };
            Children.Add(tbTitle);

            pbLoad = new ProgressBar(UIManager.Desktop.Width / 6, 5 * UIManager.Desktop.Height / 6, 2 * UIManager.Desktop.Width / 3, 20)
            {
                Background = new LinearGradientBrush(Color.CornflowerBlue, Color.Black),
                Foreground = new LinearGradientBrush(Color.CornflowerBlue, Color.LimeGreen),
                Value = 0
            };
            Children.Add(pbLoad);

            tbLoad = new TextBlock(pbLoad.X, pbLoad.Y, pbLoad.Width, pbLoad.Height, UIManager.FontCourierNew10, "0 %")
            {
                TextAlignment = TextAlignment.Center,
                TextVerticalAlignment = VerticalAlignment.Center,
                TextWrap = true
            };
            Children.Add(tbLoad);
        }
    }
}
