using SmartHub.UWP.Plugins.Speech;
using SmartHub.UWP.Plugins.Wemos.Controllers.Models;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
using System;

namespace SmartHub.UWP.Plugins.Wemos.Controllers
{
    public class WemosHeaterController : WemosControllerBase
    {
        public class ControllerConfiguration
        {
            public int LineTemperatureID
            {
                get; set;
            } = -1;
            public int LineSwitchID
            {
                get; set;
            } = -1;

            public float TemperatureMin { get; set; } = 25.0f;
            public float TemperatureMax { get; set; } = 26.0f;
            public float TemperatureAlarmMin { get; set; } = 20.0f;
            public float TemperatureAlarmMax { get; set; } = 30.0f;
            public string TemperatureAlarmMinText { get; set; } = "Критически низкая температура";
            public string TemperatureAlarmMaxText { get; set; } = "Критически высокая температура";
        }

        #region Fields
        protected float? lastLineValue;
        #endregion

        #region Properties
        private WemosLine LineTemperature
        {
            get { return host.GetLine((Configuration as ControllerConfiguration).LineTemperatureID); }
        }
        private WemosLine LineSwitch
        {
            get { return host.GetLine((Configuration as ControllerConfiguration).LineSwitchID); }
        }
        #endregion

        #region Constructor
        public WemosHeaterController(WemosController model)
            : base (model)
        {
        }
        #endregion

        #region Abstract Overrides
        protected override Type GetConfigurationType()
        {
            return typeof(ControllerConfiguration);
        }
        protected override object GetDefaultConfiguration()
        {
            return new ControllerConfiguration();
        }

        protected override bool IsMyMessage(WemosLineValue value)
        {
            return
                WemosPlugin.IsMessageFromLine(value, LineSwitch) ||
                WemosPlugin.IsMessageFromLine(value, LineTemperature);
        }
        protected async override void RequestLinesValues()
        {
            await host.RequestLineValue(LineSwitch);
            await host.RequestLineValue(LineTemperature);
        }
        protected async override void Process()
        {
            if (lastLineValue.HasValue)
            {
                float value = lastLineValue.Value;
                var config = Configuration as ControllerConfiguration;

                if (value < config.TemperatureMin)
                    await host.SetLineValue(LineSwitch, 1);
                else if (value > config.TemperatureMax)
                    await host.SetLineValue(LineSwitch, 0);

                // voice alarm:
                if (value <= config.TemperatureAlarmMin)
                    context.GetPlugin<SpeechPlugin>().Say(string.Format("{0}, {1} градусов.", config.TemperatureAlarmMinText, value));
                else if (value >= config.TemperatureAlarmMax)
                    context.GetPlugin<SpeechPlugin>().Say(string.Format("{0}, {1} градусов.", config.TemperatureAlarmMaxText, value));
            }
            else
                RequestLinesValues();
        }
        protected override void MessageReceived(WemosLineValue value)
        {
            if (WemosPlugin.IsMessageFromLine(value, LineTemperature))
                lastLineValue = value.Value;
        }
        //protected override void InitLastValues()
        //{
        //    var lastSV = mySensors.GetLastSensorValue(LineTemperature);
        //    lastLineValue = lastSV != null ? lastSV.Value : (float?) null;
        //}
        #endregion
    }
}
