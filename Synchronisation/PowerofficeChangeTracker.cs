using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient;
using Webcrm.ErpIntegrations.Configurations.Models;
using Webcrm.ErpIntegrations.Synchronisation.Models;

namespace Webcrm.ErpIntegrations.Synchronisation
{
    public sealed class PowerofficeChangeTracker
    {
        private PowerofficeChangeTracker(
            ILogger logger,
            PowerofficeClientFactory powerofficeClientFactory,
            PowerofficeQueue powerofficeQueue)
        {
            Logger = logger;
            PowerofficeClientFactory = powerofficeClientFactory;
            PowerofficeQueue = powerofficeQueue;
        }

        public static async Task<PowerofficeChangeTracker> Create(
            ILogger logger,
            string storageAccountConnectionString,
            PowerofficeClientFactory powerofficeClientFactory)
        {
            var powerofficeQueue = await PowerofficeQueue.Create(logger, storageAccountConnectionString);
            return new PowerofficeChangeTracker(logger, powerofficeClientFactory, powerofficeQueue);
        }

        private ILogger Logger { get; }
        private PowerofficeClientFactory PowerofficeClientFactory { get; }
        private PowerofficeQueue PowerofficeQueue { get; }

        public async Task EnqueueUpsertedItemsForOneSystem(
            DateTime upsertedAfterUtc,
            PowerofficeConfiguration configuration)
        {
            Logger.LogTrace($"Finding items in PowerOffice upserted after {upsertedAfterUtc:O}.");

            var powerofficeClient = await PowerofficeClientFactory.Create(configuration.PowerofficeClientKey);
            await EnqueueUpsertedOrganisations(upsertedAfterUtc, powerofficeClient, configuration.WebcrmSystemId);
            await EnqueueUpsertedPersons(upsertedAfterUtc, powerofficeClient, configuration.WebcrmSystemId);

            if (configuration.SynchroniseProducts)
                await EnqueueUpsertedProducts(upsertedAfterUtc, powerofficeClient, configuration.WebcrmSystemId);

            if (configuration.SynchroniseDeliveries == SynchroniseDeliveries.FromErp)
                await EnqueueUpsertedDeliveries(upsertedAfterUtc, powerofficeClient, configuration.WebcrmSystemId);
        }

        private async Task EnqueueUpsertedOrganisations(
            DateTime upsertedAfterUtc,
            PowerofficeClient powerofficeClient,
            string webcrmSystemId)
        {
            var upsertedOrganisations = await powerofficeClient.GetUpsertedOrganisations(upsertedAfterUtc);

            Logger.LogInformation($"Found {upsertedOrganisations.Count} upserted organisations in PowerOffice.");

            var organisationPayloads = upsertedOrganisations
                .Select(upsertedOrganisation => new UpsertOrganisationFromPowerofficePayload(upsertedOrganisation, webcrmSystemId));

            await EnqueueActions(PowerofficeQueueAction.UpsertWebcrmOrganisation, organisationPayloads);
        }

        private async Task EnqueueUpsertedPersons(
            DateTime upsertedAfterUtc,
            PowerofficeClient powerofficeClient,
            string webcrmSystemId)
        {
            var payloads = new List<UpsertPersonFromPowerofficePayload>();

            var activeOrganisations = await powerofficeClient.GetActiveOrganisations();
            foreach (var organisation in activeOrganisations)
            {
                if (organisation.ContactPersonId == null)
                    continue;

                var upsertedPerson = await powerofficeClient.GetMainContactPersonIfUpserted(organisation.Id, organisation.ContactPersonId.Value, upsertedAfterUtc);

                if (upsertedPerson == null)
                    continue;

                payloads.Add(new UpsertPersonFromPowerofficePayload(organisation.Id, upsertedPerson, webcrmSystemId));
            }

            Logger.LogInformation($"Found {payloads.Count} upserted persons in PowerOffice.");
            await EnqueueActions(PowerofficeQueueAction.UpsertWebcrmPerson, payloads);
        }

        private async Task EnqueueUpsertedProducts(
            DateTime upsertedAfterUtc,
            PowerofficeClient powerofficeClient,
            string webcrmSystemId)
        {
            var upsertedProducts = await powerofficeClient.GetUpsertedProducts(upsertedAfterUtc);

            Logger.LogInformation($"Found {upsertedProducts.Count} upserted products in PowerOffice.");

            var payloads = upsertedProducts
                .Select(upsertedProduct => new UpsertProductFromPowerofficePayload(upsertedProduct, webcrmSystemId));

            await EnqueueActions(PowerofficeQueueAction.UpsertWebcrmProduct, payloads);
        }

        private async Task EnqueueUpsertedDeliveries(
            DateTime upsertedAfterUtc,
            PowerofficeClient powerofficeClient,
            string webcrmSystemId)
        {
            var upsertedDeliveries = await powerofficeClient.GetUpsertedInvoices(upsertedAfterUtc);

            Logger.LogInformation($"Found {upsertedDeliveries.Count} upserted deliveries in PowerOffice.");

            var payloads = upsertedDeliveries
                .Select(upsertedDelivery => new UpsertDeliveryFromPowerofficePayload(upsertedDelivery, webcrmSystemId));

            await EnqueueActions(PowerofficeQueueAction.UpsertWebcrmDelivery, payloads);
        }

        private async Task EnqueueActions(
            PowerofficeQueueAction action,
            IEnumerable<BasePowerofficePayload> payloads)
        {
            foreach (var payload in payloads)
            {
                var queueMessage = new PowerofficeQueueMessage(action, payload);
                await PowerofficeQueue.Enqueue(queueMessage);
            }
        }
    }
}