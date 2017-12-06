using SmartHub.UWP.Core;

namespace SmartHub.UWP.Plugins.Scripts.Models
{
    public class UserScriptObservable : ObservableObject
    {
        #region Fields
        private UserScript model;
        #endregion

        #region Properties
        public string ID
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
                    var res = CoreUtils.RequestAsync<bool>("/api/scripts/setname", model.ID, value, model.Name);
                    model.Name = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Body
        {
            get { return model.Body; }
            set
            {
                if (model.Body != value)
                {
                    var res = CoreUtils.RequestAsync<bool>("/api/scripts/setbody", model.ID, value);
                    model.Body = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Constructor
        public UserScriptObservable(UserScript model)
        {
            this.model = model;
        }
        #endregion
    }
}
