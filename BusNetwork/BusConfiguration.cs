using Microsoft.SPOT.Hardware;

namespace BusNetwork
{
    public class BusConfiguration
    {
        public const int ClockRate = 400; // 400 kHz
        public const int Timeout = 1000; // 1 sec
        
        public I2CDevice Bus;

        public BusConfiguration(I2CDevice bus)
        {
            Bus = bus;
        }
    }
}
