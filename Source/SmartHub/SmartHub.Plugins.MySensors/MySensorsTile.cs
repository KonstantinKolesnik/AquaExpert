using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Tiles;
using System;

namespace SmartHub.Plugins.MySensors
{
    [Tile]
    public class MySensorsTile : TileBase
    {
        public override void FillModel(TileWeb webTile, dynamic options)
        {
            try
            {
                //Context.GetPlugin<MySensorsPlugin>();

                //UserScript script = GetScript(options.id);

                //var data = Context.GetPlugin<MySensorsPlugin>();


                webTile.title = "Сеть MySensors";
                webTile.url = "/webapp/mysensors/module"; //options.url;
                webTile.className = "btn-info th-tile-icon th-tile-icon-fa fa-cog";
                //webTile.content = "Узлов: 1\nСенсоров: 8";
                webTile.wide = true;
            }
            catch (Exception ex)
            {
                webTile.content = ex.Message;
            }
        }
    }
}
