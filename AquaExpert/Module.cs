using System;
using Microsoft.SPOT;

namespace AquaExpert
{
    class Module
    {
        private int address = -1;
        private int relayCount = 0; // 8
        private int waterSensorCount = 0; // 3
        private int phSensorCount = 0; // 2
        private int orpSensorCount = 0; // 2
        private int temperatureSensorCount = 0; // 1//3

        public int Address
        {
            get { return address; }
        }

        public Module(int address)
        {
            this.address = address;
        }
    }
}
