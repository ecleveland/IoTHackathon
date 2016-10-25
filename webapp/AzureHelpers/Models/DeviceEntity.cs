using Microsoft.WindowsAzure.Storage.Table;

namespace AzureHelpers.Models
{
    public class DeviceEntity : TableEntity
    {
        public DeviceEntity(string deviceId, string eventTime)
        {
            this.PartitionKey = deviceId;
            this.RowKey = eventTime;
        }

        public DeviceEntity() { }

        public string deviceid { get; set; }

        public long eventtime { get; set; }

        public long humidityreading { get; set; }

        public long pressure { get; set; }

        public long temperaturereading { get; set; }
    }
}
