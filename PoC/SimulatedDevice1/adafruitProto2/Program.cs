using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace adafruitProto2
{
    class Program
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "dns.azure-devices.net";
        static string deviceKey = "0R/+8OBQb6MU/b565Bau6gjQlnyfZl7QWW475uiX8t8=";
        static string deviceId = "adafruitProto2";

        static void Main(string[] args)
        {
            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey));

            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
        }

        private static async void SendDeviceToCloudMessagesAsync()
        {
            // Sogeti Dallas : 32.8712126,-96.9457513
            // Bellevue Park : 47.6124876,-122.2048304
            // Globe Life Park : 32.7512847,-97.0846984
            double lat = 32.7512847;
            double lon = -97.0846984;
            Random rand = new Random();
            Console.WriteLine("\nDelivering telemetry.");
            while (true)
            {
                double currentLat = lat + rand.NextDouble() * .001;
                double currentLong = lon + rand.NextDouble() * .001;

                var telemetryDataPoint = new
                {
                    DeviceId = deviceId,
                    Latitude = currentLat,
                    Longitude = currentLong,
                    EventTime = DateTime.Now,
                    Pressure = 1023,
                    MTemperature = 24,
                    Humidity = 45
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
