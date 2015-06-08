using SmartHub.Core.Plugins;
using SmartHub.Plugins.Controllers.Data;
using SmartHub.Plugins.MySensors;
using SmartHub.Plugins.MySensors.Core;
using System;

namespace SmartHub.Plugins.Controllers.Core
{
    public abstract class ControllerBase
    {
        #region Fields
        protected Controller controller;
        protected MySensorsPlugin mySensors;
        protected IServiceContext Context;
        #endregion

        #region Constructor
        public ControllerBase(Controller controller)
        {
            this.controller = controller;
        }
        #endregion

        #region Properties
        public Guid ControllerID
        {
            get { return controller.Id; }
        }
        #endregion

        #region Public methods
        public void Init(IServiceContext context)
        {
            Context = context;
            mySensors = context.GetPlugin<MySensorsPlugin>();
        }
        public void SaveToDB()
        {
            using (var session = Context.OpenSession())
            {
                session.SaveOrUpdate(controller);
                session.Flush();
            }
        }

        public abstract void SetConfiguration(string configuration);
        public abstract bool IsMyMessage(SensorMessage message);
        public abstract void RequestSensorsValues();
        #endregion

        #region Private methods
        abstract protected void Process(float? value);
        #endregion

        #region Event handlers
        public virtual void MessageCalibration(SensorMessage message)
        {
        }
        public virtual void MessageReceived(SensorMessage message)
        {
        }
        public virtual void TimerElapsed(DateTime now)
        {
        }
        #endregion
    }
}
