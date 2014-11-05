using System;
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
            try
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception) { }
        }
        #endregion
    }
}
