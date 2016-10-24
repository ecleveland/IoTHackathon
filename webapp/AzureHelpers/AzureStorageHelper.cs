using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

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

        public void GetAllTemperatures()
        {
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("DeviceRecords");

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            //TableQuery<CustomerEntity> query = new TableQuery<CustomerEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Smith"));

            // Print the fields for each customer.
            //foreach (CustomerEntity entity in table.ExecuteQuery(query))
            //{
            //    Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
            //        entity.Email, entity.PhoneNumber);
            //}
        }
    }
}
