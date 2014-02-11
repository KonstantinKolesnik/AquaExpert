
namespace AquaExpert.Managers
{
    public class ModuleControlLine
    {
        public uint ModuleAddress { get; private set; }
        public ModulePortType ModulePortType { get; private set; }
        public int ModulePortNumber { get; private set; }

        public string UserName { get; set; }
        public string FriendlyName
        {
            get
            {
                string type;

                switch (ModulePortType)
                {
                    case Managers.ModulePortType.Relay: type = "Relay"; break;
                    case Managers.ModulePortType.WaterSensor: type = "Water sensor"; break;
                    case Managers.ModulePortType.PHSensor: type = "PH sensor"; break;
                    case Managers.ModulePortType.ORPSensor: type = "ORP sensor"; break;
                    case Managers.ModulePortType.ConductivitySensor: type = "Conductivity sensor"; break;
                    case Managers.ModulePortType.TemperatureSensor: type = "Temperature sensor"; break;
                    case Managers.ModulePortType.Dimmer: type = "Dimmer"; break;
                    default: type = "[Unknown]"; break;
                }

                return "[" + ModuleAddress + "] " + type + " # " + ModulePortNumber;
            }
        }

        public ModuleControlLine(uint moduleAddress, ModulePortType modulePortType, int modulePortNumber)
        {
            ModuleAddress = moduleAddress;
            ModulePortType = modulePortType;
            ModulePortNumber = modulePortNumber;
        }
    }
}
