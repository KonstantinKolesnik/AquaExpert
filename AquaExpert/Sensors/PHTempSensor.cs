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
            Mcp342x adc = new Mcp342x(MCP342XChannel.Channel2, MCP324XResolution.TwelveBits, MCP342XConversionMode.Continuous, MCP342XGain.x1);
            while (true)
            {
                //Debug.Print("Input voltage= " + adc.ReadVolts().ToString());
                //Debug.Print("Input = " + ReadTemperature(adc.ReadVolts()).ToString());

                adc.SetInputChannel(MCP342XChannel.Channel1);
                Debug.Print("V1 = " + adc.ReadVolts().ToString());
                adc.SetInputChannel(MCP342XChannel.Channel2);
                Debug.Print("V2 = " + adc.ReadVolts().ToString());
                adc.SetInputChannel(MCP342XChannel.Channel3);
                Debug.Print("V3 = " + adc.ReadVolts().ToString());
                adc.SetInputChannel(MCP342XChannel.Channel4);
                Debug.Print("V4 = " + adc.ReadVolts().ToString());
                Debug.Print("-------------------------------");


                System.Threading.Thread.Sleep(100);
            }
            return;


            this.module = module;

            module.phOffset = 0;
            module.phSlope = 58.7;
            module.TemperatureCurve = module.GetExampleTemperatureCurveDataPoints(PHTemp.RTDType.Pt100);
            
            var isConnected = module.Connect(PHTemp.JumperState.Floating, PHTemp.JumperState.Floating);
            if (isConnected)
            {
                //module.SetResolution(MCP324XResolution.TwelveBits);

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

        public double ReadTemperature(double mV)
        {
            return mV;

            double volts = mV / 1000;
            double r1 = 1000;
            double r2 = 1000;
            double r3 = 1000;
            double t1 = (r1 + r2) * (volts / 2.048);
            double rx = (r1 * r3 + r3 * t1) / (r2 - t1);

            //double k = 2.048 / volts;
            //rx = (r3 * (r1 + r2) + k * r2) / (k * r1 - (r1 + r2));
            
            return rx;



            //if (rx <= settings.TemperatureCurve[0].Resistance)
            //    throw new Exception(string.Concat("The resistance (", rx, " Ohms) was out of range of the supplied temperature curve."));

            //var tempCurve = settings.TemperatureCurve;
            //double deltaTemp = 0;
            //double temp = (double)tempCurve[0].Temperature;

            //int i = 1;
            //while (i < tempCurve.Length)
            //{
            //    deltaTemp = (double)(tempCurve[i].Temperature - tempCurve[i - 1].Temperature);
            //    if (rx >= tempCurve[i].Resistance)
            //    {
            //        temp += deltaTemp;
            //        i++;
            //    }
            //    else
            //        return temp + (rx - tempCurve[i - 1].Resistance) * deltaTemp / (tempCurve[i].Resistance - tempCurve[i - 1].Resistance);
            //}

            //return temp;
        }

    }
}
