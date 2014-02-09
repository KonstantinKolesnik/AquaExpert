using MFE.Hardware;
using Microsoft.SPOT.Hardware;

namespace AquaExpert.Managers
{
    public class Module
    {
        #region Commands
        public const int CMD_GET_SUMMARY = 0;
        public const int CMD_GET_RELAY_STATE = 1;
        public const int CMD_SET_RELAY_STATE = 2;

        #endregion

        #region Fields
        private ushort address = 0;
        private byte type = 0;
        private int relayCount = 0;
        private int waterSensorCount = 0;
        private int phSensorCount = 0;
        private int orpSensorCount = 0;
        private int temperatureSensorCount = 0;
        #endregion

        #region Properties
        public ushort Address
        {
            get { return address; }
        }
        public string Name
        {
            get
            {
                switch (type)
                {
                    case 0: return "Test module";
                    case 1: return "AE-R8";

                    default: return type.ToString();// "[Unknown]";
                }
            }
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
        public Module(ushort address, byte type)
        {
            this.address = address;
            this.type = type;
        }
        #endregion

        public void GetRelayState(int idx)
        {
            I2CDevice.Configuration config = new I2CDevice.Configuration(address, Program.BusClockRate);
            //if (bus.TrySetRegister(config, timeout, CMD_GET_RELAY_STATE, new byte[] { 0, (byte)(on ? 1 : 0) }))
            //{
            //}
        }
        public void SetRelayState(int idx, bool on)
        {
            I2CDevice.Configuration config = new I2CDevice.Configuration(address, Program.BusClockRate);
            if (Program.Bus.TrySetRegister(config, Program.BusTimeout, CMD_SET_RELAY_STATE, new byte[] { 0, (byte)(on ? 1 : 0) }))
            {
            }
        }
    }
}
