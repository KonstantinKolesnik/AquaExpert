using Newtonsoft.Json;
using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.Wemos.Controllers.Models;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
using System;
using System.Collections.Generic;

namespace SmartHub.UWP.Plugins.Wemos.Controllers
{
    public abstract class WemosControllerBase
    {
        #region Fields
        protected WemosController model;
        protected WemosPlugin host;
        protected IServiceContext context;
        protected readonly Dictionary<int, float> lastValues = new Dictionary<int, float>();
        #endregion

        #region Properties
        public WemosController Model
        {
            get { return model; }
        }
        public object Configuration
        {
            get
            {
                if (string.IsNullOrEmpty(model.Configuration))
                    model.Configuration = JsonConvert.SerializeObject(GetDefaultConfiguration());

                return JsonConvert.DeserializeObject(model.Configuration, GetConfigurationType());
            }
            set { model.Configuration = JsonConvert.SerializeObject(value); }
        }
        #endregion

        #region Constructor
        protected WemosControllerBase(WemosController model)
        {
            this.model = model;
        }
        #endregion

        #region Public methods
        public static WemosControllerBase FromModel(WemosController model)
        {
            if (model == null)
                return null;

            switch (model.Type)
            {
                case WemosControllerType.Heater: return new WemosHeaterController(model);
                case WemosControllerType.ScheduledSwitch: return new WemosScheduledSwitchController(model);
                //case WemosControllerType.WaterLevel: return new WaterLevelController(model);

                default: throw new Exception("Not supported controller type!");
            }
        }

        public void Init(IServiceContext context)
        {
            this.context = context;
            host = context.GetPlugin<WemosPlugin>();
        }
        public void Start()
        {
            RequestLinesValues(); // force lines to report their current values
        }
        public void ProcessMessage(WemosLineValue value)
        {
            if (IsMyMessage(value))
            {
                // + lastValues;

                MessageReceived(value);
                if (model.IsAutoMode)
                    Process();
            }
        }
        public void ProcessTimer(DateTime now)
        {
            if (model.IsAutoMode)
                Process();
        }
        #endregion

        #region Abstract methods
        protected abstract Type GetConfigurationType();
        protected abstract object GetDefaultConfiguration();

        protected abstract bool IsMyMessage(WemosLineValue value);
        protected abstract void RequestLinesValues();
        protected abstract void Process();
        protected virtual void MessageReceived(WemosLineValue value)
        {
        }
        #endregion
    }
}
