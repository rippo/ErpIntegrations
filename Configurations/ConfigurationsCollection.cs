using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.Configurations.Models;

namespace Webcrm.ErpIntegrations.Configurations
{
    internal sealed class ConfigurationsCollection<TConfiguration>
    {
        private ConfigurationsCollection(
            string collectionId)
        {
            CollectionUri = UriFactory.CreateDocumentCollectionUri(IntegrationsDatabase.DatabaseId, collectionId);
        }

        public static async Task<ConfigurationsCollection<TConfiguration>> Create(
            DatabaseCredentials databaseCredentials,
            string collectionId)
        {
            await IntegrationsDatabase.Initialize(databaseCredentials);

            var integrationsCollection = new ConfigurationsCollection<TConfiguration>(collectionId);
            await IntegrationsDatabase.Client.CreateDocumentCollectionIfNotExistsAsync(
                IntegrationsDatabase.DatabaseUri,
                new DocumentCollection { Id = collectionId },
                new RequestOptions { OfferThroughput = 400 }
            );

            return integrationsCollection;
        }

        private Uri CollectionUri { get; }

        public IOrderedQueryable<TConfiguration> CreateDocumentQuery()
        {
            return IntegrationsDatabase.Client.CreateDocumentQuery<TConfiguration>(CollectionUri);
        }

        public async Task DeleteDocumentAsync(string documentUri)
        {
            await IntegrationsDatabase.Client.DeleteDocumentAsync(documentUri);
        }

        public async Task UpsertDocumentAsync(TConfiguration configuration)
        {
            await IntegrationsDatabase.Client.UpsertDocumentAsync(CollectionUri, configuration);
        }
    }
}