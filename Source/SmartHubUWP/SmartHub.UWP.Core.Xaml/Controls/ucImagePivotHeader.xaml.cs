using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Core.Xaml.Controls
{
    public sealed partial class ucImagePivotHeader : UserControl
    {
        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(string), typeof(ucImagePivotHeader), null);
        public string Glyph
        {
            get { return (string)GetValue(GlyphProperty); }
            set { SetValue(GlyphProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ucImagePivotHeader), null);
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public ucImagePivotHeader()
        {
            InitializeComponent();
            XamlUtils.FindFirstVisualChild<StackPanel>(this).DataContext = this;
        }
    }
}
