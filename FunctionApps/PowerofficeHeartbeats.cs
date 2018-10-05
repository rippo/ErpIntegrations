using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Synchronisation;

namespace Webcrm.ErpIntegrations.FunctionApps
{
    /// <summary>
    /// Using separate heartbeats for each synchronization direction to avoid the situation where an item has been edited at the same time in both systems, and is then continually synced back and forth between the two systems. With separate heartbeats one of the edits will be lost, but the sync will happen only once.
    ///
    /// Tracking the changes can take several minutes, so the timer has been set to trigger every six minutes since a function app runs for up to five minutes by default.
    ///
    /// The PowerOffice heartbeats are offset by 30 seconds to avoid running at the exact same time as the Fortnox heartbeat.
    /// </summary>
    public static class PowerofficeHeartbeats
    {
        [FunctionName("CopyFromPowerofficeHeartbeat")]
        public static async Task CopyFromPoweroffice(
            // Trigger every six minutes.
            [TimerTrigger("30 */6 * * * *")] TimerInfo timer,
            ILogger logger)
        {
            try
            {
                var configService = await PowerofficeConfigService.Create(TypedEnvironment.DatabaseCredentials);
                var powerofficeClientFactory = new PowerofficeClientFactory(TypedEnvironment.PowerofficeApiSettings);
                var powerofficeChangeTracker = await PowerofficeChangeTracker.Create(logger, TypedEnvironment.AzureWebJobsStorage, powerofficeClientFactory);

                var configurations = configService.LoadEnabledPowerofficeConfigurations();
                foreach (var configuration in configurations)
                {
                    var dateTimeBeforeSync = DateTime.UtcNow;

                    await powerofficeChangeTracker.EnqueueUpsertedItemsForOneSystem(configuration.LastSuccessfulCopyFromErpHeartbeat, configuration);

                    await configService.UpdateLastSuccessfulCopyFromErpHeartbeat(configuration.WebcrmSystemId, dateTimeBeforeSync);
                }
            }
            catch (SwaggerException ex)
            {
                SwaggerExceptionLogger.Log(ex);
            }
        }

        [FunctionName("CopyToPowerofficeHeartbeat")]
        public static async Task CopyToPoweroffice(
            // Trigger every six minutes, offset by three.
            [TimerTrigger("30 3-59/6 * * * *")] TimerInfo timer,
            ILogger logger)
        {
            try
            {
                var configService = await PowerofficeConfigService.Create(TypedEnvironment.DatabaseCredentials);
                var webcrmClientFactory = new WebcrmClientFactory(logger, TypedEnvironment.WebcrmApiBaseUrl);
                var webcrmChangeTracker = await WebcrmChangeTracker.Create(logger, webcrmClientFactory, TypedEnvironment.AzureWebJobsStorage);

                var configurations = configService.LoadEnabledPowerofficeConfigurations();
                foreach (var configuration in configurations)
                {
                    var dateTimeBeforeSync = DateTime.UtcNow;

                    await webcrmChangeTracker.EnqueueUpsertedItemsForOneSystem(configuration.LastSuccessfulCopyToErpHeartbeat, configuration);

                    await configService.UpdateLastSuccessfulCopyToErpHeartbeat(configuration.WebcrmSystemId, dateTimeBeforeSync);
                }
            }
            catch (SwaggerException ex)
            {
                SwaggerExceptionLogger.Log(ex);
            }
        }
    }
}