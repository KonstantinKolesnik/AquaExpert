using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Tiles;
using System;

namespace SmartHub.Plugins.Speech
{
    [Tile]
    public class SpeechTile : TileBase
    {
        public override void PopulateWebModel(TileWebModel tileWebModel, dynamic options)
        {
            try
            {
                tileWebModel.title = "Голосовые команды";
                tileWebModel.url = "webapp/speech/settings";
                tileWebModel.className = "btn-info th-tile-icon th-tile-icon-fa fa-gear";
            }
            catch (Exception ex)
            {
                tileWebModel.content = ex.Message;
            }
        }
    }
}
