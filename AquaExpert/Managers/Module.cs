
namespace AquaExpert.Managers
{
    class Module
    {
        #region Commands
        public const int CMD_GET_PROPERTIES = 0;
        public const int CMD_GET_RELAY_STATE = 1;
        public const int CMD_SET_RELAY_STATE = 2;

        #endregion

        #region Fields
        private ushort address = 0;
        private int relayCount = 0; // 8
        private int waterSensorCount = 0; // 3
        private int phSensorCount = 0; // 2
        private int orpSensorCount = 0; // 2
        private int temperatureSensorCount = 0; // 1//3
        #endregion

        #region Properties
        public ushort Address
        {
            get { return address; }
        }
        public int RelayCount
        {
            get { return relayCount; }
            set { relayCount = value; }
        }
        public int WaterSensorCount
        {
            get { return waterSensorCount; }
            set { waterSensorCount = value; }
        }
        public int PhSensorCount
        {
            get { return phSensorCount; }
            set { phSensorCount = value; }
        }
        public int OrpSensorCount
        {
            get { return orpSensorCount; }
            set { orpSensorCount = value; }
        }
        public int TemperatureSensorCount
        {
            get { return temperatureSensorCount; }
            set { temperatureSensorCount = value; }
        }
        #endregion

        #region Constructor
        public Module(ushort address)
        {
            this.address = address;
        }
        #endregion

        //public bool GetRelayState(int idx)
        //{

        //}
    }
}
