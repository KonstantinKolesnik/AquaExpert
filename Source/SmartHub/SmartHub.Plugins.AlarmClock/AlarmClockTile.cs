using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Tiles;
using System;
using System.Linq;

namespace SmartHub.Plugins.AlarmClock
{
    [Tile]
    public class AlarmClockTile : TileBase
    {
        public override void PopulateWebModel(TileWebModel tileWebModel, dynamic parameters)
        {
            tileWebModel.title = "Оповещения";
            tileWebModel.url = "webapp/alarm-clock/list";
            tileWebModel.content = GetAlarmTileContent();
            tileWebModel.wide = true;
            tileWebModel.className = "btn-warning th-tile-icon th-tile-icon-fa fa-bell";
        }

        private string GetAlarmTileContent()
        {
            var now = DateTime.Now;

            //var times = Context.GetPlugin<AlarmClockPlugin>().GetNextAlarmTimes(now).Take(4);
            //var strTimes = times.Select(t => t.ToShortTimeString()).ToArray();

            var strTimes = Context.GetPlugin<AlarmClockPlugin>().GetNextAlarmsTimesAndNames(now).Take(6);
            
            return strTimes.Any() ? string.Join(Environment.NewLine, strTimes) : "Нет активных оповещений";
        }
    }
}
