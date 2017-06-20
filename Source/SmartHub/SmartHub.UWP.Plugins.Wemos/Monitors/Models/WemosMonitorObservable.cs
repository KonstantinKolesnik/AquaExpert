using SmartHub.UWP.Core;
using SmartHub.UWP.Plugins.Wemos.Core.Models;

namespace SmartHub.UWP.Plugins.Wemos.Monitors.Models
{
    public class WemosMonitorObservable : ObservableObject
    {
        #region Fields
        private WemosMonitorDto model;
        #endregion

        #region Properties
        public int ID
        {
            get { return model.ID; }
        }
        public int LineID
        {
            get { return model.LineID; }
        }
        public string LineName
        {
            get { return model.LineName; }
        }
        public WemosLineType LineType
        {
            get { return model.LineType; }
        }
        public string Name
        {
            get { return model.Name; }
            set
            {
                if (model.Name != value)
                {
                    var res = CoreUtils.RequestAsync<bool>("/api/wemos/monitors/setnames", model.ID, value, model.NameForInformer);
                    model.Name = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string NameForInformer
        {
            get { return model.NameForInformer; }
            set
            {
                if (model.NameForInformer != value)
                {
                    var res = CoreUtils.RequestAsync<bool>("/api/wemos/monitors/setnames", model.ID, model.Name, value);
                    model.NameForInformer = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Configuration
        {
            get { return model.Configuration; }
            set
            {
                if (model.Configuration != value)
                {
                    var res = CoreUtils.RequestAsync<bool>("/api/wemos/monitors/setconfig", model.ID, value);
                    model.Configuration = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Constructor
        public WemosMonitorObservable(WemosMonitorDto model)
        {
            this.model = model;
        }
        #endregion
    }
}
