using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Core.Xaml.Controls
{
    public sealed partial class ucPropertyNameValuePair : UserControl
    {
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(ucPropertyNameValuePair), null);
        public string PropertyName
        {
            get { return (string) GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }

        public static readonly DependencyProperty PropertyValueProperty = DependencyProperty.Register("PropertyValue", typeof(string), typeof(ucPropertyNameValuePair), null);
        public string PropertyValue
        {
            get { return (string) GetValue(PropertyValueProperty); }
            set { SetValue(PropertyValueProperty, value); }
        }

        public static readonly DependencyProperty IsSmallProperty = DependencyProperty.Register("IsSmall", typeof(bool), typeof(ucPropertyNameValuePair), new PropertyMetadata(false));
        public bool IsSmall
        {
            get { return (bool) GetValue(IsSmallProperty); }
            set { SetValue(IsSmallProperty, value); }
        }

        public static readonly DependencyProperty IsVerticalProperty = DependencyProperty.Register("IsVertical", typeof(bool), typeof(ucPropertyNameValuePair), new PropertyMetadata(false));
        public bool IsVertical
        {
            get { return (bool) GetValue(IsVerticalProperty); }
            set { SetValue(IsVerticalProperty, value); }
        }

        public ucPropertyNameValuePair()
        {
            InitializeComponent();
            XamlUtils.FindFirstVisualChild<Grid>(this).DataContext = this;
        }
    }
}
