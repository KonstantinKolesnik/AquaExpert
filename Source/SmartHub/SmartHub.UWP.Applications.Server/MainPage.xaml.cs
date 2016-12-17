using SmartHub.UWP.Core;
using SmartHub.UWP.Plugins.UI;
using SmartHub.UWP.Plugins.Wemos;
using SmartHub.UWP.Plugins.Wemos.Core;
using SmartHub.UWP.Plugins.Wemos.Models;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SmartHub.UWP.Applications.Server
{
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<string> Messages
        {
            get;
        } = new ObservableCollection<string>();

        public MainPage()
        {
            InitializeComponent();
            DataContext = this;

            AppManager.Hub.Context.GetPlugin<WemosPlugin>().MessageReceived += async (s, e) =>
            {
                //await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Messages.Add(args.Message.ToString()));
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Messages.Add(e.Message.ToString()));
            };
        }

        #region Navigation
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            holder.Content = AppManager.Hub.Context.GetPlugin<UIPlugin>().GetUI();
        }
        #endregion

        #region Test buttons
        private async void ButtonPresent_Click(object sender, RoutedEventArgs e)
        {
            await AppManager.Hub.Context.GetPlugin<WemosPlugin>().RequestPresentation();
        }
        private async void ButtonReboot_Click(object sender, RoutedEventArgs e)
        {
            await AppManager.Hub.Context.GetPlugin<WemosPlugin>().Send(new WemosMessage(1962017, -1, WemosMessageType.Internal, (int)WemosInternalMessageType.Reboot));
        }
        private void ButtonCear_Click(object sender, RoutedEventArgs e)
        {
            Messages.Clear();
        }
        private async void ButtonOn_Click(object sender, RoutedEventArgs e)
        {
            await AppManager.Hub.Context.GetPlugin<WemosPlugin>().Send(new WemosMessage(1962017, 0, WemosMessageType.Set, (int)WemosLineType.Switch).Set(true));
        }
        private async void ButtonOff_Click(object sender, RoutedEventArgs e)
        {
            await AppManager.Hub.Context.GetPlugin<WemosPlugin>().Send(new WemosMessage(1962017, 0, WemosMessageType.Set, (int) WemosLineType.Switch).Set(false));
        }
        #endregion
    }
}
