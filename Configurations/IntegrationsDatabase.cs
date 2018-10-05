using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.Configurations.Models;

namespace Webcrm.ErpIntegrations.Configurations
{
    public static class IntegrationsDatabase
    {
        static IntegrationsDatabase()
        {
            DatabaseUri = UriFactory.CreateDatabaseUri(DatabaseId);
        }

        public static async Task Initialize(
            DatabaseCredentials databaseCredentials)
        {
            lock (CreateDatabaseLock)
            {
                if (Client == null)
                {
                    Client = new DocumentClient(databaseCredentials.Endpoint, databaseCredentials.AccountKey);
                }
            }

            await Client.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseId });
        }

        private static readonly object CreateDatabaseLock = new object();

        public const string DatabaseId = "Integrations";
        public static DocumentClient Client { get; private set; }
        public static Uri DatabaseUri { get; }
    }
}