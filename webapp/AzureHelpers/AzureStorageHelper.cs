using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using AzureHelpers.Models;
using System;

namespace AzureHelpers
{
    public class AzureStorageHelper
    {
        private CloudStorageAccount storageAccount;
        private const string connectionString = "DefaultEndpointsProtocol=https;AccountName=dnsstorageaccount;AccountKey=aPtxv4VfnuOLHnb6Y/iR6DZJ7Y7/bPQVoEPKjuvW6+U915op7XTo9CTRS2juKiZqr6N6CxyNXGNTW4WAfObwPw==";
        //private const string deviceId = "adafruitFeather1";
        public DateTimeOffset DateTimeOffsetVal { get; private set; }

        public AzureStorageHelper()
        {
            storageAccount = CloudStorageAccount.Parse(connectionString);
        }

        public int GetDeviceItemsCount(string deviceId)
        {
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("DeviceRecords");

            var filter1 = TableQuery.GenerateFilterCondition(
                   "PartitionKey", QueryComparisons.Equal,
                   deviceId);
            var filter2 = TableQuery.GenerateFilterConditionForDate(
                    "Timestamp", QueryComparisons.GreaterThanOrEqual,
                    DateTimeOffset.Now.AddDays(-1).Date);
            var combinedFilter = TableQuery.CombineFilters(
                        filter1,
                        TableOperators.And, filter2);

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<DeviceEntity> query = new TableQuery<DeviceEntity>().Where(combinedFilter);

            var results = table.ExecuteQuery(query).ToList();

            var lastKnown = results.Last();


            if (lastKnown.pressure == 0)
                return 0;
            return 1;
        }

        public IList<DeviceEntity> GetAllDeviceReadings(string deviceId)
        {
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("DeviceRecords");
            
            var filter1 = TableQuery.GenerateFilterCondition(
                   "PartitionKey", QueryComparisons.Equal,
                   deviceId);
            var filter2 = TableQuery.GenerateFilterConditionForDate(
                    "Timestamp", QueryComparisons.GreaterThanOrEqual, 
                    DateTimeOffset.Now.AddDays(-1).Date);
            var combinedFilter = TableQuery.CombineFilters(
                        filter1,
                        TableOperators.And, filter2);

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<DeviceEntity> query = new TableQuery<DeviceEntity>().Where(combinedFilter);

            var results = table.ExecuteQuery(query).ToList();

            return results;
        }

        public LocationEntity GetLastKnownLocation(string deviceId)
        {
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("LocationRecords");
            var filter1 = TableQuery.GenerateFilterCondition(
                "PartitionKey", QueryComparisons.Equal, deviceId);
            var filter2 = TableQuery.GenerateFilterConditionForDate(
                    "Timestamp", QueryComparisons.GreaterThanOrEqual,
                    DateTimeOffset.Now.AddDays(-1).Date);
            var combinedFilter = TableQuery.CombineFilters(
                        filter1,
                        TableOperators.And, filter2);

            TableQuery<LocationEntity> query = new TableQuery<LocationEntity>().Where(combinedFilter);

            var results = table.ExecuteQuery(query).ToList();

            var lastKnown = results.Last();
            return lastKnown;
        }
    }
}
