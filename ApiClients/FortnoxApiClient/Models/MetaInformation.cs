using Newtonsoft.Json;

namespace Webcrm.ErpIntegrations.ApiClients.FortnoxApiClient.Models
{
    public class MetaInformation
    {
        [JsonProperty("@CurrentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty("@TotalPages")]
        public int TotalPages { get; set; }

        [JsonProperty("@TotalResources")]
        public int TotalResources { get; set; }
    }
}