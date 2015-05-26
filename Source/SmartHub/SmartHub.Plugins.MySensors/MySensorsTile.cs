using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Tiles;
using System;

namespace SmartHub.Plugins.MySensors
{
    [Tile]
    public class MySensorsTile : TileBase
    {
        public override void PopulateWebModel(TileWebModel tileWebModel, dynamic parameters)
        {
            try
            {
                tileWebModel.title = "Сеть MySensors";
                tileWebModel.url = "webapp/mysensors/module";
                tileWebModel.className = "btn-info th-tile-icon th-tile-icon-fa fa-sitemap";
                tileWebModel.content = Context.GetPlugin<MySensorsPlugin>().BuildTileContent();
                tileWebModel.SignalRReceiveHandler = Context.GetPlugin<MySensorsPlugin>().BuildSignalRReceiveHandler();
            }
            catch (Exception ex)
            {
                tileWebModel.content = ex.Message;
            }
        }
    }
}
