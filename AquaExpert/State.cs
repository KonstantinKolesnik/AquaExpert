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

        public int Temperature;
        public int PH;

        public bool IsManualMode = false;
    }
}
