using Gadgeteer.Modules.Seeed;
using Microsoft.SPOT;
using GT = Gadgeteer;

namespace AquaExpert
{
    class WaterLevelSensor
    {
        private MoistureSensor module;
        private GT.Timer timer;
        private bool isWet = false;
        private int wetnessThreshould = 500; // 0 is fully dry and 1000 (or greater) is completely wet.

        public bool IsWet
        {
            get { return isWet; }
            private set
            {
                if (isWet != value)
                {
                    isWet = value;
                    if (WetnessChanged != null)
                        WetnessChanged(this, EventArgs.Empty);
                }
            }
        }
        public int WetnessThreshould
        {
            get { return wetnessThreshould; }
            set { wetnessThreshould = value; }
        }

        public event EventHandler WetnessChanged;

        public WaterLevelSensor(MoistureSensor module)
        {
            this.module = module;

            timer = new GT.Timer(500);
            timer.Tick += delegate(GT.Timer t)
            {
                IsWet = module.GetMoistureReading() > wetnessThreshould;
            };
            timer.Start();
        }
    }
}
