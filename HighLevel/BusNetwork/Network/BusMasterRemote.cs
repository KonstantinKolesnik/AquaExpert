
namespace BusNetwork.Network
{
    public class BusMasterRemote : BusMaster
    {
        #region Constructor
        public BusMasterRemote(uint address)
            : base(address)
        {
        }
        #endregion

        #region Public methods
        public override byte GetBusModuleType(ushort busModuleAddress)
        {
            return 255;
        }
        public override void GetBusModuleControlLines(BusModule busModule)
        {
        }
        public override void GetControlLineState(ControlLine controlLine)
        {
        }
        public override void SetControlLineState(ControlLine controlLine, byte[] state)
        {
        }
        #endregion

        #region Private methods
        protected override void Scan()
        {
        }
        #endregion
    }
}
