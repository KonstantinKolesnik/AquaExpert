using SmartHub.UWP.Core;

namespace SmartHub.UWP.Plugins.Wemos.Controllers.Models
{
    public class WemosControllerObservable : ObservableObject
    {
        #region Fields
        private WemosController model;
        #endregion

        #region Properties
        public int ID
        {
            get { return model.ID; }
        }
        public string Name
        {
            get { return model.Name; }
            set
            {
                if (model.Name != value)
                {
                    var res = Utils.RequestAsync<bool>("/api/wemos/controllers/setname", model.ID, value);
                    model.Name = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public WemosControllerType Type
        {
            get { return model.Type; }
        }
        public bool IsAutoMode
        {
            get { return model.IsAutoMode; }
            set
            {
                if (model.IsAutoMode != value)
                {
                    var res = Utils.RequestAsync<bool>("/api/wemos/controllers/setautomode", model.ID, value);
                    model.IsAutoMode = value;
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
                    var res = Utils.RequestAsync<bool>("/api/wemos/controllers/setconfig", model.ID, value);

                    model.Configuration = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Constructor
        public WemosControllerObservable(WemosController model)
        {
            this.model = model;
        }
        #endregion
    }
}
