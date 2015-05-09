using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.Plugins.AquaController
{
    [Tile]
    public class AquaControllerTile : TileBase
    {
        public override void PopulateModel(TileWeb webTile, dynamic options)
        {
            try
            {
                //Context.GetPlugin<MySensorsPlugin>();

                //UserScript script = GetScript(options.id);

                //var data = Context.GetPlugin<MySensorsPlugin>();


                webTile.title = "Aqua Controller";
                webTile.url = "/webapp/aquacontroller/module.js"; //options.url;
                webTile.cssClassName = "btn-info th-tile-icon th-tile-icon-fa fa-cog";
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
