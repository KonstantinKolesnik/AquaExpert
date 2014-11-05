using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MySensors.Controllers.Core
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(/*[CallerMemberName]*/string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
