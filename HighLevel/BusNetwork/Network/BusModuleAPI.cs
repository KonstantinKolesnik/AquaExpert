
namespace BusNetwork.Network
{
    public static class BusModuleAPI
    {
        public const byte ControlLineTypesToRequest = 32;

        public const byte CmdGetType = 0;
        public const byte CmdGetControlLineCount = 1;
        public const byte CmdGetControlLineState = 2;
        public const byte CmdSetControlLineState = 3;
    }
}
