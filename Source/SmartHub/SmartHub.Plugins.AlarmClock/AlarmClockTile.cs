using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Tiles;
using System;

namespace SmartHub.Plugins.AlarmClock
{
    [Tile]
    public class AlarmClockTile : TileBase
    {
        public override void PopulateWebModel(TileWebModel tileWebModel, dynamic parameters)
        {
            try
            {
                tileWebModel.title = "Оповещения";
                tileWebModel.url = "webapp/alarm-clock/list";
                tileWebModel.className = "btn-warning th-tile-icon th-tile-icon-fa fa-bell";
                tileWebModel.wide = true;
                tileWebModel.content = Context.GetPlugin<AlarmClockPlugin>().BuildTileContent();
                tileWebModel.SignalRReceiveHandler = Context.GetPlugin<AlarmClockPlugin>().BuildSignalRReceiveHandler();
            }
            catch (Exception ex)
            {
                tileWebModel.content = ex.Message;
            }
        }
    }
}
