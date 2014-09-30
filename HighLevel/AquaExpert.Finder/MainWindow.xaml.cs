using System;
using System.Windows;

namespace AquaExpert.Finder
{
    public partial class MainWindow : Window
    {
        private ServerFinder serverFinder;

        public MainWindow()
        {
            InitializeComponent();

            serverFinder = new ServerFinder(13000, "AquaExpert");
            serverFinder.ServerFound += serverFinder_ServerFound;
            serverFinder.ServerLost += serverFinder_ServerLost;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            serverFinder.Start();
        }
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            serverFinder.Stop();
        }
        private void serverFinder_ServerFound(object sender, EventArgs e)
        {
            
        }
        private void serverFinder_ServerLost(object sender, EventArgs e)
        {
            
        }
    }
}
