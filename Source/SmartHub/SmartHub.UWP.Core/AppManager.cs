using SmartHub.UWP.Core.Infrastructure;
using SmartHub.UWP.Plugins.ApiListener;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources.Core;
using Windows.Globalization;

namespace SmartHub.UWP.Core
{
    public class AppManager
    {
        #region Fields
        private static Hub hub;
        #endregion

        #region Properties
        //public static string AppId
        //{
        //    get { return "9b1c8c98-9041-497b-9c5b-40002ad629c8"; }
        //}
        public static string AppName
        {
            get { return Package.Current.DisplayName; }
        }
        public static string AppVersion
        {
            get
            {
                var version = Package.Current.Id.Version;
                return $"{version.Major}.{version.Minor}.{version.Build}";
                //return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            }
        }
        public static string AppEmail
        {
            get { return "gothicmaestro@live.com"; }
        }
        public static string AppHomePage
        {
            get { return "http://www.smarthub.at.ua"; }
        }
        public static string AppPrivacyPolicyPage
        {
            get { return $"{AppHomePage}/index/privacy_policy/0-10"; }
        }
        public static AppData AppData
        {
            get; private set;
        }
        //public static AppCommands AppCommands
        //{
        //    get;
        //} = new AppCommands();

        public static bool IsServer
        {
            get; private set;
        }

        public static string RemoteUrl => IsServer ? "localhost" : AppData.ServerUrl;
        public static string RemoteServiceName => ApiListenerPlugin.ServiceName;
        #endregion

        #region Public methods
        public static void Init()
        {
            AppData = new AppData(false);
            AppData.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(AppData.Language):
                        SetLanguage();
                        break;
                    case nameof(AppData.ServerUrl):
                        SetServerActivity();
                        break;
                }
            };

            SetLanguage();
            SetServerActivity();
        }
        public static void OnSuspending(SuspendingDeferral defferal)
        {
            //hub.StopServices();
        }
        #endregion

        #region Private methods
        private static void SetLanguage()
        {
            var id = AppData.Language;
            //var id = "en-US";
            //var id = "de-DE";
            //var id = "ru-RU";
            //var id = "uk-UA";

            ApplicationLanguages.PrimaryLanguageOverride = id;
            ResourceContext.GetForCurrentView().Languages = new List<string>() { id };
        }
        private static void SetServerActivity()
        {
            IsServer = string.IsNullOrEmpty(AppData.ServerUrl?.Trim());

            if (IsServer)
            {
                if (hub == null)
                {
                    var assemblies = Utils.GetSatelliteAssemblies(file => file.FileType == ".dll" && file.DisplayName.StartsWith("SmartHub"));

                    hub = new Hub();
                    hub.Init(assemblies);
                    hub.StartServices();
                }
            }
            else if (hub != null)
            {
                hub.StopServices();
                hub = null;
            }
        }
        #endregion
    }
}
