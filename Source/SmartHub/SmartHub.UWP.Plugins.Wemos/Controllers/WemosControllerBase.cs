using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.Wemos.Controllers.Models;
using SmartHub.UWP.Plugins.Wemos.Core;
using System;

namespace SmartHub.UWP.Plugins.Wemos.Controllers
{
    public abstract class WemosControllerBase
    {
        #region Fields
        protected WemosController controller;
        protected WemosPlugin host;
        protected IServiceContext Context;
        protected float? lastLineValue;
        #endregion

        #region Constructor
        public WemosControllerBase(WemosController controller)
        {
            this.controller = controller;
        }
        #endregion

        #region Properties
        public int ID
        {
            get { return controller.ID; }
        }
        public string Name
        {
            get { return controller.Name; }
            set
            {
                if (controller.Name != value)
                {
                    controller.Name = value;
                    SaveToDB();
                }
            }
        }
        public bool IsAutoMode
        {
            get { return controller.IsAutoMode; }
            set
            {
                if (controller.IsAutoMode != value)
                {
                    controller.IsAutoMode = value;
                    SaveToDB();
                }
            }
        }
        public string Configuration
        {
            get { return controller.Configuration; }
            set
            {
                if (controller.Configuration != value)
                {
                    controller.Configuration = value;
                    SaveToDB();
                }
            }
        }
        #endregion

        #region Public methods
        public void Init(IServiceContext context)
        {
            Context = context;
            host = context.GetPlugin<WemosPlugin>();

            InitLastValues();
        }
        //public void SetConfiguration(string config)
        //{
        //    controller.Configuration = config;
        //    SaveToDB();
        //}
        public void AddToDB()
        {
            using (var db = Context.OpenConnection())
                db.Insert(controller);
        }
        public void SaveToDB()
        {
            using (var db = Context.OpenConnection())
                db.InsertOrReplace(controller);
        }
        public static WemosControllerBase FromController(WemosController controller)
        {
            if (controller == null)
                return null;

            switch (controller.Type)
            {
                //case WemosControllerType.Heater: return new HeaterController(controller);
                case WemosControllerType.ScheduledSwitch: return new WemosSwitchController(controller);
                //case WemosControllerType.WaterLevel: return new WaterLevelController(controller);

                default: return null;
            }
        }
        #endregion

        #region Abstract methods
        public abstract bool IsMyMessage(WemosMessage message);
        public abstract void RequestLinesValues();
        protected abstract void Process();
        #endregion

        #region Protected methods
        protected virtual void InitLastValues()
        {
        }
        #endregion

        #region Event handlers
        public virtual void MessageCalibration(WemosMessage message)
        {
        }
        public virtual void MessageReceived(WemosMessage message)
        {
            Process();
        }
        public void TimerElapsed(DateTime now)
        {
            Process();
        }
        #endregion
    }
}
