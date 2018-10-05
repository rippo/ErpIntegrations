using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Configurations.Models;

namespace Webcrm.ErpIntegrations.Synchronisation
{
    public sealed class FortnoxChangeTracker
    {
        private FortnoxChangeTracker(
            ILogger logger,
            WebcrmClientFactory webcrmClientFactory,
            string storageAccountConnectionString,
            FortnoxConfigService fortnoxConfigService)
        {
            Logger = logger;
            WebcrmClientFactory = webcrmClientFactory;
            StorageAccountConnectionString = storageAccountConnectionString;
            FortnoxConfigService = fortnoxConfigService;
        }

        public static async Task<FortnoxChangeTracker> Create(
            ILogger logger,
            WebcrmClientFactory webcrmClientFactory,
            string storageAccountConnectionString,
            DatabaseCredentials databaseCredentials)
        {
            var fortnoxConfigService = await FortnoxConfigService.Create(databaseCredentials);
            return new FortnoxChangeTracker(logger, webcrmClientFactory, storageAccountConnectionString, fortnoxConfigService);
        }

        private string StorageAccountConnectionString { get; }
        private ILogger Logger { get; }
        private FortnoxConfigService FortnoxConfigService { get; }
        private WebcrmClientFactory WebcrmClientFactory { get; }

        public async Task EnqueueUpsertedCustomers()
        {
            var configurations = FortnoxConfigService.LoadEnabledFortnoxConfigurations();
            foreach (var configuration in configurations)
            {
                // Store time we are using to detect changes
                var dateTimeBeforeSync = DateTime.UtcNow;

                var dataCopier = await FortnoxDataCopier.Create(
                    Logger,
                    WebcrmClientFactory,
                    configuration);

                await dataCopier.EnqueueRecentlyUpsertedCustomers(
                    StorageAccountConnectionString,
                    configuration.WebcrmSystemId,
                    configuration.LastSuccessfulCopyFromErpHeartbeat);

                configuration.LastSuccessfulCopyFromErpHeartbeat = dateTimeBeforeSync;
                await FortnoxConfigService.SaveFortnoxConfiguration(configuration);
            }
        }

        public async Task EnqueueUpsertedInvoices()
        {
            var configurations = FortnoxConfigService.LoadEnabledFortnoxConfigurations();
            foreach (var configuration in configurations)
            {
                // Store time we are using to detect changes
                var dateTimeBeforeSync = DateTime.UtcNow;

                var dataCopier = await FortnoxDataCopier.Create(
                    Logger,
                    WebcrmClientFactory,
                    configuration);

                await dataCopier.EnqueueRecentlyUpsertedInvoices(
                    StorageAccountConnectionString,
                    configuration.WebcrmSystemId,
                    configuration.LastSuccessfulCopyFromErpHeartbeat);

                configuration.LastSuccessfulCopyFromErpHeartbeat = dateTimeBeforeSync;
                await FortnoxConfigService.SaveFortnoxConfiguration(configuration);
            }
        }
    }
}