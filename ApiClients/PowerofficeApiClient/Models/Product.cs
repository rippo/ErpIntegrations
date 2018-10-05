using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Product
    {
        [Key]
        public long Id { get; set; }

        public Guid? CreatedFromImportJournalId { get; set; }
        public DateTimeOffset LastChanged { get; set; }

        public string Code { get; set; }
        public decimal? CostPrice { get; set; }
        public string Description { get; set; }
        public string Gtin { get; set; }
        public bool? IsActive { get; set; }
        public string Name { get; set; }
        public long? ProductGroupId { get; set; }
        public decimal? ProductsOnHand { get; set; }
        public long? SalesAccount { get; set; }
        public decimal? SalesPrice { get; set; }
        public ProductType Type { get; set; }
        public string Unit { get; set; }
        public long? VatExemptSalesAccount { get; set; }
    }
}