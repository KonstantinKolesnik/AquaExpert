using SmartNetwork.Core.Plugins;
using SmartNetwork.Plugins.Timer;
using System;
//using System.Linq;

namespace SmartNetwork.Plugins.MySensors
{
    [Plugin]
    public class MySensorsPlugin : PluginBase
    {



        public override void InitPlugin()
        {
        }
        public override void StartPlugin()
        {
        }
        public override void StopPlugin()
        {
        }



        [OnTimerElapsed]
	    public void OnTimerElapsed(DateTime now)
	    {
            int a = 0;
            int b = a;
	    }
    }
}
