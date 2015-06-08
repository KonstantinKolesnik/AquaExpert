﻿using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Tiles;
using System;

namespace SmartHub.Plugins.Management
{
    [Tile]
    public class ManagementTile : TileBase
    {
        public override void PopulateWebModel(TileWebModel tileWebModel, dynamic options)
        {
            try
            {
                tileWebModel.title = "Менеджмент";
                tileWebModel.url = "webapp/management/settings";
                tileWebModel.className = "btn-info th-tile-icon th-tile-icon-fa fa-gear";
                tileWebModel.content = Context.GetPlugin<ManagementPlugin>().BuildTileContent();
                tileWebModel.SignalRReceiveHandler = Context.GetPlugin<ManagementPlugin>().BuildSignalRReceiveHandler();
            }
            catch (Exception ex)
            {
                tileWebModel.content = ex.Message;
            }
        }
    }
}
