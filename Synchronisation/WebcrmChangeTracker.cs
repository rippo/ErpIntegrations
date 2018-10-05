using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;
using Webcrm.ErpIntegrations.Configurations.Models;
using Webcrm.ErpIntegrations.Synchronisation.Models;

namespace Webcrm.ErpIntegrations.Synchronisation
{
    public sealed class WebcrmChangeTracker
    {
        private WebcrmChangeTracker(
            ILogger logger,
            WebcrmClientFactory webcrmClientFactory,
            PowerofficeQueue powerofficeQueue)
        {
            Logger = logger;
            WebcrmClientFactory = webcrmClientFactory;
            PowerofficeQueue = powerofficeQueue;
        }

        public static async Task<WebcrmChangeTracker> Create(
            ILogger logger,
            WebcrmClientFactory webcrmClientFactory,
            string storageAccountConnectionString)
        {
            var powerofficeQueue = await PowerofficeQueue.Create(logger, storageAccountConnectionString);
            var webcrmChangeTracker = new WebcrmChangeTracker(logger, webcrmClientFactory, powerofficeQueue);
            return webcrmChangeTracker;
        }

        private ILogger Logger { get; }
        private PowerofficeQueue PowerofficeQueue { get; }
        private WebcrmClientFactory WebcrmClientFactory { get; }

        public async Task EnqueueUpsertedItemsForOneSystem(
            DateTime upsertedAfterUtc,
            BaseConfiguration configuration)
        {
            Logger.LogTrace($"Finding items in webCRM upserted after {upsertedAfterUtc:O}.");

            var webcrmClient = await WebcrmClientFactory.Create(configuration.WebcrmApiKey);
            // Synchronising organisations first, since the persons and deliveries might depend on them.
            await EnqueueUpsertedOrganisations(upsertedAfterUtc, webcrmClient, configuration);
            await EnqueueUpsertedPersons(upsertedAfterUtc, webcrmClient, configuration);

            if (configuration.SynchroniseDeliveries == SynchroniseDeliveries.ToErp)
                await EnqueueUpsertedDeliveries(upsertedAfterUtc, webcrmClient, configuration.WebcrmSystemId);
        }

        private async Task EnqueueUpsertedDeliveries(
            DateTime upsertedAfterUtc,
            WebcrmClient webcrmClient,
            string webcrmSystemId)
        {
            var upsertedDeliveries = await webcrmClient.GetUpsertedDeliveries(upsertedAfterUtc);
            Logger.LogInformation($"Found {upsertedDeliveries.Count} upsertedDeliveries deliveries in webCRM.");

            const int millisecondsDelayBetweenCalls = 20;
            var createDeliveryPayloadTasks = upsertedDeliveries
                .Select(async (delivery, index) =>
                {
                    // Adding an indexed delay before fetching the delivery lines to avoid that all the calls are made simultaneously in the WhenAll line below. Without this delay we get sporadic errors about 'connection was forcibly closed', 'unauthorized' or 'https connection was dropped dropped'. This may be due to an error in our REST API not handling a lot of simultaneous requests correctly.
                    await Task.Delay(index * millisecondsDelayBetweenCalls);
                    var deliveryLines = await webcrmClient.GetDeliveryLines(delivery.DeliveryId);
                    return new UpsertDeliveryToPowerofficePayload(delivery, deliveryLines, webcrmSystemId);
                });

            var deliveryPayloads = await Task.WhenAll(createDeliveryPayloadTasks);

            await EnqueueActions(PowerofficeQueueAction.UpsertPowerofficeDelivery, deliveryPayloads);
        }

        private async Task EnqueueUpsertedOrganisations(
            DateTime upsertedAfterUtc,
            WebcrmClient webcrmClient,
            BaseConfiguration configuration)
        {
            var upsertedOrganisations = await webcrmClient.GetUpsertedOrganisations(upsertedAfterUtc, configuration.AcceptedOrganisationStatuses, configuration.AcceptedOrganisationTypes);
            Logger.LogInformation($"Found {upsertedOrganisations.Count} upserted organisations in webCRM.");

            var organisationPayloads = upsertedOrganisations
                .Select(organisation => new UpsertOrganisationToPowerofficePayload(organisation, configuration.WebcrmSystemId));

            await EnqueueActions(PowerofficeQueueAction.UpsertPowerofficeOrganisation, organisationPayloads);
        }

        private async Task EnqueueUpsertedPersons(
            DateTime upsertedAfterUtc,
            WebcrmClient webcrmClient,
            BaseConfiguration configuration)
        {
            var upsertedPersons = await webcrmClient.GetUpsertedPersons(upsertedAfterUtc, configuration);
            Logger.LogInformation($"Found {upsertedPersons.Count} upserted persons in webCRM.");

            var personPayloads = upsertedPersons
                .Select(person => new UpsertPersonToPowerofficePayload(person, configuration.WebcrmSystemId));

            await EnqueueActions(PowerofficeQueueAction.UpsertPowerofficePerson, personPayloads);
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