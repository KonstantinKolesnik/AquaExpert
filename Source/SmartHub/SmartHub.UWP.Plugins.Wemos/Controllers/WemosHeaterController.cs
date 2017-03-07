using SmartHub.UWP.Plugins.Speech;
using SmartHub.UWP.Plugins.Wemos.Controllers.Models;
using SmartHub.UWP.Plugins.Wemos.Core.Messages;
using SmartHub.UWP.Plugins.Wemos.Core.Models;

namespace SmartHub.UWP.Plugins.Wemos.Controllers
{
    public class WemosHeaterController : WemosControllerBase
    {
        public class ControllerConfiguration
        {
            public int LineTemperatureID
            {
                get; set;
            }
            public int LineSwitchID
            {
                get; set;
            }
            public float TemperatureCalibration
            {
                get; set;
            }

            public float TemperatureMin { get; set; }
            public float TemperatureMax { get; set; }
            public float TemperatureAlarmMin { get; set; }
            public float TemperatureAlarmMax { get; set; }
            public string TemperatureAlarmMinText { get; set; }
            public string TemperatureAlarmMaxText { get; set; }

            public static ControllerConfiguration Default
            {
                get
                {
                    return new ControllerConfiguration()
                    {
                        LineTemperatureID = -1,
                        LineSwitchID = -1,
                        TemperatureCalibration = 0.0f,

                        TemperatureMin = 25.0f,
                        TemperatureMax = 26.0f,
                        TemperatureAlarmMin = 20.0f,
                        TemperatureAlarmMax = 30.0f,
                        TemperatureAlarmMinText = "Критически низкая температура",
                        TemperatureAlarmMaxText = "Критически высокая температура"
                    };
                }
            }
        }

        #region Fields
        private ControllerConfiguration configuration = null;
        protected float? lastSensorValue;
        #endregion

        #region Properties
        public WemosLine LineTemperature
        {
            get { return host.GetLine(configuration.LineSwitchID); }
        }
        public WemosLine LineSwitch
        {
            get { return host.GetLine(configuration.LineSwitchID); }
        }
        #endregion

        #region Constructor
        public WemosHeaterController(WemosController model)
            : base (model)
        {
            if (string.IsNullOrEmpty(model.Configuration))
            {
                configuration = ControllerConfiguration.Default;
                model.SerializeConfiguration(configuration);
            }
            else
                configuration = model.DeserializeConfiguration<ControllerConfiguration>();
        }
        #endregion

        #region Abstract Overrides
        public override bool IsMyMessage(WemosMessage message)
        {
            return
                WemosPlugin.IsMessageFromLine(message, LineSwitch) ||
                WemosPlugin.IsMessageFromLine(message, LineTemperature);
        }
        public async override void RequestLinesValues()
        {
            await host.RequestLineValue(LineSwitch);
            await host.RequestLineValue(LineTemperature);
        }
        protected async override void Process()
        {
            if (lastSensorValue.HasValue)
            {
                float value = lastSensorValue.Value;

                if (value < configuration.TemperatureMin)
                    await host.SetLineValue(LineSwitch, 1);
                else if (value > configuration.TemperatureMax)
                    await host.SetLineValue(LineSwitch, 0);

                // voice alarm:
                if (value <= configuration.TemperatureAlarmMin)
                    context.GetPlugin<SpeechPlugin>().Say(string.Format("{0}, {1} градусов.", configuration.TemperatureAlarmMinText, value));
                else if (value >= configuration.TemperatureAlarmMax)
                    context.GetPlugin<SpeechPlugin>().Say(string.Format("{0}, {1} градусов.", configuration.TemperatureAlarmMaxText, value));
            }
            else
                RequestLinesValues();
        }
        //protected override void InitLastValues()
        //{
        //    var lastSV = mySensors.GetLastSensorValue(LineTemperature);
        //    lastSensorValue = lastSV != null ? lastSV.Value : (float?) null;
        //}
        #endregion

        #region Event handlers
        internal override void MessageCalibration(WemosMessage message)
        {
            if (WemosPlugin.IsMessageFromLine(message, LineTemperature))
                message.Set(message.GetFloat() + configuration.TemperatureCalibration);
        }
        internal override void MessageReceived(WemosMessage message)
        {
            if (WemosPlugin.IsMessageFromLine(message, LineTemperature))
                lastSensorValue = message.GetFloat();

            base.MessageReceived(message);
        }
        #endregion
    }
}
