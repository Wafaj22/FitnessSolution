using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace FitnessSolution.Helpers
{
    public class TableManager
    {
        public TableManager()
        {
        }

        public const string DIETS_TABLE = "diets";
        public const string RECIPES_TABLE = "recipes";

        public static async Task<CloudTable> GetStorageTableAsync(string tableName)
        {
            // Step 1 Read Json
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            IConfiguration configuration = builder.Build();

            // Step 2 Add access key to account object by referring the account appsettings.json
            CloudStorageAccount objectAccount =
                CloudStorageAccount.Parse(configuration["connectionStrings:BlobStorageConnection"]);

            CloudTableClient tableClient = objectAccount.CreateCloudTableClient();
            CloudTable cloudTable = tableClient.GetTableReference(tableName);
            await cloudTable.CreateIfNotExistsAsync();

            return cloudTable;
        }
    }
}
