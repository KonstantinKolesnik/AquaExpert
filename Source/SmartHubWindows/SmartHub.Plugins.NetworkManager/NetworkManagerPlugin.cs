using SmartHub.Core.Plugins;
using SmartHub.Plugins.Speech;
using SmartHub.Plugins.Timer.Attributes;
using System;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace SmartHub.Plugins.NetworkManager
{
    [Plugin]
    public class NetworkManagerPlugin : PluginBase
    {
        #region Fields
        private bool isInternetAvailable = false;
        #endregion

        #region Plugin overrides
        public override void InitPlugin()
        {
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;

            if (!NetworkInterface.GetIsNetworkAvailable())
                Context.GetPlugin<SpeechPlugin>()?.Say("Сетевое соединение недоступно");
        }
        #endregion

        #region Event handlers
        private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            Context.GetPlugin<SpeechPlugin>()?.Say(e.IsAvailable ? "Сетевое соединение восстановлено" : "Потеряно сетевое соединение");

            if (!e.IsAvailable)
            {
                ProcessStartInfo pInfo = new ProcessStartInfo();
                pInfo.FileName = @"C:\WINDOWS\System32\ipconfig.exe";

                pInfo.Arguments = "/release";
                Process.Start(pInfo).WaitForExit();

                pInfo.Arguments = "/renew";
                Process.Start(pInfo).WaitForExit();
            }
        }
        private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            //foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            //    foreach (UnicastIPAddressInformation addr in ni.GetIPProperties().UnicastAddresses)
            //        Console.WriteLine(" - {0} (lease expires {1})", addr.Address, DateTime.Now + new TimeSpan(0, 0, (int)addr.DhcpLeaseLifetime));
        }

        [RunPeriodically(1)]
        private void timer_Elapsed(DateTime now)
        {
            CheckForInternetConnection();
        }
        #endregion

        #region Public methods
        public void CheckForInternetConnection()
        {
            bool result = IsInternetAvailable();

            if (result)
            {
                if (!isInternetAvailable)
                    Context.GetPlugin<SpeechPlugin>()?.Say("Интернет соединение восстановлено");
            }
            else
            {
                if (isInternetAvailable)
                    Context.GetPlugin<SpeechPlugin>()?.Say("Потеряно интернет соединение");
            }

            isInternetAvailable = result;
        }
        public static bool IsInternetAvailable()
        {
            return new Ping().Send("www.google.com.mx", 5000).Status == IPStatus.Success;


            //try
            //{
            //    String host = "google.com";
            //    byte[] buffer = new byte[32];
            //    int timeout = 1000;
            //    PingOptions pingOptions = new PingOptions();
            //    PingReply reply = new Ping().Send(host, timeout, buffer, pingOptions);
            //    return (reply.Status == IPStatus.Success);
            //}
            //catch (Exception)
            //{
            //    return false;
            //}





            //try
            //{
            //    using (var client = new WebClient())
            //    using (var stream = client.OpenRead("http://www.google.com"))
            //    {
            //        return true;
            //    }
            //}
            //catch
            //{
            //    return false;
            //}
        }
        #endregion
    }
}
