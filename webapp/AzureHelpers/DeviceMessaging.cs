using Microsoft.Azure.Devices;
using System.Text;
using System.Threading.Tasks;
using System;

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

        public Task SendCommand(int command)
        {
            throw new NotImplementedException();
        }
    }
}
