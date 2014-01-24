using System;
using Microsoft.SPOT;
using Gadgeteer.Modules.LoveElectronics;

namespace AquaExpert.Sensors
{
    class PHTempSensor
    {
        private PHTemp module;

        public double PH
        {
            get { return module.ReadPH(); }
        }
        public double Temperature
        {
            get { return module.ReadTemperature(); }
        }

        public PHTempSensor(PHTemp module)
        {
            this.module = module;

            module.phOffset = 0;
            module.phSlope = 58.7;
            module.TemperatureCurve = module.GetExampleTemperatureCurveDataPoints(PHTemp.RTDType.Pt100);
            
            var isConnected = module.Connect(PHTemp.JumperState.Floating, PHTemp.JumperState.Floating);
            if (isConnected)
            {
                module.SetResolution(MCP324XResolution.TwelveBits);
                module.SaveSettingsToFlash();

                try
                {
                    Debug.Print(module.ReadTemperature().ToString());
                    //Debug.Print(module.ReadPH().ToString());
                }
                catch (Exception e)
                {
                    Debug.Print(e.Message + e.StackTrace);
                }
            }
            
            // See more at: https://www.ghielectronics.com/community/forum/topic?id=12045#sthash.4hIYQ09W.dpuf
        }
    }
}
