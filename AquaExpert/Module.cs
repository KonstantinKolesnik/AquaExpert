using System;
using Microsoft.SPOT;

namespace AquaExpert
{
    class Module
    {
        private int address = -1;

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
