// ReSharper disable InconsistentNaming
namespace Webcrm.ErpIntegrations.ApiClients.FortnoxApiClient.Models
{
    // For a full list of available fields see
    // https://github.com/FortnoxAB/csharp-api-sdk/blob/master/FortnoxAPILibrary/Entities/Invoice.cs#L1470
    // NOTE: Fortnox will only accept strings
    public class InvoiceRow
    {
        public string ArticleNumber { get; set; }
        public string DeliveredQuantity { get; set; }
        public string Description { get; set; }
        public string Discount { get; set; }
        public string Price { get; set; }

        /// <summary>The VAT needs to match allowed VAT rates in Fortnox, i.e. "0", "6", "12", "25".</summary>
        public string VAT { get; set; }
    }
}