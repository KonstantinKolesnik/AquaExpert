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
        public string ID
        {
            get { return model.ID; }
        }
        public string LineID
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
        public float Min
        {
            get { return model.Min; }
            set
            {
                if (model.Min != value)
                {
                    model.Min = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public float Max
        {
            get { return model.Max; }
            set
            {
                if (model.Max != value)
                {
                    model.Max = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //public string Configuration
        //{
        //    get { return model.Configuration; }
        //    set
        //    {
        //        if (model.Configuration != value)
        //        {
        //            model.Configuration = value;
        //            NotifyPropertyChanged();
        //        }
        //    }
        //}
        #endregion

        #region Constructor
        public WemosMonitorObservable(WemosMonitorDto model)
        {
            this.model = model;
            PropertyChanged += (s, e) => { var res = CoreUtils.RequestAsync<bool>("/api/wemos/monitors/update", model); };
        }
        #endregion
    }
}
