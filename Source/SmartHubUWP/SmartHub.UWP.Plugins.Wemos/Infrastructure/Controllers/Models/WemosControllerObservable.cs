using SmartHub.UWP.Core;

namespace SmartHub.UWP.Plugins.Wemos.Infrastructure.Controllers.Models
{
    public class WemosControllerObservable : ObservableObject
    {
        #region Fields
        private WemosController model;
        #endregion

        #region Properties
        public string ID
        {
            get { return model.ID; }
        }
        public WemosControllerType Type
        {
            get { return model.Type; }
        }
        public string Name
        {
            get { return model.Name; }
            set
            {
                if (model.Name != value)
                {
                    model.Name = value;
                    NotifyPropertyChanged(nameof(Name));
                }
            }
        }
        public bool IsAutoMode
        {
            get { return model.IsAutoMode; }
            set
            {
                if (model.IsAutoMode != value)
                {
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

            PropertyChanged += async (s, e) => { await CoreUtils.RequestAsync<bool>("/api/wemos/controllers/update", model); };
        }
        #endregion
    }
}
