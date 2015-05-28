using NHibernate.Linq;
using SmartHub.Core.Plugins;
using SmartHub.Plugins.AquaController.Data;
using SmartHub.Plugins.MySensors;
using SmartHub.Plugins.MySensors.Attributes;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.SignalR;
using System;
using System.Linq;

namespace SmartHub.Plugins.AquaController.Core
{
    public abstract class ControllerBase
    {
        #region Fields
        protected MySensorsPlugin mySensors;
        protected IServiceContext Context;
        #endregion

        #region SignalR events
        private void NotifyForSignalR(object msg)
        {
            Context.GetPlugin<SignalRPlugin>().Broadcast(msg);
        }
        #endregion

        #region Public methods
        public virtual void Init(IServiceContext context)
        {
            Context = context;
            mySensors = Context.GetPlugin<MySensorsPlugin>();
        }
        public abstract bool IsMyMessage(SensorMessage message);
        #endregion

        #region Private methods
        protected AquaControllerSetting GetSetting(string name)
        {
            using (var session = Context.OpenSession())
                return session.Query<AquaControllerSetting>().FirstOrDefault(setting => setting.Name == name);
        }
        protected void SaveOrUpdate(object item)
        {
            using (var session = Context.OpenSession())
            {
                session.SaveOrUpdate(item);
                session.Flush();
            }
        }

        abstract protected void RequestSensorsValues();
        #endregion

        #region Event handlers
        [MySensorsConnected]
        private void Connected()
        {
            RequestSensorsValues();
        }

        [MySensorsMessage]
        abstract protected void MessageReceived(SensorMessage message);
        #endregion
    }
}
