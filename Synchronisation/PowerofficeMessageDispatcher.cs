using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Configurations.Models;
using Webcrm.ErpIntegrations.Synchronisation.Models;

namespace Webcrm.ErpIntegrations.Synchronisation
{
    /// <summary>Can handle messages that are dequeue from the PowerOffice queue.</summary>
    public sealed class PowerofficeMessageDispatcher
    {
        private PowerofficeMessageDispatcher(
            ILogger logger,
            WebcrmClientFactory webcrmClientFactory,
            PowerofficeClientFactory powerofficeClientFactory,
            PowerofficeConfigService powerofficeConfigService)
        {
            Logger = logger;
            WebcrmClientFactory = webcrmClientFactory;
            PowerofficeClientFactory = powerofficeClientFactory;
            PowerofficeConfigService = powerofficeConfigService;
        }

        public static async Task<PowerofficeMessageDispatcher> Create(
            ILogger logger,
            WebcrmClientFactory webcrmClientFactory,
            DatabaseCredentials databaseCredentials,
            PowerofficeClientFactory powerofficeClientFactory)
        {
            var powerofficeConfigService = await PowerofficeConfigService.Create(databaseCredentials);
            return new PowerofficeMessageDispatcher(logger, webcrmClientFactory, powerofficeClientFactory, powerofficeConfigService);
        }

        private ILogger Logger { get; }
        private PowerofficeClientFactory PowerofficeClientFactory { get; }
        private PowerofficeConfigService PowerofficeConfigService { get; }
        private WebcrmClientFactory WebcrmClientFactory { get; }

        public async Task HandleDequeuedMessage(PowerofficeQueueMessage message)
        {
            switch (message.Action)
            {
                case PowerofficeQueueAction.UpsertPowerofficeDelivery:
                    {
                        var (payload, dataCopier) = await GetPayloadAndDataCopier<UpsertDeliveryToPowerofficePayload>(message);
                        await dataCopier.CopyDeliveryToPoweroffice(payload.WebcrmDelivery, payload.WebcrmDeliveryLines);
                    }
                    break;

                case PowerofficeQueueAction.UpsertPowerofficeOrganisation:
                    {
                        var (payload, dataCopier) = await GetPayloadAndDataCopier<UpsertOrganisationToPowerofficePayload>(message);
                        await dataCopier.CopyOrganisationToPoweroffice(payload.WebcrmOrganisation);
                    }
                    break;

                case PowerofficeQueueAction.UpsertPowerofficePerson:
                    {
                        var (payload, dataCopier) = await GetPayloadAndDataCopier<UpsertPersonToPowerofficePayload>(message);
                        await dataCopier.CopyPersonToPoweroffice(payload.WebcrmPerson);
                    }
                    break;

                case PowerofficeQueueAction.UpsertWebcrmDelivery:
                    {
                        var (payload, dataCopier) = await GetPayloadAndDataCopier<UpsertDeliveryFromPowerofficePayload>(message);
                        var configuration = PowerofficeConfigService.LoadPowerofficeConfiguration(payload.WebcrmSystemId);
                        var powerofficeClient = await PowerofficeClientFactory.Create(configuration.PowerofficeClientKey);
                        var powerofficeDeliveryWithDeliveryLines = await powerofficeClient.GetInvoice(payload.PowerofficeDelivery.Id);
                        await dataCopier.CopyDeliveryFromPoweroffice(powerofficeDeliveryWithDeliveryLines);
                    }
                    break;

                case PowerofficeQueueAction.UpsertWebcrmOrganisation:
                    {
                        var (payload, dataCopier) = await GetPayloadAndDataCopier<UpsertOrganisationFromPowerofficePayload>(message);
                        await dataCopier.CopyOrganisationFromPoweroffice(payload.PowerofficeOrganisation);
                    }
                    break;

                case PowerofficeQueueAction.UpsertWebcrmPerson:
                    {
                        var (payload, dataCopier) = await GetPayloadAndDataCopier<UpsertPersonFromPowerofficePayload>(message);
                        await dataCopier.CopyPersonFromPoweroffice(payload.PowerofficePerson, payload.PowerofficeOrganisationId);
                    }
                    break;

                case PowerofficeQueueAction.UpsertWebcrmProduct:
                    {
                        var (payload, dataCopier) = await GetPayloadAndDataCopier<UpsertProductFromPowerofficePayload>(message);
                        await dataCopier.CopyProductFromPoweroffice(payload.PowerofficeProduct);
                    }
                    break;

                case PowerofficeQueueAction.Unknown:
                default:
                    throw new ApplicationException($"The action '{message.Action}' is not supported.");
            }
        }

        private async Task<(TPayload, PowerofficeDataCopier)> GetPayloadAndDataCopier<TPayload>(PowerofficeQueueMessage message) where TPayload : BasePowerofficePayload
        {
            var payload = JsonConvert.DeserializeObject<TPayload>(message.SerialisedPayload);
            var configuration = PowerofficeConfigService.LoadPowerofficeConfiguration(payload.WebcrmSystemId);
            var dataCopier = await PowerofficeDataCopier.Create(Logger, WebcrmClientFactory, PowerofficeClientFactory, configuration);

            return (payload, dataCopier);
        }
    }
}