using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Xaml;
using SmartHub.UWP.Plugins.Scripts.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Scripts.UI.Controls
{
    public sealed partial class ucScript : UserControl
    {
        #region Properties
        public static readonly DependencyProperty ScriptProperty = DependencyProperty.Register("Script", typeof(UserScriptObservable), typeof(ucScript), null);
        public UserScriptObservable Script
        {
            get { return (UserScriptObservable)GetValue(ScriptProperty); }
            set { SetValue(ScriptProperty, value); }
        }

        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(ucScript), new PropertyMetadata(false));
        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }
        #endregion

        #region Constructor
        public ucScript()
        {
            InitializeComponent();
            XamlUtils.FindFirstVisualChild<Grid>(this).DataContext = this;
        }
        #endregion

        #region Event handlers
        private async void btnRun_Click(object sender, RoutedEventArgs e)
        {
            await CoreUtils.RequestAsync<bool>("/api/scripts/run", Script.ID);
        }
        #endregion
    }
}
