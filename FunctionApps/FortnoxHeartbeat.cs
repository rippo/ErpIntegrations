using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;
using Webcrm.ErpIntegrations.Synchronisation;

namespace Webcrm.ErpIntegrations.FunctionApps
{
    public static class FortnoxHeartbeat
    {
        [FunctionName("FortnoxHeartbeat")]
        public static async Task Run(
            // Trigger every minute.
            [TimerTrigger("0 */1 * * * *")] TimerInfo timer,
            ILogger logger)
        {
            try
            {
                var webcrmClientFactory = new WebcrmClientFactory(logger, TypedEnvironment.WebcrmApiBaseUrl);
                var changeTracker = await FortnoxChangeTracker.Create(
                    logger,
                    webcrmClientFactory,
                    TypedEnvironment.AzureWebJobsStorage,
                    TypedEnvironment.DatabaseCredentials);

                await changeTracker.EnqueueUpsertedCustomers();
                await changeTracker.EnqueueUpsertedInvoices();
            }
            catch (SwaggerException ex)
            {
                SwaggerExceptionLogger.Log(ex);
            }
        }
    }
}