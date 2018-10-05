using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Synchronisation;
using Webcrm.ErpIntegrations.Synchronisation.Models;

namespace Webcrm.ErpIntegrations.FunctionApps
{
    public static class PowerofficeDequeue
    {
        [FunctionName("PowerofficeDequeue")]
        public static async Task Run(
            [QueueTrigger(Constants.PowerofficeQueueName)] string queueItem,
            ILogger logger)
        {
            logger.LogTrace($"Dequeuing message from the '{Constants.PowerofficeQueueName}' queue.");

            try
            {
                var webcrmClientFactory = new WebcrmClientFactory(logger, TypedEnvironment.WebcrmApiBaseUrl);
                var powerofficeClientFactory = new PowerofficeClientFactory(TypedEnvironment.PowerofficeApiSettings);
                var dispatcher = await PowerofficeMessageDispatcher.Create(logger, webcrmClientFactory, TypedEnvironment.DatabaseCredentials, powerofficeClientFactory);
                var message = JsonConvert.DeserializeObject<PowerofficeQueueMessage>(queueItem);
                await dispatcher.HandleDequeuedMessage(message);
            }
            catch (SwaggerException ex)
            {
                SwaggerExceptionLogger.Log(ex);
            }
        }
    }
}