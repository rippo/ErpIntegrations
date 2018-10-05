using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.Configurations.Models;

namespace Webcrm.ErpIntegrations.Configurations
{
    public sealed class FortnoxConfigService
    {
        private FortnoxConfigService(ConfigurationsCollection<FortnoxConfiguration> fortnoxCollection)
        {
            FortnoxCollection = fortnoxCollection;
        }

        public static async Task<FortnoxConfigService> Create(DatabaseCredentials databaseCredentials)
        {
            var fortnoxCollection = await ConfigurationsCollection<FortnoxConfiguration>.Create(databaseCredentials, CollectionId);
            return new FortnoxConfigService(fortnoxCollection);
        }

        private const string CollectionId = "FortnoxConfigurations";
        private ConfigurationsCollection<FortnoxConfiguration> FortnoxCollection { get; }

        public async Task DeleteFortnoxConfiguration(FortnoxConfiguration configuration)
        {
            await FortnoxCollection.DeleteDocumentAsync(configuration.SelfLink);
        }

        /// <summary>Returns `null` if the configuration was not found.</summary>
        public FortnoxConfiguration LoadFortnoxConfiguration(string webcrmSystemId)
        {
            var configuration = FortnoxCollection.CreateDocumentQuery()
                // Enumerating to avoid an 'Interop not found' error when hosted on Azure.
                .AsEnumerable()
                .FirstOrDefault(config => config.WebcrmSystemId == webcrmSystemId);

            return configuration;
        }

        public IEnumerable<FortnoxConfiguration> LoadEnabledFortnoxConfigurations()
        {
            var configurations = FortnoxCollection.CreateDocumentQuery()
                .Where(config => !config.Disabled);

            return configurations;
        }

        public async Task SaveFortnoxConfiguration(FortnoxConfiguration configuration)
        {
            await FortnoxCollection.UpsertDocumentAsync(configuration);
        }
    }
}