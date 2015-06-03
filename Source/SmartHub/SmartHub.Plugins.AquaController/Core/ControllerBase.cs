using SmartHub.Core.Plugins;
using SmartHub.Plugins.AquaController.Data;
using SmartHub.Plugins.MySensors;
using SmartHub.Plugins.MySensors.Core;
using System;

namespace SmartHub.Plugins.AquaController.Core
{
    public abstract class ControllerBase
    {
        #region Fields
        private Controller controller;
        protected MySensorsPlugin mySensors;
        protected IServiceContext Context;
        #endregion

        #region Constructor
        public ControllerBase(Controller controller)
        {
            this.controller = controller;
        }
        #endregion

        #region Public methods
        public void Init(IServiceContext context)
        {
            Context = context;
            mySensors = context.GetPlugin<MySensorsPlugin>();
        }
        public void Save()
        {
            using (var session = Context.OpenSession())
            {
                session.SaveOrUpdate(controller);
                session.Flush();
            }
        }

        public abstract bool IsMyMessage(SensorMessage message);
        public abstract void RequestSensorsValues();
        #endregion

        #region Private methods
        abstract protected void Process(float value);
        #endregion

        #region Event handlers
        public virtual void MessageCalibration(SensorMessage message)
        {
        }
        abstract public void MessageReceived(SensorMessage message);
        public virtual void TimerElapsed(DateTime now)
        {
        }
        #endregion
    }
}
