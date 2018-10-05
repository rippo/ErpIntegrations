using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient
{
    public class WebcrmClientFactory
    {
        public WebcrmClientFactory(
            ILogger logger,
            Uri baseApiUrl)
        {
            Logger = logger;
            BaseApiUrl = baseApiUrl;
        }

        private ILogger Logger { get; }
        private Uri BaseApiUrl { get; }

        public async Task<WebcrmClient> Create(string apiKey)
        {
            var webcrmClient = await WebcrmClient.Create(Logger, BaseApiUrl, apiKey);
            return webcrmClient;
        }
    }
}