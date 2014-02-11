using System;
using Microsoft.SPOT;

namespace AquaExpert
{
    public class State
    {
        public bool IsLightOn = false;
        public bool IsCO2On = false;
        public bool IsHeaterOn = false;
        public bool IsWaterOutMode = false;
        public bool IsWaterInMode = false;

        public double Temperature = 0;
        public double PH = 0;

        public bool IsManualMode = false;
    }
}
