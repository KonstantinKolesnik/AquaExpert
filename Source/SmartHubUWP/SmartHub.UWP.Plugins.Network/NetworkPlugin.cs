using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Core.StringResources;
using SmartHub.UWP.Plugins.Speech;
using System;
using Windows.ApplicationModel.Core;
using Windows.Networking.Connectivity;
using Windows.UI.Core;

namespace SmartHub.UWP.Plugins.Network
{
    [Plugin]
    public class NetworkPlugin : PluginBase
    {
        #region Plugin overrides
        public override void InitPlugin()
        {
            NetworkInformation.NetworkStatusChanged += async (s) =>
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { CheckInternetAccess(); });
            };
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
                Context.GetPlugin<SpeechPlugin>()?.Say(Labels.NetworkConnectivityNone);
            else
            {
                switch (connectionProfile.GetNetworkConnectivityLevel())
                {
                    case NetworkConnectivityLevel.None: Context.GetPlugin<SpeechPlugin>()?.Say(Labels.NetworkConnectivityLevelNone); break;
                    case NetworkConnectivityLevel.LocalAccess: Context.GetPlugin<SpeechPlugin>()?.Say(Labels.NetworkConnectivityLevelLocalAccess); break;
                    case NetworkConnectivityLevel.ConstrainedInternetAccess: Context.GetPlugin<SpeechPlugin>()?.Say(Labels.NetworkConnectivityLevelConstrainedInternetAccess); break;
                    case NetworkConnectivityLevel.InternetAccess: Context.GetPlugin<SpeechPlugin>()?.Say(Labels.NetworkConnectivityLevelInternetAccess); break;
                }
            }
        }
        #endregion
    }
}
