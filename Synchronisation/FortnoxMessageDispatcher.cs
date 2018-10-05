using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Configurations.Models;
using Webcrm.ErpIntegrations.Synchronisation.Models;

namespace Webcrm.ErpIntegrations.Synchronisation
{
    public sealed class FortnoxMessageDispatcher
    {
        private FortnoxMessageDispatcher(
            ILogger logger,
            WebcrmClientFactory webcrmClientFactory,
            FortnoxConfigService fortnoxConfigService)
        {
            Logger = logger;
            WebcrmClientFactory = webcrmClientFactory;
            FortnoxConfigService = fortnoxConfigService;
        }

        public static async Task<FortnoxMessageDispatcher> Create(
            ILogger logger,
            WebcrmClientFactory webcrmClientFactory,
            DatabaseCredentials databaseCredentials)
        {
            var fortnoxConfigService = await FortnoxConfigService.Create(databaseCredentials);
            return new FortnoxMessageDispatcher(logger, webcrmClientFactory, fortnoxConfigService);
        }

        private ILogger Logger { get; }
        private FortnoxConfigService FortnoxConfigService { get; }
        private WebcrmClientFactory WebcrmClientFactory { get; }

        public async Task HandleDequeuedMessage(FortnoxQueueMessage message)
        {
            switch (message.Action)
            {
                case FortnoxQueueAction.UpsertFortnoxDelivery:
                    {
                        var payload = JsonConvert.DeserializeObject<FortnoxUpsertWebcrmDeliveryPayload>(message.Payload);
                        var configuration = FortnoxConfigService.LoadFortnoxConfiguration(payload.WebcrmSystemId);
                        var dataCopier = await FortnoxDataCopier.Create(Logger, WebcrmClientFactory, configuration);
                        await dataCopier.CopyDeliveryToFortnox(payload.FortnoxCustomerNumber, payload.FortnoxCustomerNumber, configuration.OrganisationIdFieldName);
                    }
                    break;

                case FortnoxQueueAction.UpsertWebcrmOrganisation:
                    {
                        var payload = JsonConvert.DeserializeObject<FortnoxUpsertWebcrmOrganisationPayload>(message.Payload);

                        var configuration = FortnoxConfigService.LoadFortnoxConfiguration(payload.WebcrmSystemId);
                        var dataCopier = await FortnoxDataCopier.Create(Logger, WebcrmClientFactory, configuration);
                        await dataCopier.CopyOrganisationFromFortnox(payload.FortnoxCustomerNumber, configuration.OrganisationIdFieldName);
                    }
                    break;

                case FortnoxQueueAction.UpsertFortnoxOrganisation:
                    {
                        var payload = JsonConvert.DeserializeObject<WebcrmUpsertFortnoxOrganisationPayload>(message.Payload);

                        // TODO 1358: Only update Fortnox if a custom checkbox list contains a checked item that says something like "Fortnox Customer". Otherwise we should not sync the organisation.

                        var configuration = FortnoxConfigService.LoadFortnoxConfiguration(payload.WebcrmSystemId);
                        var dataCopier = await FortnoxDataCopier.Create(Logger, WebcrmClientFactory, configuration);
                        await dataCopier.CopyOrganisationToFortnox(payload.Organisation, configuration.OrganisationIdFieldName);
                    }
                    break;

                case FortnoxQueueAction.Unknown:
                default:
                    throw new ApplicationException($"The action '{message.Action}' is not supported.");
            }
        }
    }
}