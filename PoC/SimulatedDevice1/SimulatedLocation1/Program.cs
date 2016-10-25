using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedLocation1
{
    class Program
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "dns.azure-devices.net";
        static string deviceKey = "d+mYzCKG+VxmfWLjW/9K5EIZiKb5J4McRdNSrslDVcY=";

        static void Main(string[] args)
        {
            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("simulatedLocation1", deviceKey));

            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
        }

        private static async void SendDeviceToCloudMessagesAsync()
        {
            // Sogeti Dallas : 32.8712126,-96.9457513
            double lat = 32.8712126;
            double lon = -96.9457513;
            Random rand = new Random();
            Console.WriteLine("\nDelivering telemetry.");
            while (true)
            {
                double currentLat = lat + rand.NextDouble() * .001;
                double currentLong = lon + rand.NextDouble() * .001;

                var telemetryDataPoint = new
                {
                    DeviceId = "simulatedLocation1",
                    Latitude = currentLat,
                    Longitude = currentLong,
                    EventTime = DateTime.Now
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                Task.Delay(5000).Wait();
            }
        }
    }
}
