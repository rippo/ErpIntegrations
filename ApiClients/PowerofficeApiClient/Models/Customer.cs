using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models
{
    /// <remarks>https://api.poweroffice.net/Web/docs/index.html#Reference/Rest/Type_Customer.md</remarks>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Customer : NewCustomer
    {
        [Key]
        public long Id { get; set; }

        public Guid? CreatedFromImportJournalId { get; set; }
        public DateTimeOffset LastChanged { get; set; }
    }
}