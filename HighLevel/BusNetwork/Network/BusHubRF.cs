
namespace BusNetwork.Network
{
    public class BusHubRF : BusHub
    {
        #region Constructor
        public BusHubRF(uint address)
            : base(address)
        {
        }
        #endregion

        #region Private methods
        protected override void Scan()
        {
        }
        internal override bool BusModuleWriteRead(BusModule busModule, byte[] request, byte[] response)
        {
            return false;
        }
        internal override bool BusModuleWrite(BusModule busModule, byte[] request)
        {
            return false;
        }
        #endregion
    }
}
