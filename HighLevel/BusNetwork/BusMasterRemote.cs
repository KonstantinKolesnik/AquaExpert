
namespace BusNetwork
{
    public class BusMasterRemote : BusMaster
    {

        protected override void ScanBusModules()
        {
        }
        protected override byte GetBusModuleType(ushort busModuleAddress)
        {
            return 255;
        }
        protected override void GetBusModuleControlLines(BusModule busModule)
        {
        }

        public override byte[] GetControlLineState(ControlLine controlLine)
        {
            return new byte[0];
        }
    }
}
