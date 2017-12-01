﻿using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
using SQLite.Net.Attributes;
using System;

namespace SmartHub.UWP.Plugins.Wemos.Controllers.Models
{
    public class WemosController
    {
        #region Fields
        private WemosControllerWorker worker;
        #endregion

        #region Properties
        [PrimaryKey, NotNull, AutoIncrement]
        public int ID
        {
            get; set;
        }
        [NotNull]
        public string Name
        {
            get; set;
        }
        [NotNull]
        public WemosControllerType Type
        {
            get; set;
        }
        [NotNull, Default]
        public bool IsAutoMode
        {
            get; set;
        }
        [NotNull]
        public string Configuration
        {
            get; set;
        }
        #endregion

        #region Public methods
        public void Init(IServiceContext context)
        {
            switch (Type)
            {
                case WemosControllerType.ScheduledSwitch: worker = new WemosControllerWorkerScheduledSwitch(this, context); break;
                case WemosControllerType.Heater: worker = new WemosControllerWorkerHeater(this, context); break;
                //case WemosControllerType.WaterLevel: worker = new WemosControllerWaterLevel(this, context); break;
                default: throw new Exception("Not supported controller type!");
            }
        }
        public void Start()
        {
            worker?.Start();
        }
        public void ProcessMessage(WemosLineValue value)
        {
            if (IsAutoMode)
                worker?.ProcessMessage(value);
        }
        public void ProcessTimer(DateTime now)
        {
            if (IsAutoMode)
                worker?.ProcessTimer(now);
        }
        #endregion
    }
}
