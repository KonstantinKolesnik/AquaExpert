using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.Speech;
using Windows.Networking.Connectivity;

namespace SmartHub.UWP.Plugins.Network
{
    [Plugin]
    public class NetworkPlugin : PluginBase
    {
        #region Plugin overrides
        public override void InitPlugin()
        {
            NetworkInformation.NetworkStatusChanged += (s) => { CheckInternetAccess(); };
        }
        public override void StartPlugin()
        {
            CheckInternetAccess();
        }
        #endregion

        #region Private methods
        private void CheckInternetAccess()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            if (connectionProfile == null)
                Context.GetPlugin<SpeechPlugin>()?.Say("Нет доступных сетевых подключений");
            else
            {
                switch (connectionProfile.GetNetworkConnectivityLevel())
                {
                    case NetworkConnectivityLevel.None: Context.GetPlugin<SpeechPlugin>()?.Say("Локальная сеть не доступна"); break;
                    case NetworkConnectivityLevel.LocalAccess: Context.GetPlugin<SpeechPlugin>()?.Say("Доступна только локальная сеть"); break;
                    case NetworkConnectivityLevel.ConstrainedInternetAccess: Context.GetPlugin<SpeechPlugin>()?.Say("Доступен только ограниченный доступ в интернет"); break;
                    case NetworkConnectivityLevel.InternetAccess: Context.GetPlugin<SpeechPlugin>()?.Say("Доступна локальная сеть и полный доступ в интернет"); break;
                }
            }
        }
        #endregion
    }
}
