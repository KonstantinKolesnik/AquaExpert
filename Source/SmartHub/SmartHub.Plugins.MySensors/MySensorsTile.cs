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
                //var plugin = Context.GetPlugin<MySensorsPlugin>();
                //using (var session = plugin.Context.OpenSession())
                //return session.Query<Node>().Select(BuildNodeModel).Where(x => x != null).ToArray();


                tileWebModel.title = "Сеть MySensors";
                tileWebModel.url = "/webapp/mysensors/module.js";
                tileWebModel.className = "btn-info th-tile-icon th-tile-icon-fa fa-share-alt";
                tileWebModel.SignalRReceiveHandler = "function onSignalR(data) { item.set({ 'content': new Date(data.Value) }); }";
            }
            catch (Exception ex)
            {
                tileWebModel.content = ex.Message;
            }
        }
    }
}
