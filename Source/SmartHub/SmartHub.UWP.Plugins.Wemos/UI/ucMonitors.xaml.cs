using SmartHub.UWP.Core;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Wemos.UI
{
    public sealed partial class ucMonitors : UserControl
    {
        public ucMonitors()
        {
            InitializeComponent();
            Utils.FindFirstVisualChild<Grid>(this).DataContext = this;
        }
    }
}
