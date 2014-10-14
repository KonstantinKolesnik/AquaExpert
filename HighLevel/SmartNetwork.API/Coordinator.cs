using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartNetwork.API
{
    public class Coordinator : ObservableObject
    {
        #region Fields
        //private const int updateInterval = 3000;

        private IEnumerable<Module> modules = new ObservableCollection<Module>();
        private IEnumerable<ControlLine> controlLines = new ObservableCollection<ControlLine>();
        //private Timer timerUpdate = null;
        #endregion

        #region Properties
        public IEnumerable<Module> Modules
        {
            get { return modules; }
        }
        public Module this[byte[] moduleAddress]
        {
            get
            {
                var res = modules.Where(module => module.Address == moduleAddress);
                return res.Any() ? res.First() : null;
            }
        }

        public IEnumerable<ControlLine> ControlLines // control lines of all modules
        {
            get { return controlLines; }
        }
        #endregion

        #region Constructors
        public Coordinator()
        {
            //StartTimer();
        }
        #endregion

        #region Protected methods
        //protected abstract ArrayList GetOnlineModules();
        //protected abstract void ScanModules(out ArrayList modulesAdded, out ArrayList modulesRemoved);


        //internal abstract bool BusModuleWriteRead(BusModule busModule, byte[] request, byte[] response);
        //internal abstract bool BusModuleWrite(BusModule busModule, byte[] request);
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
        private void Update()
        {
            //ArrayList modulesOnline = GetOnlineModules();


            //ArrayList modulesAdded, modulesRemoved;
            //ScanModules(out modulesAdded, out modulesRemoved);

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

            //// BusModulesCollectionChanged:
            //if (BusModulesCollectionChanged != null && (modulesAdded.Count != 0 || modulesRemoved.Count != 0))
            //    BusModulesCollectionChanged(modulesAdded, modulesRemoved);

            //// BusControlLinesCollectionChanged:
            //if (BusControlLinesCollectionChanged != null && (controlLinesAdded.Count != 0 || controlLinesRemoved.Count != 0))
            //    BusControlLinesCollectionChanged(controlLinesAdded, controlLinesRemoved);
        }
        #endregion
    }
}
