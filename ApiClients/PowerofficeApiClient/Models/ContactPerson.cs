using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ContactPerson : NewContactPerson
    {
        [Key]
        public long Id { get; set; }

        public DateTimeOffset LastChanged { get; set; }
    }
}