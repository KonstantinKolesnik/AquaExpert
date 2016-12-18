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
        //public Lazy<ApiCommandMethod, IApiCommandAttribute>[] ApiCommands { get; set; }
        //public ApiCommandMethod[] ApiCommands
        //{
        //    get;
        //    set;
        //}
        public IEnumerable<ApiCommandMethod> ApiCommands
        {
            get; set;
        }

        [OnImportsSatisfied]
        public void OnImportsSatisfied()
        {
            int a = 0;
            int b = a;
        }


        //public IEnumerable<Func<object[], object>> ApiCommands
        //{
        //    get;
        //    set;
        //}
        //public Func<object[], object>[] ApiCommands
        //{
        //    get;
        //    set;
        //}
        //

        //private void NotifyMessageReceivedForPlugins(WemosMessage msg)
        //{
        //    Run(WemosMessageHandlers, x => x(msg));
        //}
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
    }
}
