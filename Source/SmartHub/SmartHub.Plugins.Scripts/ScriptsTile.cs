using SmartHub.Plugins.Scripts.Data;
using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Tiles;
using System;

namespace SmartHub.Plugins.Scripts
{
    [Tile]
    public class ScriptsTile : TileBase
    {
        public override void PopulateWebModel(TileWebModel tileWebModel, dynamic parameters)
        {
            try
            {
                UserScript script = GetScript(parameters);

                tileWebModel.title = script.Name;
                tileWebModel.content = "Выполнить скрипт\r\n" + script.Name;
                tileWebModel.className = "btn-primary th-tile-icon th-tile-icon-fa fa-file-code-o";
            }
            catch (Exception ex)
            {
                tileWebModel.content = ex.Message;
            }
        }
        public override string ExecuteAction(object parameters)
        {
            try
            {
                UserScript script = GetScript(parameters);
                Context.GetPlugin<ScriptsPlugin>().ExecuteScript(script);

                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private UserScript GetScript(dynamic parameters)
        {
            string strId = parameters.id;
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
