using SmartHub.UWP.Plugins.Wemos.Core.Models;
using SmartHub.UWP.Plugins.Wemos.Infrastructure.Monitors.Models;

namespace SmartHub.UWP.Plugins.Wemos.Infrastructure.Monitors
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
                LineID = model.LineID;
                Factor = model.Factor;
                Offset = model.Offset;
                Units = model.Units;
                Min = model.Min;
                Max = model.Max;
                Precision = model.Precision;
                ValuesCount = model.ValuesCount;
            }
        }
    }
}
