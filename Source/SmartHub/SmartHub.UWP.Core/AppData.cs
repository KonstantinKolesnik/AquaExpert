namespace SmartHub.UWP.Core
{
    public class AppData : ObservableObject
    {
        #region Settings data
        public string RemoteUrl
        {
            get { return Utils.GetAppData(nameof(RemoteUrl), "en-US", IsRoaming); }
            set { Utils.SetAppData(nameof(RemoteUrl), value, IsRoaming); NotifyPropertyChanged(); }
        }
        public string Language
        {
            get { return Utils.GetAppData(nameof(Language), "en-US", IsRoaming); }
            set { Utils.SetAppData(nameof(Language), value, IsRoaming); NotifyPropertyChanged(); }
        }
        public bool IsServer
        {
            get { return Utils.GetAppData(nameof(IsServer), false, IsRoaming); }
            set { Utils.SetAppData(nameof(IsServer), value, IsRoaming); NotifyPropertyChanged(); }
        }
        #endregion

        #region Properties
        public bool IsRoaming
        {
            get;
        }
        #endregion

        #region Constructor
        public AppData(bool isRoaming)
        {
            IsRoaming = isRoaming;
        }
        #endregion
    }
}
