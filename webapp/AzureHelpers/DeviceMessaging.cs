using Microsoft.Azure.Devices;
using System.Text;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;

namespace AzureHelpers
{
    public class DeviceMessaging
    {
        static ServiceClient serviceClient;
        static string connectionString = "HostName=dns.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=Yz7BDPAyRCzHF63YWU6E2di30j8YPlepKkd5utT2Op0=";

        public async Task SendMessageAsync()
        {
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            var commandMessage = new Message(Encoding.ASCII.GetBytes("Cloud to device message."));
            await serviceClient.SendAsync("simulatedDevice1", commandMessage);
        }

        public async Task SendCommand(string deviceId, string command)
        {
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            var messageObj = new { Name = command, Parameters = "" };
            var message = JsonConvert.SerializeObject(messageObj);
            var commandMessage = new Message(Encoding.ASCII.GetBytes(message));
            await serviceClient.SendAsync(deviceId, commandMessage);
        }
    }
}
