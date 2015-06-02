using NHibernate.Linq;
using SmartHub.Core.Plugins;
using SmartHub.Plugins.AquaController.Data;
using SmartHub.Plugins.MySensors;
using SmartHub.Plugins.MySensors.Core;
using System;
using System.Linq;

namespace SmartHub.Plugins.AquaController.Core
{
    public abstract class ControllerBase : Controller
    {
        #region Fields
        protected MySensorsPlugin mySensors;
        protected IServiceContext Context;
        #endregion

        #region Public methods
        public virtual void Init(IServiceContext context)
        {
            Context = context;
            mySensors = context.GetPlugin<MySensorsPlugin>();
        }
        public abstract bool IsMyMessage(SensorMessage message);
        public abstract void SetDefaultConfiguration();
        public void Save()
        {
            using (var session = Context.OpenSession())
            {
                session.SaveOrUpdate(this);
                session.Flush();
            }
        }
        #endregion

        #region Private methods

        abstract protected void RequestSensorsValues();
        abstract protected void Process(float value);
        #endregion

        #region Event handlers
        public void Connected()
        {
            RequestSensorsValues();
        }

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
