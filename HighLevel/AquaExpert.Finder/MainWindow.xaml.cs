using System;
using System.Net.NetworkInformation;
using System.Windows;

namespace AquaExpert.Finder
{
    public partial class MainWindow : Window
    {
        private ServerFinder serverFinder;
        private DiscoveryListener listener;

        public MainWindow()
        {
            InitializeComponent();

            serverFinder = new ServerFinder(8888, "AquaExpert");
            serverFinder.ServerFound += serverFinder_ServerFound;
            serverFinder.ServerLost += serverFinder_ServerLost;

            listener = new DiscoveryListener(8888, "AquaExpert");
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnRefresh.IsEnabled = NetworkInterface.GetIsNetworkAvailable();
            //lvServers.ItemsSource = serverFinder.Servers;

            listener.Start();
        }
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            listener.Stop();
        }

        private void serverFinder_ServerFound(object sender, EventArgs e)
        {
            
        }
        private void serverFinder_ServerLost(object sender, EventArgs e)
        {
            
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            serverFinder.Refresh();
        }
    }
}
