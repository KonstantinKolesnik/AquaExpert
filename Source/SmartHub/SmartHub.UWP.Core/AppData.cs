using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.UWP.Core
{
    public class AppData : ObservableObject
    {
        #region Settings data
        //private const string DeviceKey = "Device";
        //private const string ModelsKey = "Models";

        //public Device Device
        //{
        //    get { return Utils.GetAppData(DeviceKey, Device.Default, IsRoaming); }
        //    set { Utils.SetAppData(DeviceKey, value); NotifyPropertyChanged(DeviceKey); }
        //}
        //public ObservableCollection<Model> Models
        //{
        //    get { return Utils.GetAppData(ModelsKey, new ObservableCollection<Model>(), IsRoaming); }
        //    set { Utils.SetAppData(ModelsKey, value); NotifyPropertyChanged(ModelsKey); }
        //}
        #endregion

        #region Properties
        public bool IsRoaming { get; private set; }
        #endregion

        #region Constructor
        public AppData(bool isRoaming)
        {
            IsRoaming = isRoaming;
        }
        #endregion
    }
}
