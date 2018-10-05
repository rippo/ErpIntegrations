namespace Webcrm.ErpIntegrations.ApiClients.FortnoxApiClient.Models
{
    public class FortnoxApiKeys
    {
        public FortnoxApiKeys(
            string accessToken,
            string clientSecret)
        {
            AccessToken = accessToken;
            ClientSecret = clientSecret;
        }

        public string AccessToken { get; }
        public string ClientSecret { get; }
    }
}