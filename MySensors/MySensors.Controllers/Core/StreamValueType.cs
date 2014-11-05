
namespace MySensors.Controllers.Core
{
    public enum StreamValueType : byte
    {
        FirmwareConfigRequest = 0,
        FirmwareConfigResponse = 1,
        FirmwareRequest = 2,
        FirmwareResponse = 3,
        Sound = 4,
        Image = 5,
    }
}
