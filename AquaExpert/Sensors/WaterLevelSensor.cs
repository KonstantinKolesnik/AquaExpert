using Gadgeteer.Modules.Seeed;

namespace AquaExpert.Sensors
{
    class WaterLevelSensor
    {
        private MoistureSensor module;
        private int wetnessThreshould = 500; // 0 is fully dry and 1000 (or greater) is completely wet.

        public bool IsWet
        {
            get { return module.GetMoistureReading() > wetnessThreshould; }
        }
        public int WetnessThreshould
        {
            get { return wetnessThreshould; }
            set { wetnessThreshould = value; }
        }

        public WaterLevelSensor(MoistureSensor module)
        {
            this.module = module;
        }
    }
}
