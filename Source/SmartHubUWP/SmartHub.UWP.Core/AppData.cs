namespace SmartHub.UWP.Core
{
    public class AppData : ObservableObject
    {
        #region Settings data
        public string ServerUrl
        {
            get { return CoreUtils.GetAppData(nameof(ServerUrl), "", IsRoaming); }
            set { CoreUtils.SetAppData(nameof(ServerUrl), value?.Trim(), IsRoaming); NotifyPropertyChanged(); }
        }
        public string Language
        {
            get { return CoreUtils.GetAppData(nameof(Language), "en-US", IsRoaming); }
            set { CoreUtils.SetAppData(nameof(Language), value, IsRoaming); NotifyPropertyChanged(); }
        }
        public int Theme
        {
            get { return CoreUtils.GetAppData(nameof(Theme), 0); }
            set { CoreUtils.SetAppData(nameof(Theme), value); NotifyPropertyChanged(); }
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
