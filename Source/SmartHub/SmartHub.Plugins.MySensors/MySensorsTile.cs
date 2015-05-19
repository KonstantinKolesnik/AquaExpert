using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Tiles;
using System;

namespace SmartHub.Plugins.MySensors
{
    [Tile]
    public class MySensorsTile : TileBase
    {
        public override void PopulateModel(TileWeb webTile, dynamic options)
        {
            try
            {
                //var plugin = Context.GetPlugin<MySensorsPlugin>();
                //using (var session = plugin.Context.OpenSession())
                //return session.Query<Node>().Select(BuildNodeModel).Where(x => x != null).ToArray();


                webTile.title = "Сеть MySensors";
                webTile.url = "/webapp/mysensors/module.js"; //options.url;
                webTile.className = "btn-info th-tile-icon th-tile-icon-fa fa-share-alt";
                //webTile.content = "Узлов: 1\nСенсоров: 8";
                webTile.SignalRReceiveHandler = "function onSignalR(data) { item.set({ 'content': new Date(data.Value) }); }";
            }
            catch (Exception ex)
            {
                webTile.content = ex.Message;
            }
        }
    }
}
