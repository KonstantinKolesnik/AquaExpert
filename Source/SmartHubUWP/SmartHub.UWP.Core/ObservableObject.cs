using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmartHub.UWP.Core
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception) { }
        }
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            NotifyPropertyChanged(propertyName);

            return true;
        }
        #endregion
    }
}
