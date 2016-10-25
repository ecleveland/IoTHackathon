using System.Web.Http;
using AzureHelpers;
using System.Threading.Tasks;

namespace dnsweb.Controllers
{
    [RoutePrefix("api/Device")]
    public class DeviceController : ApiController
    {
        private DeviceMessaging deviceMessaging = new DeviceMessaging();
        private AzureStorageHelper storageHelper = new AzureStorageHelper();

        [HttpGet]
        [Route("SendCommandToDevice/{deviceId}/{function}")]
        public async Task<IHttpActionResult> SendCommandToDevice(string deviceId, string function)
        {            
            await deviceMessaging.SendCommand(deviceId, function);
            return Ok(1);
        }

        [HttpGet]
        [Route("GetDeviceInformation/{DeviceId}")]
        public async Task<IHttpActionResult> GetDeviceInformation(string DeviceId)
        {
            var data = new { deviceName = "name", locked = true };
            return Ok(data);
        }
        
        [HttpGet]
        [Route("GetDeviceAccessed/{DeviceId}")]
        public async Task<IHttpActionResult> GetCaseAccessed(string DeviceId)
        {
            var data = new { name = "name", time = "time"};
            return Ok(data);
        }

        [HttpGet]
        [Route("GetIsDeviceFlagged/{DeviceId}")]
        public async Task<IHttpActionResult> GetIsDeviceFlagged(string DeviceId)
        {
            var data = true;
            return Ok(data);
        }

        [HttpGet]
        [Route("GetItemsCount/{DeviceId}")]
        public async Task<IHttpActionResult> GetItemsCount(string DeviceId)
        {
            var data = storageHelper.GetDeviceItemsCount(DeviceId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetItemsRemovedCount/{DeviceId}")]
        public async Task<IHttpActionResult> GetItemsRemovedCount(string DeviceId)
        {
            var data = "Items Removed Data";
            return Ok(data);
        }

        [HttpGet]
        [Route("GetDeviceLocation/{DeviceId}")]
        public async Task<IHttpActionResult> GetDeviceLocation(string DeviceId)
        {
            var data = storageHelper.GetLastKnownLocation(DeviceId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetDeviceReadings")]
        public async Task<IHttpActionResult> GetDeviceReadings()
        {
            var data = storageHelper.GetAllDeviceReadings("adafruitFeather1");
            return Ok(data);
        }
    }
}