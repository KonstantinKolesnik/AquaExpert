using SmartHub.UWP.Core.Communication;
using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.ApiListener.Attributes;
using System;
using System.Collections.Generic;
using System.Composition;

namespace SmartHub.UWP.Plugins.ApiListener
{
    [Plugin]
    public class ApiListenerPlugin : PluginBase
    {
        public const string ServiceName = "9999";

        #region Fields
        private StreamListener listener;
        #endregion

        #region Imports
        [ImportMany]
        public IEnumerable<Lazy<Func<object[], object>, ApiCommandAttribute>> ApiCommands { get; set; }
        #endregion

        #region Plugin ovverrides
        public override void InitPlugin()
        {
            listener = new StreamListener();
            listener.DataReceived += Listener_DataReceived;
        }
        public override void StartPlugin()
        {
            listener.Start(ServiceName);
        }
        public override void StopPlugin()
        {
            listener.Stop();
        }
        #endregion

        #region Event handlers
        private void Listener_DataReceived(object sender, EventArgs e)
        {
            
        }
        #endregion

        private void ExecuteApiCommand()
        {
            //switch (operation.Name)
            //{
            //    case "+":
            //        foreach (var addMethod in AddMethods)
            //        {
            //            if (addMethod.Metadata.Speed == Speed.Fast)
            //            {
            //                result = addMethod.Value.Operation(operands[0], operands[1]);
            //            }
            //        }
            //        break
        }
    }
}
