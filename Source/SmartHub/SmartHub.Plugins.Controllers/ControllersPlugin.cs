using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using SmartHub.Core.Plugins;
using SmartHub.Core.Plugins.Utils;
using SmartHub.Plugins.Controllers.Core;
using SmartHub.Plugins.Controllers.Data;
using SmartHub.Plugins.HttpListener.Api;
using SmartHub.Plugins.HttpListener.Attributes;
using SmartHub.Plugins.MySensors;
using SmartHub.Plugins.MySensors.Attributes;
using SmartHub.Plugins.MySensors.Core;
using SmartHub.Plugins.SignalR;
using SmartHub.Plugins.Timer.Attributes;
using SmartHub.Plugins.WebUI.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartHub.Plugins.Controllers
{
    [AppSection("Контроллеры", SectionType.System, "/webapp/controllers/settings.js", "SmartHub.Plugins.Controllers.Resources.js.settings.js", TileTypeFullName = "SmartHub.Plugins.Controllers.ControllersTile")]
    [JavaScriptResource("/webapp/controllers/settings-view.js", "SmartHub.Plugins.Controllers.Resources.js.settings-view.js")]
    [JavaScriptResource("/webapp/controllers/settings-model.js", "SmartHub.Plugins.Controllers.Resources.js.settings-model.js")]
    [HttpResource("/webapp/controllers/settings.html", "SmartHub.Plugins.Controllers.Resources.js.settings.html")]

    // controller editor
    [JavaScriptResource("/webapp/controllers/controller-editor.js", "SmartHub.Plugins.Controllers.Resources.js.controller-editor.js")]
    [JavaScriptResource("/webapp/controllers/controller-editor-view.js", "SmartHub.Plugins.Controllers.Resources.js.controller-editor-view.js")]
    [JavaScriptResource("/webapp/controllers/controller-editor-model.js", "SmartHub.Plugins.Controllers.Resources.js.controller-editor-model.js")]
    [HttpResource("/webapp/controllers/controller-editor.html", "SmartHub.Plugins.Controllers.Resources.js.controller-editor.html")]

    [JavaScriptResource("/webapp/controllers/utils.js", "SmartHub.Plugins.Controllers.Resources.js.utils.js")]
    [HttpResource("/webapp/controllers/utils.html", "SmartHub.Plugins.Controllers.Resources.js.utils.html")]

    [Plugin]
    public class ControllersPlugin : PluginBase
    {
        #region Fields
        private MySensorsPlugin mySensors;
        private List<ControllerBase> controllers = new List<ControllerBase>();
        #endregion

        #region SignalR events
        private void NotifyForSignalR(object msg)
        {
            Context.GetPlugin<SignalRPlugin>().Broadcast(msg);
        }
        #endregion

        #region Plugin overrides
        public override void InitDbModel(ModelMapper mapper)
        {
            mapper.Class<Controller>(cfg => cfg.Table("Controllers_Controllers"));
        }
        public override void InitPlugin()
        {
            mySensors = Context.GetPlugin<MySensorsPlugin>();

            var ctrls = Get();
            foreach (var ctrl in ctrls)
            {
                ControllerBase controller = ConvertController(ctrl);
                if (controller != null)
                    controllers.Add(controller);
            }

            foreach (ControllerBase controller in controllers)
                controller.Init(Context);
        }
        #endregion

        #region API
        public List<Controller> Get()
        {
            using (var session = Context.OpenSession())
                return session.Query<Controller>()
                    .OrderBy(controller => controller.Name)
                    .ToList();
        }
        public Controller Get(Guid id)
        {
            using (var session = Context.OpenSession())
                return session.Get<Controller>(id);
        }
        #endregion

        #region Private methods
        private static ControllerBase ConvertController(Controller controller)
        {
            switch (controller.Type)
            {
                case ControllerType.Heater: return new HeaterController(controller);
                case ControllerType.ScheduledSwitch: return new SwitchController(controller);

                default: return null;
            }
        }
        #endregion

        #region Event handlers
        [MySensorsConnected]
        private void Connected()
        {
            foreach (ControllerBase controller in controllers)
            {
                controller.RequestSensorsValues(); // force sensors to report their current values
                controller.SendSensorsValues(); // e.g. to nodes with display
            }
        }

        [MySensorsMessageCalibration]
        private void MessageCalibration(SensorMessage message)
        {
            foreach (ControllerBase controller in controllers)
                controller.MessageCalibration(message);
        }

        [MySensorsMessage]
        private void MessageReceived(SensorMessage message)
        {
            foreach (ControllerBase controller in controllers)
                controller.MessageReceived(message);
        }

        //[RunPeriodically(1)]
        [Timer_10_sec_Elapsed]
        private void timer_Elapsed(DateTime now)
        {
            foreach (ControllerBase controller in controllers)
                controller.TimerElapsed(now);
        }
        #endregion

        #region Web API
        public object BuildControllerWebModel(Controller controller)
        {
            if (controller == null)
                return null;

            return new
            {
                Id = controller.Id,
                Name = controller.Name,
                Type = (int)controller.Type,
                TypeName = controller.Type.GetEnumDescription(),
                IsAutoMode = controller.IsAutoMode,
                Configuration = controller.Configuration
            };
        }

        [HttpCommand("/api/controllers/controllertype/list")]
        private object apiGetControllerTypes(HttpRequestParams request)
        {
            return Enum.GetValues(typeof(ControllerType)).Cast<ControllerType>()
                .Select(v => new
                {
                    Id = v,
                    Name = v.GetEnumDescription(),
                }).ToArray();
        }
        
        [HttpCommand("/api/controllers/list")]
        private object apiGetControllers(HttpRequestParams request)
        {
            return Get()
                .Select(BuildControllerWebModel)
                .Where(x => x != null)
                .ToArray();
        }
        [HttpCommand("/api/controllers/get")]
        private object apiGetController(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");

            return BuildControllerWebModel(Get(id));
        }
        
        [HttpCommand("/api/controllers/add")]
        private object apiAddController(HttpRequestParams request)
        {
            var name = request.GetRequiredString("name");
            var type = (ControllerType)request.GetRequiredInt32("type");

            var ctrl = new Controller()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Type = type,
                IsAutoMode = true
            };

            ControllerBase controller = ConvertController(ctrl);
            if (controller != null)
            {
                controller.Init(Context);
                controller.SaveToDB();
                controllers.Add(controller);

                NotifyForSignalR(new { MsgId = "ControllerAdded", Data = BuildControllerWebModel(ctrl) });
            }

            return null;
        }
        [HttpCommand("/api/controllers/setname")]
        private object apiSetControllerName(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            var name = request.GetRequiredString("name");

            foreach (ControllerBase controller in controllers)
                if (controller.ID == id)
                {
                    controller.Name = name;
                    break;
                }

            //NotifyForSignalR(new { MsgId = "ControllerNameChanged", Data = BuildControllerWebModel(ctrl) });

            return null;
        }
        [HttpCommand("/api/controllers/setisautomode")]
        private object apiSetControllerIsAutoMode(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            var isAutoMode = request.GetRequiredBool("isAutoMode");

            foreach (ControllerBase controller in controllers)
                if (controller.ID == id)
                {
                    controller.IsAutoMode = isAutoMode;
                    break;
                }

            //NotifyForSignalR(new { MsgId = "ControllerIsAutoModeChanged", Data = BuildControllerWebModel(ctrl) });

            return null;
        }
        [HttpCommand("/api/controllers/setconfiguration")]
        private object apiSetControllerConfiguration(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            var conf = request.GetRequiredString("config");

            foreach (ControllerBase controller in controllers)
                if (controller.ID == id)
                {
                    controller.SetConfiguration(conf);
                    break;
                }

            //NotifyForSignalR(new { MsgId = "ControllerIsVisibleChanged", Data = BuildControllerWebModel(ctrl) });

            return null;
        }
        [HttpCommand("/api/controllers/delete")]
        private object apiDeleteController(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("Id");

            using (var session = Context.OpenSession())
            {
                var ctrl = session.Load<Controller>(id);
                session.Delete(ctrl);
                session.Flush();

                NotifyForSignalR(new { MsgId = "ControllerDeleted", Data = new { Id = id } });
            }

            foreach (ControllerBase controller in controllers)
                if (controller.ID == id)
                {
                    controllers.Remove(controller);
                    break;
                }

            return null;
        }
        #endregion
    }
}
