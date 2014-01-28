using Gadgeteer.Modules.LoveElectronics;
using Microsoft.SPOT;
using System;

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
            //module.TemperatureCurve = module.GetExampleTemperatureCurveDataPoints(PHTemp.RTDType.Pt100);

            var isConnected = module.Connect(MCP342xJumperState.Floating, MCP342xJumperState.Floating);
            if (isConnected)
            {
                //module.SetResolution(MCP324XResolution.TwelveBits);

                while (true)
                {
                    try
                    {
                        Debug.Print("pH = " + module.ReadPH().ToString());
                        Debug.Print("T = " + module.ReadTemperature().ToString());
                    }
                    catch (Exception e)
                    {
                        Debug.Print(e.Message + e.StackTrace);
                    }

                    Debug.Print("-------------------------------");

                    System.Threading.Thread.Sleep(500);
                }
            }
        }
    }
}
