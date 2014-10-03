using MFE.Graphics.Controls;
using MFE.Graphics.Media;

namespace AquaExpert.Server.UI
{
    class DebugPage : Panel
    {
        private TextBlock tbText;

        public string Text
        {
            get { return tbText.Text; }
            set { tbText.Text = value; }
        }

        public DebugPage()
            : base(0, 0, UIManager.Desktop.Width, UIManager.Desktop.Height)
        {
            tbText = new TextBlock(0, 0, UIManager.Desktop.Width, UIManager.Desktop.Height, UIManager.FontRegular, "")
            {
                ForeColor = Color.White,
                TextAlignment = TextAlignment.Left,
                TextVerticalAlignment = VerticalAlignment.Top,
                TextWrap = true
            };
            Children.Add(tbText);
        }

        public void AddLine(string txt)
        {
            Text += (Text != "" ? "\n" : "") + txt;
        }
        public void Clear()
        {
            Text = "";
        }
    }
}
