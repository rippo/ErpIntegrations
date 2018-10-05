using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.Configurations.Models;

namespace Webcrm.ErpIntegrations.Configurations
{
    public sealed class PowerofficeConfigService
    {
        private PowerofficeConfigService(
            ConfigurationsCollection<PowerofficeConfiguration> powerofficeCollection)
        {
            PowerofficeCollection = powerofficeCollection;
        }

        public static async Task<PowerofficeConfigService> Create(DatabaseCredentials databaseCredentials)
        {
            var powerofficeCollection = await ConfigurationsCollection<PowerofficeConfiguration>.Create(databaseCredentials, CollectionId);
            return new PowerofficeConfigService(powerofficeCollection);
        }

        private const string CollectionId = "PowerofficeConfigurations";
        private ConfigurationsCollection<PowerofficeConfiguration> PowerofficeCollection { get; }

        public async Task DeletePowerofficeConfiguration(PowerofficeConfiguration configuration)
        {
            await PowerofficeCollection.DeleteDocumentAsync(configuration.SelfLink);
        }

        /// <summary>Returns `null` if the configuration was not found.</summary>
        public PowerofficeConfiguration LoadPowerofficeConfiguration(string webcrmSystemId)
        {
            var configuration = PowerofficeCollection.CreateDocumentQuery()
                // Enumerating to avoid an 'Interop not found' error when hosted on Azure.
                .AsEnumerable()
                .FirstOrDefault(config => config.WebcrmSystemId == webcrmSystemId);

            if (configuration == null)
            {
                return null;
            }

            configuration.Validate();
            return configuration;
        }

        public IEnumerable<PowerofficeConfiguration> LoadEnabledPowerofficeConfigurations()
        {
            var configurations = PowerofficeCollection.CreateDocumentQuery()
                .Where(config => !config.Disabled);

            return configurations;
        }

        public async Task SavePowerofficeConfiguration(PowerofficeConfiguration configuration)
        {
            await PowerofficeCollection.UpsertDocumentAsync(configuration);
        }

        public async Task UpdateLastSuccessfulCopyFromErpHeartbeat(string webcrmSystemId, DateTime newValue)
        {
            await UpdatePowerofficeConfiguration(webcrmSystemId, configuration => configuration.LastSuccessfulCopyFromErpHeartbeat = newValue);
        }

        public async Task UpdateLastSuccessfulCopyToErpHeartbeat(string webcrmSystemId, DateTime newValue)
        {
            await UpdatePowerofficeConfiguration(webcrmSystemId, configuration => configuration.LastSuccessfulCopyToErpHeartbeat = newValue);
        }

        private async Task UpdatePowerofficeConfiguration(string webcrmSystemId, Action<PowerofficeConfiguration> configurationUpdate)
        {
            var configuration = LoadPowerofficeConfiguration(webcrmSystemId);
            configurationUpdate(configuration);
            await SavePowerofficeConfiguration(configuration);
        }
    }
}