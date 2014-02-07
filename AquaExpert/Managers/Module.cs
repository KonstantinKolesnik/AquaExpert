using System;
using Microsoft.SPOT;

namespace AquaExpert.Managers
{
    class Module
    {
        private ushort address = 0;
        private int relayCount = 0; // 8
        private int waterSensorCount = 0; // 3
        private int phSensorCount = 0; // 2
        private int orpSensorCount = 0; // 2
        private int temperatureSensorCount = 0; // 1//3

        public ushort Address
        {
            get { return address; }
        }

        public Module(ushort address)
        {
            this.address = address;
        }
    }
}
