using MFE.Hardware;
using Microsoft.SPOT.Hardware;
using System.Collections;

namespace AquaExpert.Managers
{
    public class Module
    {
        #region Commands
        public const int CMD_GET_SUMMARY = 0;
        public const int CMD_GET_RELAY_STATE = 1;
        public const int CMD_SET_RELAY_STATE = 2;
        public const int CMD_GET_TEMPERATURE = 3;

        #endregion

        #region Fields
        private ushort address = 0;
        private byte type = 0;

        private int relayCount = 0;
        private int waterSensorCount = 0;
        private int phSensorCount = 0;
        private int orpSensorCount = 0;
        private int conductivitySensorCount = 0;
        private int temperatureSensorCount = 0;
        private int dimmerCount = 0;
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
        public int ConductivitySensorCount
        {
            get { return conductivitySensorCount; }
            set { conductivitySensorCount = value; }
        }
        public int TemperatureSensorCount
        {
            get { return temperatureSensorCount; }
            set { temperatureSensorCount = value; }
        }
        public int DimmerCount
        {
            get { return dimmerCount; }
            set { dimmerCount = value; }
        }

        public ArrayList ControlLines
        {
            get
            {
                ArrayList list = new ArrayList();

                for (int i = 0; i < relayCount; i++)
                    list.Add(new ModuleControlLine(address, ModulePortType.Relay, i));
                for (int i = 0; i < waterSensorCount; i++)
                    list.Add(new ModuleControlLine(address, ModulePortType.WaterSensor, i));
                for (int i = 0; i < phSensorCount; i++)
                    list.Add(new ModuleControlLine(address, ModulePortType.PHSensor, i));
                for (int i = 0; i < orpSensorCount; i++)
                    list.Add(new ModuleControlLine(address, ModulePortType.ORPSensor, i));
                for (int i = 0; i < conductivitySensorCount; i++)
                    list.Add(new ModuleControlLine(address, ModulePortType.ConductivitySensor, i));
                for (int i = 0; i < temperatureSensorCount; i++)
                    list.Add(new ModuleControlLine(address, ModulePortType.TemperatureSensor, i));
                for (int i = 0; i < dimmerCount; i++)
                    list.Add(new ModuleControlLine(address, ModulePortType.Dimmer, i));

                return list;
            }
        }
        #endregion

        #region Constructor
        public Module(ushort address, byte type)
        {
            this.address = address;
            this.type = type;
        }
        #endregion

        //TODO: test!!!!!!!!!!!!!!!!
        public bool GetRelayState(int idx)
        {
            byte res = 0;
            I2CDevice.Configuration config = new I2CDevice.Configuration(address, Program.BusClockRate);
            if (Program.Bus.TryGetRegisters2(config, Program.BusTimeout, CMD_GET_RELAY_STATE, (byte)idx, new byte[] { res }))
                return res == 1;

            return false;
        }
        public void SetRelayState(int idx, bool on)
        {
            I2CDevice.Configuration config = new I2CDevice.Configuration(address, Program.BusClockRate);
            if (Program.Bus.TrySetRegister(config, Program.BusTimeout, CMD_SET_RELAY_STATE, new byte[] { 0, (byte)(on ? 1 : 0) }))
            {
            }
        }

        public float GetTemperature(int idx)
        {
            byte[] res = new byte[2];
            I2CDevice.Configuration config = new I2CDevice.Configuration(address, Program.BusClockRate);
            if (Program.Bus.TryGetRegisters2(config, Program.BusTimeout, CMD_GET_TEMPERATURE, (byte)idx, res))
            {
                return (float)res[0] + (float)res[1] / 100;
            }

            return 0;
        }
    }
}
