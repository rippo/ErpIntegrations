using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Webcrm.ErpIntegrations.Synchronisation.Models
{
    public class FortnoxQueueMessage
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public FortnoxQueueAction Action { get; set; }

        public string Payload { get; set; }
    }
}