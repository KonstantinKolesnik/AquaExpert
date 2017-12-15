using SmartHub.UWP.Plugins.UI.Attributes;
using SmartHub.UWP.Plugins.UI.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.UWP.Plugins.Wemos
{
    [Tile]
    public class WemosTile : TileBase
    {
        //public override void PopulateWebModel(TileWebModel tileWebModel, dynamic parameters)
        //{
        //    try
        //    {
        //        tileWebModel.title = "Сеть MySensors";
        //        tileWebModel.url = "webapp/mysensors/settings";
        //        tileWebModel.className = "btn-info th-tile-icon th-tile-icon-fa fa-sitemap";
        //        tileWebModel.content = Context.GetPlugin<MySensorsPlugin>().BuildTileContent();
        //        tileWebModel.SignalRReceiveHandler = Context.GetPlugin<MySensorsPlugin>().BuildSignalRReceiveHandler();
        //    }
        //    catch (Exception ex)
        //    {
        //        tileWebModel.content = ex.Message;
        //    }
        //}
    }
}
