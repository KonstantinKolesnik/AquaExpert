﻿using SmartHub.Core.Plugins.Utils;
using System;

namespace SmartHub.Plugins.WebUI.Data
{
    public class TileDB
    {
        public virtual Guid Id { get; set; }
        public virtual string HandlerKey { get; set; } // is tileTypeFullName
        public virtual int SortOrder { get; set; }
        public virtual string SerializedParameters { get; set; }
        
        public virtual dynamic GetParameters()
        {
            var json = string.IsNullOrWhiteSpace(SerializedParameters) ? "{}" : SerializedParameters;
            return Extensions.FromJson(json);
        }
        public virtual void SetParameters(object parameters)
        {
            SerializedParameters = parameters.ToJson();
        }
    }
}
