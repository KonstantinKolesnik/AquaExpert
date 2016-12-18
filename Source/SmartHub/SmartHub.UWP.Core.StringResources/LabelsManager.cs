using System.ComponentModel;

namespace SmartHub.UWP.Core.StringResources
{
    public class LabelsManager : INotifyPropertyChanged
    {
        #region Properties
        public Labels Labels
        {
            get;
            private set;
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Constructor
        public LabelsManager()
        {
            Labels = new Labels();
        }
        #endregion

        #region Public methods
        public void RefreshResources()
        {
            NotifyPropertyChanged(nameof(Labels));
        }
        #endregion
    }
}
