using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using SmartHub.Core.Plugins;
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
using System.ComponentModel;
using System.Linq;
using System.Reflection;

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

            var ctrls = GetControllers();
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

        #region Public methods


        #endregion

        #region Private methods
        private static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
        private static ControllerBase ConvertController(Controller controller)
        {
            switch (controller.Type)
            {
                case ControllerType.Heater: return new HeaterController(controller);
                case ControllerType.Switch: return new SwitchController(controller);

                default: return null;
            }
        }

        private List<Controller> GetControllers()
        {
            using (var session = Context.OpenSession())
                return session.Query<Controller>()
                    .OrderBy(controller => controller.Name)
                    .ToList();
        }

        private object BuildControllerWebModel(Controller controller)
        {
            if (controller == null)
                return null;

            return new
            {
                Id = controller.Id,
                Name = controller.Name,
                Type = (int)controller.Type,
                TypeName = GetEnumDescription(controller.Type),
                Configuration = controller.Configuration
            };
        }
        private object BuildControllerRichWebModel(Controller controller)
        {
            if (controller == null)
                return null;

            return new
            {
                Id = controller.Id,
                Name = controller.Name,
                Type = (int)controller.Type,
                TypeName = GetEnumDescription(controller.Type),
                Configuration = controller.Configuration
            };
        }
        #endregion

        #region Event handlers
        [MySensorsConnected]
        private void Connected()
        {
            foreach (ControllerBase controller in controllers)
                controller.RequestSensorsValues();
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
        [HttpCommand("/api/controllers/controllertype/list")]
        private object apiGetControllerTypes(HttpRequestParams request)
        {
            return Enum.GetValues(typeof(ControllerType))
                .Cast<ControllerType>()
                .Select(v => new
                {
                    Id = v,
                    Name = GetEnumDescription(v)
                }).ToArray();
        }
        [HttpCommand("/api/controllers/list")]
        private object apiGetControllers(HttpRequestParams request)
        {
            return GetControllers()
                .Select(BuildControllerWebModel)
                .Where(x => x != null)
                .ToArray();
        }
        [HttpCommand("/api/controllers/list/dashboard")]
        private object apiGetControllersForDashboard(HttpRequestParams request)
        {
            return GetControllers()
                .Select(BuildControllerRichWebModel)
                .Where(x => x != null)
                .ToArray();
        }
        [HttpCommand("/api/controllers/get")]
        private object apiGetController(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");

            using (var session = Context.OpenSession())
                return BuildControllerWebModel(session.Get<Controller>(id));
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
                Type = type
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

            using (var session = Context.OpenSession())
            {
                var ctrl = session.Load<Controller>(id);
                ctrl.Name = name;
                session.Flush();

                NotifyForSignalR(new { MsgId = "ControllerNameChanged", Data = BuildControllerWebModel(ctrl) });
            }

            return null;
        }
        [HttpCommand("/api/controllers/setconfiguration")]
        private object apiSetControllerConfiguration(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            var conf = request.GetRequiredString("config");

            foreach (ControllerBase controller in controllers)
                if (controller.ControllerID == id)
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

            return null;
        }
        #endregion
    }
}
