using SmartHub.UWP.Core;
using SmartHub.UWP.Plugins.UI.Attributes;

namespace SmartHub.UWP.Applications.Client.Common
{
    public class AppState : ObservableObject
    {
        private bool isAppsSection;
        private AppSectionItemAttribute selectedItemApps;
        private AppSectionItemAttribute selectedItemSystem;

        public bool IsAppsSection
        {
            get { return isAppsSection; }
            set
            {
                isAppsSection = value;
                NotifyPropertyChanged();
            }
        }
        public AppSectionItemAttribute SelectedItemApps
        {
            get { return selectedItemApps; }
            set
            {
                selectedItemApps = value;
                NotifyPropertyChanged();
            }
        }
        public AppSectionItemAttribute SelectedItemSystem
        {
            get { return selectedItemSystem; }
            set
            {
                selectedItemSystem = value;
                NotifyPropertyChanged();
            }
        }
    }
}
