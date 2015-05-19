using SmartHub.Plugins.Scripts.Data;
using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Tiles;
using System;

namespace SmartHub.Plugins.Scripts
{
    [Tile]
    public class ScriptsTile : TileBase
    {
        public override void PopulateModel(TileWeb webTile, dynamic options)
        {
            try
            {
                UserScript script = GetScript(options);

                webTile.title = script.Name;
                webTile.content = "Run the script\r\n" + script.Name;
                webTile.cssClassName = "btn-primary th-tile-icon th-tile-icon-fa fa-rocket";
            }
            catch (Exception ex)
            {
                webTile.content = ex.Message;
            }
        }
        public override string ExecuteAction(object options)
        {
            try
            {
                UserScript script = GetScript(options);

                Context.GetPlugin<ScriptsPlugin>().ExecuteScript(script);
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private UserScript GetScript(dynamic options)
        {
            string strId = options.id;
            if (string.IsNullOrWhiteSpace(strId))
                throw new Exception("Missing id parameter");

            Guid scriptId;
            if (!Guid.TryParse(strId, out scriptId))
                throw new Exception("Id parameter must contain GUID value");

            using (var session = Context.OpenSession())
            {
                var script = session.Get<UserScript>(scriptId);

                if (script == null)
                    throw new Exception(string.Format("Script '{0}' is not found", scriptId));

                return script;
            }
        }
    }
}
