using SmartHub.Plugins.WebUI.Tiles;
using System;

namespace SmartHub.Plugins.MySensors
{
    [Tile]
    public class MySensorsTile : TileBase
    {
        public override void FillModel(TileWeb model, dynamic options)
        {
            try
            {
                //Context.GetPlugin<MySensorsPlugin>();

                //UserScript script = GetScript(options.id);

                model.title = "Сеть MySensors";
                model.url = "/webapp/mysensors/module.js";// options.url;
                model.className = "btn-primary th-tile-icon th-tile-icon-fa fa-cog";
                model.content = "Узлов: 1\nСенсоров: 8";
                model.wide = true;
            }
            catch (Exception ex)
            {
                model.content = ex.Message;
            }
        }
    }
}
