using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace SmartNetwork.Hardware
{
    public class Coordinator : ObservableObject
    {
        #region Fields
        private ObservableCollection<Module> modules = new ObservableCollection<Module>();
        #endregion

        #region Properties
        public ObservableCollection<Module> Modules
        {
            get { return modules; }
        }
        public ObservableCollection<ControlLine> ControlLines // control lines of all modules
        {
            get { return new ObservableCollection<ControlLine>(modules.SelectMany(module => module.ControlLines).ToList()); }
        }

        public Module this[byte[] moduleAddress]
        {
            get
            {
                var res = modules.Where(module => module.Address == moduleAddress);
                return res.Any() ? res.First() : null;
            }
        }
        public ControlLine this[byte[] moduleAddress, byte lineAddress]
        {
            get
            {
                var res = ControlLines.Where(line => line.Module.Address == moduleAddress && line.Address == lineAddress);
                return res.Any() ? res.First() : null;
            }
        }
        #endregion

        #region Events
        public event NotifyCollectionChangedEventHandler ModulesCollectionChanged;
        #endregion

        #region Constructors
        public Coordinator()
        {
        }
        #endregion

        #region Public methods
        public void UpdateNetwork()
        {
            var modulesOnline = GetOnlineModules();

            var modulesRemoved = Modules.Except(modulesOnline);
            foreach (var item in modulesRemoved)
                Modules.Remove(item);

            var modulesAdded = modulesOnline.Except(Modules);
            foreach (var module in modulesAdded)
                Modules.Add(module);

            if (ModulesCollectionChanged != null && (modulesAdded.Count() != 0 || modulesRemoved.Count() != 0))
                ModulesCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, modulesAdded, modulesRemoved));
        }
        #endregion

        #region Private methods
        internal bool Write(Module target, byte[] request)
        {
            return false;
        }
        internal bool WriteRead(Module target, byte[] request, byte[] response)
        {
            return false;
        }
        private IList<Module> GetOnlineModules()
        {
            //var newItems =
            //    from str in data
            //    let m = r.Match(str)
            //    let id = int.Parse(m.Groups["ID"].Value)
            //    let address = int.Parse(m.Groups["Address"].Value)
            //    let strName = m.Groups["Name"].Value
            //    let name = !string.IsNullOrEmpty(strName) ? strName.Replace("\"", "") : ""
            //    let protocol = m.Groups["Protocol"].Value
            //    select new Locomotive(id, address, name, protocol);


            return new List<Module>();
        }
        #endregion
    }
}
