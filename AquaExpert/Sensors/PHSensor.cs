using System;
using Microsoft.SPOT;
using Gadgeteer.Modules.LoveElectronics;

namespace AquaExpert.Sensors
{
    class PHSensor
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

        public PHSensor(PHTemp module)
        {
            this.module = module;

            module.phOffset = 0;
            module.phSlope = 58.7;
            module.TemperatureCurve = module.GetExampleTemperatureCurveDataPoints(PHTemp.ExampleRTDCurve.Pt1000);
            var isConnected = module.Connect(PHTemp.AddressPin.Floating, PHTemp.AddressPin.Floating);
            if (isConnected)
            {
                module.SetResolution(MCP324XResolution.TwelveBits);
                module.SaveSettings();
                
                //try
                //{
                //    Debug.Print("Trying...");
                //    Debug.Print(module.ReadPH().ToString());
                //    Debug.Print(module.ReadTemperature().ToString());
                //}
                //catch (Exception e)
                //{
                //    Debug.Print(e.Message + e.StackTrace);
                //}
            }
            
            // See more at: https://www.ghielectronics.com/community/forum/topic?id=12045#sthash.4hIYQ09W.dpuf
        }
    }
}
