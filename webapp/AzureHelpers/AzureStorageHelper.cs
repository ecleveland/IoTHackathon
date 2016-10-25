using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using AzureHelpers.Models;

namespace AzureHelpers
{
    public class AzureStorageHelper
    {
        private CloudStorageAccount storageAccount;
        private const string connectionString = "DefaultEndpointsProtocol=https;AccountName=dnsstorageaccount;AccountKey=aPtxv4VfnuOLHnb6Y/iR6DZJ7Y7/bPQVoEPKjuvW6+U915op7XTo9CTRS2juKiZqr6N6CxyNXGNTW4WAfObwPw==";
        public AzureStorageHelper()
        {
            storageAccount = CloudStorageAccount.Parse(connectionString);
        }

        public IList<DeviceEntity> GetAllDeviceReadings()
        {
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("DeviceRecords");

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<DeviceEntity> query = new TableQuery<DeviceEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "adafruitFeather1"));


            var results = table.ExecuteQuery(query).ToList();

            return results;
        }
    }
}
