using System.Collections.Generic;

namespace Webcrm.ErpIntegrations.ApiClients.FortnoxApiClient.Models
{
    // For a full list of available fields see
    // https://github.com/FortnoxAB/csharp-api-sdk/blob/master/FortnoxAPILibrary/Entities/Invoice.cs
    // https://developer.fortnox.se/documentation/resources/invoices/
    public class NewFortnoxInvoice
    {
        public string CustomerNumber { get; set; }
        public string DeliveryDate { get; set; }
        //This needs to map to the WebCrm Delivery Number
        public string OurReference { get; set; }
        public List<InvoiceRow> InvoiceRows { get; set; }
    }
}