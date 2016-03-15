using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System.Threading.Tasks;

namespace SmartHub.Core.IoT
{
    public class IoTDeviceManager
    {
        private static RegistryManager registryManager;

        public IoTDeviceManager(string iotHubConnectionString)
        {
            registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);
        }

        public async static Task<Device> AddDeviceAsync(string deviceId)
        {
            Device device;

            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await GetDeviceAsync(deviceId);
            }

            return device;

            //Console.WriteLine("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey);
        }
        public async static Task<Device> GetDeviceAsync(string deviceId)
        {
            return await registryManager.GetDeviceAsync(deviceId);
        }
    }
}
