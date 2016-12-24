namespace SmartHub.UWP.Core
{
    public class AppData : ObservableObject
    {
        #region Settings data
        public string ServerUrl
        {
            get { return Utils.GetAppData(nameof(ServerUrl), "", IsRoaming); }
            set { Utils.SetAppData(nameof(ServerUrl), value?.Trim(), IsRoaming); NotifyPropertyChanged(); }
        }
        public string Language
        {
            get { return Utils.GetAppData(nameof(Language), "en-US", IsRoaming); }
            set { Utils.SetAppData(nameof(Language), value, IsRoaming); NotifyPropertyChanged(); }
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
