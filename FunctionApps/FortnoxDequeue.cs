using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Synchronisation;
using Webcrm.ErpIntegrations.Synchronisation.Models;

namespace Webcrm.ErpIntegrations.FunctionApps
{
    public static class FortnoxDequeue
    {
        [FunctionName("FortnoxDequeue")]
        public static async Task Run(
            [QueueTrigger(Constants.FortnoxQueueName)] string queueItem,
            ILogger logger)
        {
            try
            {
                var webcrmClientFactory = new WebcrmClientFactory(logger, TypedEnvironment.WebcrmApiBaseUrl);
                var fortnoxMessageDispatcher = await FortnoxMessageDispatcher.Create(logger, webcrmClientFactory, TypedEnvironment.DatabaseCredentials);
                var message = JsonConvert.DeserializeObject<FortnoxQueueMessage>(queueItem);
                await fortnoxMessageDispatcher.HandleDequeuedMessage(message);
            }
            catch (SwaggerException ex)
            {
                SwaggerExceptionLogger.Log(ex);
            }
        }
    }
}