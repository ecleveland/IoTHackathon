using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace AzureHelpers.Models 
{
    public class LocationEntity : TableEntity
    {
        public LocationEntity(string deviceId, string eventTime)
        {
            this.PartitionKey = deviceId;
            this.RowKey = eventTime;
        }

        public LocationEntity() { }

        public string deviceid { get; set; }

        public DateTime eventtime { get; set; }

        public double latitude { get; set; }

        public double longitude { get; set; }
    }
}
