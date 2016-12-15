using SmartHub.UWP.Core;
using SmartHub.UWP.Plugins.Wemos;
using SmartHub.UWP.Plugins.Wemos.Core;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

            AppManager.Hub.Context.GetPlugin<WemosPlugin>().MessageReceived += MainPage_DataReceived; ;
        }

        private async void MainPage_DataReceived(object sender, WemosMessageEventArgs args)
        {
            //await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Messages.Add(args.Message.ToString()));
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Messages.Add(args.Message.ToString()));
        }

        private async void ButtonPresent_Click(object sender, RoutedEventArgs e)
        {
            await AppManager.Hub.Context.GetPlugin<WemosPlugin>().RequestPresentation();
        }
        //private async void ButtonSend_Click(object sender, RoutedEventArgs e)
        //{
        //    await AppManager.Hub.Context.GetPlugin<WemosPlugin>().Send(new WemosMessage(1, 1, WemosMessageType.Set, 0).Set(DateTime.Now + ": Test string"));
        //}
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
    }
}
//