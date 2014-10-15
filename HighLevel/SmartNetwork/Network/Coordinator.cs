using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace SmartNetwork.Network
{
    public class Coordinator : ObservableObject
    {
        #region Fields
        //private const int updateInterval = 3000;

        private ObservableCollection<Module> modules = new ObservableCollection<Module>();
        private ObservableCollection<ControlLine> controlLines = new ObservableCollection<ControlLine>();
        //private Timer timerUpdate = null;
        //private Windows.UI.Xaml.DispatcherTimer t;
        #endregion

        #region Properties
        public ObservableCollection<Module> Modules
        {
            get { return modules; }
        }
        public ObservableCollection<ControlLine> ControlLines // control lines of all modules
        {
            get { return controlLines; }
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
                var res = controlLines.Where(line => line.Module.Address == moduleAddress && line.Address == lineAddress);
                return res.Any() ? res.First() : null;
            }
        }
        #endregion

        #region Events
        public event NotifyCollectionChangedEventHandler ModulesCollectionChanged;
        public event NotifyCollectionChangedEventHandler ControlLinesCollectionChanged;
        #endregion

        #region Constructors
        public Coordinator()
        {
            //StartTimer();
        }
        #endregion


        #region Private methods
        //private void StartTimer()
        //{
        //    timerUpdate = new Timer((state) =>
        //    {
        //        StopTimer();
        //        Update();
        //        StartTimer();
        //    }, null, updateInterval, updateInterval);
        //}
        //private void StopTimer()
        //{
        //    timerUpdate.Change(Timeout.Infinite, Timeout.Infinite);
        //}

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
            return null;
        }
        private void Update()
        {
            IList<Module> modulesOnline = GetOnlineModules();
            IList<Module> modulesAdded = new List<Module>(), modulesRemoved = new List<Module>();

            foreach (Module module in modulesOnline)
            {
                //if (module != null) // query module
                //{
                //    // module with this address is online;
                //    // check if it's already registered in BusModules:

                //    BusModule busModule = this[new byte[] { address }];

                //    if (busModule == null) // module with this address isn't registered
                //    {
                //        busModule = new BusModule(this, new byte[] { address }, (BusModuleType)type);

                //        // query module control lines count with updating lines states:
                //        //busModule.QueryControlLines(true);

                //        // register this module in BusModules:
                //        modulesAdded.Add(busModule);
                //        BusModules.Add(busModule);
                //    }
                //    else // module with this address is already registered
                //    {
                //        // updated when added;
                //    }
                //}
                //else
                //{
                //    // module with this address is offline;
                //    // check if it's already registered in BusModules:

                //    BusModule busModule = this[new byte[] { address }];

                //    if (busModule != null) // offline module
                //    {
                //        modulesRemoved.Add(busModule);
                //        BusModules.Remove(busModule);
                //    }
                //}
            }















            //ArrayList controlLinesAdded = new ArrayList();
            //ArrayList controlLinesRemoved = new ArrayList();

            //// removed control lines:
            //foreach (BusModule moduleRemoved in modulesRemoved)
            //    foreach (ControlLine controlLine in busControlLines)
            //    {
            //        if (controlLine.BusModule.Address == moduleRemoved.Address)
            //        {
            //            busControlLines.Remove(controlLine);
            //            controlLinesRemoved.Add(controlLine);
            //        }
            //    }

            //// added control lines:
            //foreach (BusModule moduleAdded in modulesAdded)
            //{
            //    BusModule busModule = this[moduleAdded.Address];
            //    foreach (ControlLine controlLine in busModule.ControlLines)
            //    {
            //        busControlLines.Add(controlLine);
            //        controlLinesAdded.Add(controlLine);
            //    }
            //}

            // ModulesCollectionChanged:
            if (ModulesCollectionChanged != null && (modulesAdded.Count != 0 || modulesRemoved.Count != 0))
                ModulesCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, modulesAdded, modulesRemoved));

            //// BusControlLinesCollectionChanged:
            //if (BusControlLinesCollectionChanged != null && (controlLinesAdded.Count != 0 || controlLinesRemoved.Count != 0))
            //    BusControlLinesCollectionChanged(controlLinesAdded, controlLinesRemoved);
        }
        #endregion
    }
}
