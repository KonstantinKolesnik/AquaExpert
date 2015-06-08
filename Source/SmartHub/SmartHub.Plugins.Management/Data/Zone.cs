using SmartHub.Core.Plugins.Utils;
using System;
using System.Collections.Generic;

namespace SmartHub.Plugins.Management.Data
{
    public class Zone
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string MonitorsList { get; set; }
        public virtual string ControllersList { get; set; }
        public virtual string ScriptsList { get; set; }
        public virtual string GrapsList { get; set; }




        //public virtual List<Guid> GetMonitorsList()
        //{
        //    var json = string.IsNullOrWhiteSpace(MonitorsList) ? "[]" : MonitorsList;
        //    return Extensions.FromJson(typeof(List<Guid>), json) as List<Guid>;
        //}
        //public virtual void SetMonitorsList(object value)
        //{
        //    MonitorsList = value.ToJson("[]");
        //}

        //public virtual List<Guid> GetControllersList()
        //{
        //    var json = string.IsNullOrWhiteSpace(ControllersList) ? "[]" : ControllersList;
        //    return Extensions.FromJson(typeof(List<Guid>), json) as List<Guid>;
        //}
        //public virtual void SetControllersList(List<Guid> value)
        //{
        //    ControllersList = value.ToJson("[]");
        //}
    }
}
