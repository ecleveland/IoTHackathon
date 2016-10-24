using System.Threading.Tasks;

namespace AzureHelpers
{
    public interface IDeviceMessaging
    {
        Task SendMessageAsync();
        Task SendCommand(int command);
    }
}
