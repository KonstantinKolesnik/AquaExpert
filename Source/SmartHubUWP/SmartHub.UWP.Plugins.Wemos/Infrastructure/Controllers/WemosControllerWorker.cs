using Newtonsoft.Json;
using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
using SmartHub.UWP.Plugins.Wemos.Infrastructure.Controllers.Models;
using System;

namespace SmartHub.UWP.Plugins.Wemos.Infrastructure.Controllers
{
    public abstract class WemosControllerWorker
    {
        #region Fields
        protected WemosController ctrl;
        protected IServiceContext context;
        protected WemosPlugin host;
        #endregion

        #region Properties
        protected object Configuration
        {
            get { return JsonConvert.DeserializeObject(ctrl.Configuration, GetConfigurationType()); }
        }
        #endregion

        #region Constructor
        internal WemosControllerWorker(WemosController ctrl, IServiceContext context)
        {
            this.ctrl = ctrl;
            this.context = context;
            host = context?.GetPlugin<WemosPlugin>();

            if (string.IsNullOrEmpty(ctrl.Configuration))
                ctrl.Configuration = JsonConvert.SerializeObject(GetDefaultConfiguration());
        }
        #endregion

        #region Public methods
        internal void Start()
        {
            RequestLinesValues(); // force lines to report their current values
        }
        internal void ProcessMessage(WemosLineValue value)
        {
            if (IsMyMessage(value))
            {
                Preprocess(value);
                DoWork(DateTime.Now);
            }
        }
        internal void ProcessTimer(DateTime now)
        {
            DoWork(now);
        }
        #endregion

        #region Private methods
        protected abstract Type GetConfigurationType();
        protected abstract object GetDefaultConfiguration();

        protected abstract void RequestLinesValues();
        protected abstract bool IsMyMessage(WemosLineValue value);
        protected virtual void Preprocess(WemosLineValue value)
        {
        }
        protected abstract void DoWork(DateTime now);
        #endregion
    }
}
