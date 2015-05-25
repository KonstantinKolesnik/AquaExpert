using NHibernate.Linq;
using SmartHub.Plugins.MySensors.Data;
using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Tiles;
using System;
using System.Linq;
using System.Text;

namespace SmartHub.Plugins.MySensors
{
    [Tile]
    public class MySensorsTile : TileBase
    {
        public override void PopulateWebModel(TileWebModel tileWebModel, dynamic parameters)
        {
            try
            {
                tileWebModel.title = "Сеть MySensors";
                tileWebModel.url = "webapp/mysensors/module";
                tileWebModel.className = "btn-info th-tile-icon th-tile-icon-fa fa-sitemap";
                tileWebModel.content = BuildContent();
                tileWebModel.SignalRReceiveHandler = BuildSignalRReceiveHandler();
            }
            catch (Exception ex)
            {
                tileWebModel.content = ex.Message;
            }
        }

        private string BuildContent()
        {
            string result = "";

            SensorValue lastSV = null;
            using (var session = Context.OpenSession())
                lastSV = session.Query<SensorValue>().OrderByDescending(sv => sv.TimeStamp).FirstOrDefault();

            if (lastSV != null)
            {
                result += string.Format("<span>{0:dd.MM.yyyy}</span>&nbsp;&nbsp;<span style='font-size:0.9em; font-style:italic;'>{0:HH:mm:ss}</span>", lastSV.TimeStamp);
                result += string.Format("<div>[{0}][{1}] {2}: {3}</div>", lastSV.NodeNo, lastSV.SensorNo, lastSV.Type.ToString(), lastSV.Value);
            }

            return result;
        }
        private string BuildSignalRReceiveHandler()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("if (data.MsgId == 'SensorValue') { ");
            sb.Append("var dt = kendo.toString(new Date(data.Data.TimeStamp), 'dd.MM.yyyy'); ");
            sb.Append("var tm = kendo.toString(new Date(data.Data.TimeStamp), 'HH:mm:ss'); ");
            sb.Append("var val = '[' + data.Data.NodeNo + '][' + data.Data.SensorNo + '] ' + data.Data.TypeName + ': ' + data.Data.Value; ");
            sb.Append("var result = '<span>' + dt + '</span>&nbsp;&nbsp;'; ");
            sb.Append("result += '<span style=\"font-size:0.9em; font-style:italic;\">' + tm + '</span>'; ");
            sb.Append("result += '<div>' + val + '</div>'; ");
            sb.Append("model.tileModel.set({ 'content': result }); ");
            sb.Append("}");

            return sb.ToString();
        }
    }
}
