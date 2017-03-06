using SmartHub.UWP.Plugins.Wemos.Core.Models;
using SmartHub.UWP.Plugins.Wemos.Monitors.Models;

namespace SmartHub.UWP.Plugins.Wemos.Monitors
{
    public class WemosMonitorDto : WemosMonitor
    {
        public string LineName
        {
            get; set;
        }
        public WemosLineType LineType
        {
            get; set;
        }

        public WemosMonitorDto(WemosMonitor model)
        {
            if (model != null)
            {
                ID = model.ID;
                Name = model.Name;
                NameForInformer = model.NameForInformer;
                LineID = model.LineID;
                Configuration = model.Configuration;
            }
        }
    }
}
