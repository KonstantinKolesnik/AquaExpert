using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Tiles;
using System;

namespace SmartHub.Plugins.MeteoStation
{
    [Tile]
    public class MeteoStationTile : TileBase
    {
        public override void PopulateWebModel(TileWebModel tileWebModel, dynamic options)
        {
            try
            {
                tileWebModel.title = "Метеостанция";
                tileWebModel.url = "webapp/meteostation/module-main";
                tileWebModel.className = "btn-info th-tile-icon th-tile-icon-fa fa-umbrella";
                //tileWebModel.wide = true;
                tileWebModel.content = Context.GetPlugin<MeteoStationPlugin>().BuildTileContent();
                tileWebModel.SignalRReceiveHandler = Context.GetPlugin<MeteoStationPlugin>().BuildSignalRReceiveHandler();
            }
            catch (Exception ex)
            {
                tileWebModel.content = ex.Message;
            }
        }
    }
}
