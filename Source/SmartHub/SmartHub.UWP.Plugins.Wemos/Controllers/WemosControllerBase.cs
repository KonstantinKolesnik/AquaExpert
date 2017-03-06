using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.Wemos.Controllers.Models;
using SmartHub.UWP.Plugins.Wemos.Core;
using System;

namespace SmartHub.UWP.Plugins.Wemos.Controllers
{
    public abstract class WemosControllerBase
    {
        #region Fields
        protected WemosController model;
        protected WemosPlugin host;
        protected IServiceContext context;
        //protected float? lastLineValue;
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
                //case WemosControllerType.Heater: return new HeaterController(model);
                case WemosControllerType.ScheduledSwitch: return new WemosScheduledSwitchController(model);
                //case WemosControllerType.WaterLevel: return new WaterLevelController(model);

                default: return null;
            }
        }
        public void Init(IServiceContext context)
        {
            this.context = context;
            host = context.GetPlugin<WemosPlugin>();

            //InitLastValues();
        }
        #endregion

        #region Abstract methods
        public abstract bool IsMyMessage(WemosMessage message);
        public abstract void RequestLinesValues();
        protected abstract void Process();
        #endregion

        #region Protected methods
        //protected virtual void InitLastValues()
        //{
        //}
        #endregion

        #region Event handlers
        public virtual void MessageCalibration(WemosMessage message)
        {
        }
        public virtual void MessageReceived(WemosMessage message)
        {
            if (model.IsAutoMode)
                Process();
        }
        public void TimerElapsed(DateTime now)
        {
            if (model.IsAutoMode)
                Process();
        }
        #endregion
    }
}
