using System.Net.Http;
using System.Net.Http.Headers;

namespace Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient
{
    public partial class WebcrmSdk
    {
        internal string AccessToken { get; set; }

        partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
        {
            if (AccessToken != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            }
        }
    }
}