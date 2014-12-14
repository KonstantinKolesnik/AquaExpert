﻿using SmartNetwork.Plugins.MySensors.Core;
using System;

namespace SmartNetwork.Plugins.MySensors.Data
{
    public class Sensor
    {
        public virtual Guid Id { get; set; }

        public virtual Node Node { get; set; }
        public virtual byte ID { get; set; }
        public virtual SensorType Type { get; set; }
        public virtual string ProtocolVersion { get; set; }
        
        //private ObservableCollection<SensorValue> values = new ObservableCollection<SensorValue>();
    }
}