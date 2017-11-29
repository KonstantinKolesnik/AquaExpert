using Newtonsoft.Json;
using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.Wemos.Controllers.Models;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
using System;

namespace SmartHub.UWP.Plugins.Wemos.Controllers
{
    abstract class WemosControllerWorker
    {
        protected WemosController ctrl;
        protected IServiceContext context;
        protected WemosPlugin host;

        protected object Configuration
        {
            //get { CheckModelConfiguration(); return JsonConvert.DeserializeObject(model.Configuration, GetConfigurationType()); }
            get { return JsonConvert.DeserializeObject(ctrl.Configuration, GetConfigurationType()); }

            //set { ctrl.Configuration = JsonConvert.SerializeObject(value); }
        }

        public WemosControllerWorker(WemosController ctrl, IServiceContext context)
        {
            this.ctrl = ctrl;
            this.context = context;
            host = context?.GetPlugin<WemosPlugin>();

            ctrl.Configuration = JsonConvert.SerializeObject(GetDefaultConfiguration());
        }

        public void Start()
        {
            RequestLinesValues(); // force lines to report their current values
        }
        public void ProcessMessage(WemosLineValue value)
        {
            if (IsMyMessage(value))
            {
                Preprocess(value);
                DoWork();
            }
        }
        public void ProcessTimer(DateTime now)
        {
            DoWork();
        }

        protected abstract Type GetConfigurationType();
        protected abstract object GetDefaultConfiguration();
        protected abstract void RequestLinesValues();
        protected abstract bool IsMyMessage(WemosLineValue value);
        protected virtual void Preprocess(WemosLineValue value)
        {
        }
        protected abstract void DoWork();

        #region Private methods
        //private void CheckModelConfiguration()
        //{
        //    if (string.IsNullOrEmpty(ctrl.Configuration))
        //        ctrl.Configuration = JsonConvert.SerializeObject(GetDefaultConfiguration());
        //}
        #endregion
    }
}
