using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.Core.IoT
{
    public class IoTMessageManager
    {
        private static string iotHubD2cEndpoint = "messages/events";
        private static EventHubClient eventHubClient;

        static DeviceClient deviceClient;
        static string iotHubUri = "{iot hub hostname}";
        static string deviceKey = "{device key}";


        //public event MessageReceived


        public IoTMessageManager(string iotHubConnectionString)
        {
            eventHubClient = EventHubClient.CreateFromConnectionString(iotHubConnectionString, iotHubD2cEndpoint);

            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            foreach (string partition in d2cPartitions)
                ReceiveMessagesFromDeviceAsync(partition);

            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("myFirstDevice"/*deviceId */, deviceKey));

            //SendDeviceToCloudMessagesAsync();
        }

        private async static Task ReceiveMessagesFromDeviceAsync(string partition)
        {
            var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);

            while (true)
            {
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null)
                    continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                //Console.WriteLine(string.Format("Message received. Partition: {0} Data: '{1}'", partition, data));

                //MessageReceived(partition, data);
            }
        }

        public static async void SendDeviceToCloudMessagesAsync(string deviceId, object data)
        {
            //double avgWindSpeed = 10; // m/s
            //Random rand = new Random();

            while (true)
            {
                //double currentWindSpeed = avgWindSpeed + rand.NextDouble() * 4 - 2;

                var telemetryDataPoint = new
                {
                    deviceId = deviceId,
                    //windSpeed = currentWindSpeed
                    value = data
                };

                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(messageString));

                await deviceClient.SendEventAsync(message);

                //Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                //Thread.Sleep(1000);
            }
        }
    }
}
