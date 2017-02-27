using SmartHub.UWP.Core.Communication.Stream;
using SmartHub.UWP.Core.Infrastructure;
using SmartHub.UWP.Plugins.ApiListener;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources.Core;
using Windows.Globalization;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

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
            AppData.PropertyChanged += AppData_PropertyChanged;

            SetServerActivity();

            SetLanguage();
            //SetLanguage("en");
            //SetLanguage("de");
            //SetLanguage("ru");
            //SetLanguage("uk");
        }
        public static void OnSuspending(SuspendingDeferral defferal)
        {
            //hub.StopServices();
        }

        public static void SetAppTheme()
        {
            var theme = (ElementTheme) AppData.Theme;

            //AppShell.Current.RequestedTheme = theme;

            // set title bar colors:
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            if (titleBar != null)
            {
                //(Color) Application.Current.Resources["SystemChromeMediumColor"];
                var colorLight = Color.FromArgb(255, 230, 230, 230); // #FFE6E6E6
                var colorDark = Color.FromArgb(255, 31, 31, 31); // #FF1F1F1F

                Color color;
                if (theme == ElementTheme.Light)
                    color = colorLight;
                else if (theme == ElementTheme.Dark)
                    color = colorDark;
                else if (theme == ElementTheme.Default)
                    color = Application.Current.RequestedTheme == ApplicationTheme.Light ? colorLight : colorDark;

                titleBar.BackgroundColor = color;
                titleBar.ButtonBackgroundColor = color;
            }
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

        #region Event handlers
        private static void AppData_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
        }
        #endregion
    }
}
