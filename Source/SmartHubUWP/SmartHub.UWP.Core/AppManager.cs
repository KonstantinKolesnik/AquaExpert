using SmartHub.UWP.Core.Infrastructure;
using SmartHub.UWP.Plugins.ApiListener;
using Windows.ApplicationModel;

namespace SmartHub.UWP.Core
{
    public class AppManager
    {
        #region Fields
        private static Hub hub;
        #endregion

        #region Properties
        public static string AppId => Package.Current.Id.ProductId;
        public static string AppName => Package.Current.DisplayName;
        public static string AppVersion
        {
            get
            {
                var version = Package.Current.Id.Version;
                return $"{version.Major}.{version.Minor}.{version.Build}";
                //return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            }
        }
        public static string AppEmail => "gothicmaestro@live.com";
        public static string AppHomePage => "http://www.smarthub.at.ua";
        public static string AppPrivacyPolicyPage => $"{AppHomePage}/index/privacy_policy/0-10";
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
        public static string RemoteTcpServiceName => ApiListenerPlugin.TcpServiceName;
        #endregion

        #region Constructor
        static AppManager()
        {
            AppData = new AppData(false);
            AppData.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(AppData.Language): CoreUtils.SetAppLanguage(AppData.Language); break;
                    case nameof(AppData.ServerUrl): SetServerActivity(); break;
                }
            };

            CoreUtils.SetAppLanguage(AppData.Language);
            SetServerActivity();
        }
        #endregion

        //#region Public methods
        //public static void OnSuspending(SuspendingDeferral defferal)
        //{
        //    hub.StopServices();
        //}
        //#endregion

        #region Private methods
        private static void SetServerActivity()
        {
            IsServer = string.IsNullOrEmpty(AppData.ServerUrl?.Trim());

            if (IsServer)
            {
                if (hub == null)
                {
                    var assemblies = CoreUtils.GetSatelliteAssemblies(file => file.FileType == ".dll" && file.DisplayName.ToLower().StartsWith("smarthub"));

                    hub = new Hub();
                    hub.Init(assemblies);
                    hub.StartServices();
                }
            }
            else
            {
                hub?.StopServices();
                hub = null;
            }
        }
        #endregion
    }
}
