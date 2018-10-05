using System;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.Configurations.Models;

namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient
{
    public class PowerofficeClientFactory
    {
        public PowerofficeClientFactory(PowerofficeApiSettings apiSettings)
        {
            ApiSettings = apiSettings;
        }

        private PowerofficeApiSettings ApiSettings { get; }

        public async Task<PowerofficeClient> Create(Guid clientKey)
        {
            return await PowerofficeClient.Create(ApiSettings, clientKey);
        }
    }
}