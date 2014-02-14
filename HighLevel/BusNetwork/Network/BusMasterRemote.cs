
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
        public override bool BusModuleWriteRead(BusModule busModule, byte[] request, byte[] response)
        {
            return false;
        }
        #endregion

        #region Private methods
        protected override void Scan()
        {
        }
        #endregion
    }
}
